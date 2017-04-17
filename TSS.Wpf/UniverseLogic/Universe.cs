using System;
using System.Collections.Generic;
using TSS.Another;
using TSS.Helpers;

namespace TSS.UniverseLogic
{
    /// <summary>
    /// There is the whole gameplay in this class.
    /// <para></para>
    /// В этом классе происходит весь игровой процесс.
    /// </summary>
    [Serializable]
    class Universe
    {
        long totalUniverseEnergy=0;

        /// <summary>
        /// The matrix that stores all of the objects of the game field.
        /// <para></para>
        /// Матрица в которой хранятся все объекты игрового поля.
        /// </summary>
        UniverseObject[,] universeMatrix;
        bool[,] poisonPlace, foodPlace;

        /// <summary>
        /// This list contains all of the cells for quick access to them.
        /// <para></para>
        /// В этом списке хранятся все клетки для быстрого доступа к ним.
        /// </summary>
        List<Cell> cellList = new List<Cell>(0);
        int width, height;
        long ticksCount ;
        Cell MostFitGenome_OneCell;
        ConstsUniverse constsUniverse;
        bool disposed = false;
        public ConstsUniverse ConstsUniverseProperty
        {
            get { return constsUniverse; }
            set
            {
                /*There had to implement a crutch. Because UniverseObject have access to that object not through Universe,
                but directly by storing a reference to it, would have to change the pointer in each of these objects, it is slow and not reliable.
                For this reason I made this crutch, which copies the data from the input value to an existing object. * /

                /*Здесь пришлось реализовать костыль. Так-как UniverseObject имеют доступ к этому объекту не через Universe, 
                а напрямую, храня ссылку на него, пришлось бы менять указатель в каждом из этих объектов, это медленно и не надежно.
                По этой причине я сделал этот костыль, который копирует данные из входного значения в имеющийся объект.*/
                if (constsUniverse == null)
                {
                    constsUniverse = value;
                }
                else
                {
                    var xml = SerializeHandler.FieldValuesToXml(value);
                    SerializeHandler.FieldValuesFromXml(constsUniverse, xml);
                }
            }
        }
        public int Height
        {
            get { return height; }
        }
        public int Width
        {
            get { return width; }
        }
        List<Action> invokedActions = new List<Action>();
        int typesOfCellsCount = 0;
        public int TypesOfCellsCount
        {
            get { return typesOfCellsCount; }
            private set { typesOfCellsCount = value; }
        }

        public Universe(int width, int height)
        {
            this.width = width;
            this.height = height;
            foodPlace = new bool[width, height];
            poisonPlace = new bool[width, height];
            SetDefaultObjectsAccessiblePlace();
            ConstsUniverseProperty = new ConstsUniverse();
            universeMatrix = new UniverseObject[width,height];
            ticksCount = 0;
        }
        public void Dispose()
        {
            disposed = true;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GetMatrixElement(i, j).Dispose();
                }
            }
            universeMatrix=null;
            poisonPlace = null;
            foodPlace=null;
            cellList = null;
            MostFitGenome_OneCell=null;
            constsUniverse=null;
            invokedActions = null;
        }
        public bool IsDisposed()
        {
            return disposed;
        }

        /// <summary>
        /// By calling this method, you spend all the calculations in the universe (game).
        /// They are held only here, it will make for reliable operation and synchronization of the entire system.
        /// <para></para>
        /// Вызывая этот метод вы проводите все вычисления во вселенной(игры).
        /// Они проводятся только здесь, это сделанно для надежности работы и синхронизации всей системы.
        /// </summary>
        public void DoUniverseTick()
        {
            
            if (IsDisposed())
                return;
            CheckAllCells();
            HandleAllCellsMoves();
            GenerateFood(ConstsUniverseProperty.Special_FoodCountForTick);
            GeneratePoison(ConstsUniverseProperty.Special_PoisonCountForTick);
            CalcMostFitCell();
            if (ticksCount%15 == 1)
                CalcTotalUniverseEnergy();
            ticksCount++;

            if (invokedActions.Count > 0)
            {
                foreach (Action act in invokedActions.ToArray())
                {
                    act();
                    invokedActions.Remove(act);
                }
            }
            
        }

        /// <summary>
        /// Every object in the universe has its own a descriptor. More about it in the comments to UniverseObject.
        /// This method allows you to find a descriptor of the object by coordinates on the field.
        /// <para></para>
        /// У каждого объекта во вселенной есть свой дескриптор. Подробней о нем в комментариях к UniverseObject.
        /// Этот метод позволяет узнать дескриптор объекта по координатам на поле.
        /// </summary>
        public int GetObjectDescriptor(int x, int y)
        {
            if (ValidateCords(x, y) && GetMatrixElement(x, y)!=null) 
                return GetMatrixElement(x, y).GetDescriptor();
            return 0;
        }

        /// <summary>
        /// It returns an array of descriptors. In my project is used for rendering the game.
        /// <para></para>
        /// Возвращает массив дескрипторов. В моем проекте используется для рендеринга игры.
        /// </summary>
        public int[,] GetAllDescriptors()
        {
            int[,] descriptors = new int[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    descriptors[i, j] = GetObjectDescriptor(i,j);
                }
            }
            return descriptors;
        }
        public int GetCellsCount()
        {
            return cellList.Count;
        }
        public long GetTicksCount()
        {
            return ticksCount;
        }
        public Cell GetMostFitCell()
        {
            return MostFitGenome_OneCell; 
        }
        public long GetTotalUniverseEnergy()
        {
            return totalUniverseEnergy;
        }
        public UniverseObject GetMatrixElement(int x, int y)
        {
            return universeMatrix[x, y];
        }

        /// <summary>
        /// It uses the constructor to mark the place where the poison and food is generated. For the default food - all place.
        /// To poison - 1/50 of all cells, selected randomly.
        /// <para></para>
        /// Используется в конструкторе чтоб отметить место, где генерируется яд и еда. Для еды по умолчанию - все пространство. 
        /// Для яда - 1/50 всех ячеек, выбирается случайно.
        /// </summary>
        public void SetDefaultObjectsAccessiblePlace()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    SetFoodCanPlaceAt(i, j, true);
                    SetPoisonCanPlaceAt(i, j, StableRandom.rd.Next(0, 50) == 1);
                }
            }
        }
        public void SetFoodCanPlaceAt(int x, int y, bool value)
        {
            if (ValidateCords(x, y))
                foodPlace[x, y] = value;
        }
        public bool GetFoodCanPlaceAt(int x, int y)
        {
            return foodPlace[x, y];
        }
        public void SetPoisonCanPlaceAt(int x, int y, bool value)
        {
            if (ValidateCords(x, y))
                poisonPlace[x, y] = value;
        }
        public bool GetPoisonCanPlaceAt(int x, int y)
        {
            return poisonPlace[x, y];
        }

        /// <summary>
        /// It marks the place where food can be generated using the input array. The same with poison.
        /// <para></para>
        /// Отмечает место где может генерироваться еда используя входной массив. То же самое с ядом.
        /// </summary>
        public void SetAllPlaceOfFood(bool[,] fieldDescription)
        {
            if (fieldDescription.GetLength(0) != width || fieldDescription.GetLength(1) != height)
            {
                throw new Exception("Field description size are not correct.");
            }
            foodPlace = fieldDescription.Clone() as bool[,];
        }
        public bool[,] GetAllPlaceOfFood()
        {
            return foodPlace.Clone() as bool[,];
        }
        public void SetAllPlaceOfPoison(bool[,] fieldDescription)
        {
            if (fieldDescription.GetLength(0) != width || fieldDescription.GetLength(1) != height)
            {
                throw new Exception("Field description size are not correct.");
            }
            poisonPlace = fieldDescription.Clone() as bool[,];
        }
        public bool[,] GetAllPlaceOfPoison()
        {
            return poisonPlace.Clone() as bool[,];
        }

        /// <summary>
        /// Clears the field from all objects.
        /// Note: The method is executed asynchronously on the DoUniverseTick thread.
        /// <para></para>
        /// Очищает поле от всех объектов.
        /// Примечание: метод выполнится асинхронно в потоке DoUniverseTick.
        /// </summary>
        public void ClearField(bool isAsync = true)
        {
            Action act = new Action(() =>
              {
                  cellList = new List<Cell>();
                  MostFitGenome_OneCell = null;
                  universeMatrix = new UniverseObject[width, height];
                  TypesOfCellsCount = 0;
              });
            if (isAsync)
                BeginInvokeAction(act);
            else
                act();
        }

        /// <summary>
        /// Generates a predetermined number of cells. From the outside it is recommended to call it asynchronously, 
        /// then it will be executed in the flow DoUniverseTick. Asynchronous he was invoked before, to generate cells before the start of the game.
        /// <para></para>
        /// Генерирует заданное количество клеток. Извне рекомендуется вызывать его асинхронно, тогда он выполнится в потоке DoUniverseTick.
        /// Асинхронно он вызывался раньше, чтоб сгенерировать клетки перед стартом игры.
        /// </summary>
        public void GenerateCells(int count, bool isSync = true)
        {

            Action act = new Action(() =>
              {
                  //List<Cell> bufCellList = new List<Cell>(0);
                  for (int i = 0; i < count; i++)
                  {

                      int x = StableRandom.rd.Next(width);
                      int y = StableRandom.rd.Next(height);
                      Cell cell = new Cell(ConstsUniverseProperty)/*need_factory*/;
                      if (AddUniverseObject(x, y, cell, true))
                      {
                          cellList.Add(cell);
                          TypesOfCellsCount++;
                      }
                  }
                  //cellList.AddRange(bufCellList);
              });
            if (isSync)
                BeginInvokeAction(act);
            else
                act();

        }

        /// <summary>
        /// It fills the entire field of food.
        /// <para></para>
        /// Заполняет все поле едой.
        /// </summary>
        public void GenerateFoodOnAllField(bool isAsync=true)
        {
            ClearField(isAsync);

            Action act = new Action(() =>
              {
                  for (int i = 0; i < width; i++)
                  {
                      for (int j = 0; j < height; j++)
                      {
                          SetMatrixElement(i, j, new Food(ConstsUniverseProperty, FoodType.defaultFood));
                      }
                  }
              });
            if (isAsync)
                BeginInvokeAction(act);
            else
                act();
            
        }

        /// <summary>
        /// The method has will be performed during the next tick. This is done for the stable operation of the system.
        /// <para></para>
        /// Переданный метод будет выполнен во время следующего тика. Это сделанно для устойчивой работы системы.
        /// </summary>
        void BeginInvokeAction(Action action)
        {
            invokedActions.Add(action);
        }

        /// <summary>
        /// Checks to x, y did not go out of bounds.
        /// <para></para>
        /// Проверяет чтоб x,y не выходили за границы поля.
        /// </summary>
        bool ValidateCords(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < width && y < height);
        }

        /// <summary>
        /// This method, moreover, which adds the object to the field monitors to the object so that the object got its current position in the matrix.
        /// It is necessary for Cell. When we calculate their direction of movement, we work through a list of cells,
        /// instead of through the elements matrix (field of this class). That has to be stored in the object coordinates and change them when moving.
        /// <para></para>
        /// Этот метод, кроме того, что добавляет объект на поле, следит чтоб объект чтоб объект получил свои текущие координаты в матрице.
        /// Это нужно для Cell. Когда мы высчитываем их направление движения, мы работаем через список клеток,
        /// а не через матрицу элементов (речь о полях данного класса). Вот и приходится хранить координаты в самом объекте и менять их при перемещении.
        /// </summary>
        void SetMatrixElement(int x, int y, UniverseObject universeObject)
        {
            universeMatrix[x,y] = universeObject;
            if(universeObject!=null)
                universeObject.SetCords(x, y);
        }

        /// <summary>
        /// Extended version of SetMatrixElement. Among other things, monitoring the removal of the prev item.
        /// <para></para>
        /// Расширенная версия SetMatrixElement. Кроме прочего, следит за удалением старого элемента.
        /// </summary>
        bool AddUniverseObject(int x, int y, UniverseObject universeObject, bool canReSetPrevObject)
        {
            if (GetMatrixElement(x, y) == null)
            {
                SetMatrixElement(x, y, universeObject);
                return true;
            }
            else if (canReSetPrevObject || GetMatrixElement(x, y).IsDisposed())
            {
                GetMatrixElement(x, y).Dispose();
                SetMatrixElement(x, y, universeObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// It moves the object to a new location by removing the object which we are moving. At the site of the old object is formed empty space.
        /// <para></para>
        /// Перемещает объект на новое место удаляя тот объект, куда мы перемещаемся. На месте старого объекта образуется пустое пространство.
        /// </summary>
        void RelocateUniverseObject(int x1, int y1, int x2, int y2)
        {
            SetMatrixElement(x2, y2, GetMatrixElement(x1, y1));
            SetMatrixElement(x1, y1, null);
        }
        void GenerateFood(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = StableRandom.rd.Next(0, width);
                int y = StableRandom.rd.Next(0, height);
                if(GetFoodCanPlaceAt(x,y))
                {
                    if (GetObjectDescriptor(x, y) > 100)
                    {
                        try
                        {
                            (GetMatrixElement(x, y) as Cell).AddEnergy(ConstsUniverseProperty.EnergyLevel_DefFood);
                        }catch { }
                    }
                    else
                    {
                        AddUniverseObject(x, y, new Food(ConstsUniverseProperty, FoodType.defaultFood), true);
                    }
                }
            }
        }
        void GeneratePoison(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = StableRandom.rd.Next(0, width);
                int y = StableRandom.rd.Next(0, height);
                if (GetPoisonCanPlaceAt(x, y))
                {
                    if (GetObjectDescriptor(x,y) > 100)
                    {
                        try
                        {
                            (GetMatrixElement(x, y) as Cell).AddEnergy(ConstsUniverseProperty.EnergyLevel_PoisonedFood);
                        }
                        catch { }
                    }
                    else
                    {
                        AddUniverseObject(x, y, new Food(ConstsUniverseProperty, FoodType.poison), true);
                    }
                }
                    
            }
        }
        void KillCell(int x, int y)
        {
            GetMatrixElement(x, y).Dispose();
            AddUniverseObject(x, y, new Food(ConstsUniverseProperty,FoodType.deadCell)/*need_factory*/, true);
        }

        /// <summary>
        /// /// This method is called from DoUniverseTick for all cells. It is calculated for each cell the direction of its movement.
        /// And as soon as an event occurs: the cell takes damage, moves ...
        /// <para></para>        
        /// Этот метод вызывается из DoUniverseTick для всех клеток. В нем для каждой клетки высчитывается направление ее движения.
        /// И сразу же происходит событие: клетка получает урон, перемещается...
        /// </summary>
        void HandleCellMove(Cell cell)
        {
            if (cell.IsDisposed())
                return;
            int x1=cell.GetX(), y1=cell.GetY(), x2, y2;
            cell.CalcMoveDirectionAspiration(this);
            MoveDirection md = cell.GetMoveDisperation();
            switch (md)
            {
                case MoveDirection.stand:
                    return;
                case MoveDirection.up:
                    x2 = x1; y2 = y1 - 1;
                    break;
                case MoveDirection.down:
                    x2 = x1; y2 = y1 + 1;
                    break;
                case MoveDirection.left:
                    x2 = x1-1; y2 = y1;
                    break;
                case MoveDirection.right:
                    x2 = x1+1; y2 = y1;
                    break;
                default:
                    return;
            }

            if (!ValidateCords(x2, y2))
                return;

            UniverseObject unObj = GetMatrixElement(x2, y2);
            if (unObj == null || unObj.IsDisposed())
            {
                //Если пустое пространство.
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else
            {
                int desc = unObj.GetDescriptor();
                if (desc < 0)
                {
                    //Если еда или яд.
                    cell.AddEnergy((unObj as Food).GetEnergyLevel());
                    RelocateUniverseObject(x1, y1, x2, y2);
                }
                else if (cell.GetDescriptor() == desc)
                {
                    //Если клетка - родственник
                    cell.AddEnergy(ConstsUniverseProperty.EnergyLevel_MovesFriendly);
                    (unObj as Cell).AddEnergy(ConstsUniverseProperty.EnergyLevel_MovesFriendly);
                }
                else
                {
                    Cell anotherCell = unObj as Cell;
                    if ((ConstsUniverseProperty.Mutation_AttackChildrenMutantsOfFirstGeneration || cell.GetDescriptor() != anotherCell.GetParentDescriptor()) && 
                        (ConstsUniverseProperty.Mutation_AttackParentIfCellIsYouMutant || cell.GetParentDescriptor() != anotherCell.GetDescriptor()))
                    {
                        /*Если это клетка с другим геномом, который является ребенком или родителем генома текущей клетки, и при этом включена атака.
                         Или же если клетки имеют совсем различное происхождение.*/
                        cell.AddEnergy((float)(ConstsUniverseProperty.EnergyLevel_MovesAggression * 0.8));
                        anotherCell.AddEnergy(-ConstsUniverseProperty.EnergyLevel_MovesAggression);
                    }
                }
            }

        }
        void HandleAllCellsMoves()
        {
            foreach (Cell cell in cellList)
            {
                HandleCellMove(cell);
            }
        }

        /// <summary>
        /// This method is called from DoUniverseTick. In this method, the main action - check whether the cells have died. Not checked whether they were
        /// deleted from the memory external method Dispose, checked their energy level, age.If all is well, it is calculated whether they can reproduce. 
        /// If it is ok creates another cell - child of it. So is calculated in this method, the total number of cells per field.
        /// Calculating the number of cell types, but the feature still does not work reliably.
        /// Sometimes the list of cells is mixed, so the calculation of their moves was more honest.
        /// <para></para>
        /// Этот метод вызывается из DoUniverseTick. В этом методе главное действие - проверка не умерли ли клетки. Проверяется не были ли они удалены из памяти
        /// извне методом Dispose, проверяется их уровень энергии, возраст. Если все хорошо, то высчитывается могут ли они размножаться.В случае удачи
        /// рядом создается еще одна клетка. Так-же в этом методе высчитывается общее количество клеток на поле. Высчитывается количество типов клеток,
        /// но эта функция до сих пор работает ненадежно. Иногда список клеток перемешивается, чтоб расчет их ходов был более честным.
        /// </summary>
        void CheckAllCells()
        {
            int cellCountWas = GetCellsCount();
            List<Cell> bufCellList = new List<Cell>(0);

            for (int i = 0; i <cellList.Count;i++)
            {
                if (cellList[i].IsDisposed())
                {
                    cellCountWas--;
                    continue;
                }
                else
                    bufCellList.Add(cellList[i]);

                if (cellList[i].IncAge() || cellList[i].DecEnergy())
                {
                    cellCountWas--;
                    KillCell(cellList[i].GetX(), cellList[i].GetY());
                    if (cellList[i].GetCellsCountWithThisDescriptor() <= 0)
                        TypesOfCellsCount--;
                    //if (cellList[i].GetCellsCountWithThisDescriptor() <= 0)
                    //    TypesOfCellsCount--;
                    continue;
                }

                bool notUniverseOverflow = cellCountWas <= ConstsUniverseProperty.CellsCount_MaxAtField;
                if (notUniverseOverflow && cellList[i].CanReproduct() && 
                    cellList[i].GetCellsCountWithThisDescriptor()<ConstsUniverseProperty.CellsCount_MaxWithOneType)
                {
                    List<MoveDirection> md = new List<MoveDirection>(0);
                    int x = cellList[i].GetX();
                    int y = cellList[i].GetY();
                    if (ValidateCords(x,y-1) && GetObjectDescriptor(x, y - 1) < 100  /*&& GetObjectDescriptor(x, y - 1)!=-3  */)
                        md.Add(MoveDirection.up);
                    if (ValidateCords(x, y + 1) && GetObjectDescriptor(x, y + 1) < 100  /*&& GetObjectDescriptor(x, y + 1) != -3 */)
                        md.Add(MoveDirection.down);
                    if (ValidateCords(x-1, y ) && GetObjectDescriptor(x - 1, y) < 100  /* && GetObjectDescriptor(x - 1, y) != -3 */)
                        md.Add(MoveDirection.left);
                    if (ValidateCords(x+1, y ) && GetObjectDescriptor(x + 1, y) < 100  /*&& GetObjectDescriptor(x + 1, y) != -3 */)
                        md.Add(MoveDirection.right);

                    MoveDirection choice = MoveDirection.stand;
                    if (md.Count>0)
                        choice = md[StableRandom.rd.Next(md.Count)];

                    if (choice == MoveDirection.stand)
                        continue;

                    Cell newCell = cellList[i].CreateChild(TypesOfCellsCount<ConstsUniverseProperty.MaxCountOfCellTypes);
                    
                    switch (choice)
                    {
                        case MoveDirection.up:
                            AddUniverseObject(x, y - 1, newCell, true);
                            break;
                        case MoveDirection.down:
                            AddUniverseObject(x, y + 1, newCell, true);
                            break;
                        case MoveDirection.left:
                            AddUniverseObject(x - 1, y, newCell, true);
                            break;
                        case MoveDirection.right:
                            AddUniverseObject(x+1, y, newCell, true);
                            break;

                    }
                    bufCellList.Add(newCell);
                    if (newCell.GetDescriptor() != cellList[i].GetDescriptor())
                        TypesOfCellsCount++;
                    cellCountWas++;
                }
            }

            List<Cell> bufCellList2;
            if (ticksCount % 11 == 0)
            {
                bufCellList2 = new List<Cell>(0);
                while (bufCellList.Count > 0)
                {
                    int index = StableRandom.rd.Next(bufCellList.Count);
                    bufCellList2.Add(bufCellList[index]);
                    bufCellList.RemoveAt(index);
                }
            }
            else
                bufCellList2 = bufCellList;

            cellList =bufCellList2;
        }

        /// <summary>
        /// Find the most successful type of cell.
        /// <para></para>
        /// Находит самый успешный тип клеток.
        /// </summary>
        void CalcMostFitCell()
        {
            int maxCount = 0;
            Cell cell = null;
            if (cellList.Count>0)
                cell=cellList[0];
            foreach(Cell item in cellList)
            {
                int count=item.GetCellsCountWithThisDescriptor();
                if (count > maxCount)
                {
                    maxCount = count;
                    cell = item;
                }
            }
            MostFitGenome_OneCell = cell;
        }
        void CalcTotalUniverseEnergy()
        {
            long sum = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    UniverseObject uo = GetMatrixElement(i, j);
                    if (uo != null && !uo.IsDisposed() && typeof(IHasEnergy).IsAssignableFrom(uo.GetType()))
                        sum += (long)(uo as IHasEnergy).GetEnergyLevel();

                }
            }
            totalUniverseEnergy = sum;
        }

    }
}
