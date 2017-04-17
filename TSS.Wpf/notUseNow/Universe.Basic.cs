using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;

namespace TSS.UniverseLogic
{
    partial class Universe:IConvertibleByFields
    {
        int width, height;
        int foodForTick;
        int foodPlaceX1, foodPlaceY1, foodPlaceX2, foodPlaceY2;
        int poisonForTick;
        int poisonPlaceX1, poisonPlaceY1, poisonPlaceX2, poisonPlaceY2;
        UniverseObject[,] universeMatrix;
        List<Cell> cellList = new List<Cell>(0);
        long ticksCount ;
        int blockCellDesc ;
        int maxCellsCount;
        bool disposed;
        ConstsUniverse constsUniverse;


        public ConstsUniverse ConstsUniverse
        {
            get { return constsUniverse; }
            private set { constsUniverse = value; }
        }

        public long Width
        {
            get { return width; }
        }

        public long Height
        {
            get { return height; }
        }

        public int PoisonForTick
        {
            set { poisonForTick = value; }
            get { return poisonForTick; }
        }

        public int FoodForTick
        {
            set { foodForTick = value; }
            get { return foodForTick; }
        }

        public long TicksCount
        {
            get { return ticksCount; }
        }

        public float MaxCellsPersent
        {
            set { maxCellsCount = (int)(value * width * height); }
            get { return maxCellsCount/ (width*height); }
        }

        public int MaxCellsCount
        {
            set { maxCellsCount = value; }
            get { return maxCellsCount; }
        }

        public Universe(int width, int height)
        {
            Initialize(
                width,
                height,
                new ConstsUniverse()
                );
        }

        Universe(Dictionary<string, object> dictionary, int width, int height)
        {
            Initialize(
                width,
                height,
                new ConstsUniverse()
                );
            InitializeDict(dictionary);
        }

        public static Universe CreateFromDictionary(Dictionary<string, object> dictionary)
        {
            return new Universe(
                dictionary,
                Convert.ToInt32(dictionary[@"width"]),
                Convert.ToInt32(dictionary[@"height"])
                );
        }

        public Universe CreateResizedClone(int width, int height)
        {
            Dictionary<string, object> dictionary = ToDictionary();
            Universe res = new Universe(
                dictionary,
                Convert.ToInt32(dictionary[@"width"]),
                Convert.ToInt32(dictionary[@"height"])
                );
            return res;
        }

        void InitializeDict(Dictionary<string, object> dictionary)
        {
            FieldInfo[] ignoreList = new FieldInfo[]
            {
                GetType().GetField("universeMatrix", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            };
            CBFHandler.InitFromDictionaryDef(this, dictionary, ignoreList);

            IEnumerable<Dictionary<string, object>> dictNumerable = (dictionary["universeMatrix"] as IEnumerable<Dictionary<string, object>>);
            int index = 0;
            foreach (Dictionary<string, object> newDict in dictNumerable)
            {
                string type = Convert.ToString(newDict["ObjType"]);
                UniverseObject newUO;
                if (type == typeof(EmptySpace).ToString())
                {
                    newUO= new EmptySpace(ConstsUniverse, dictionary);
                }
                else if (type == typeof(Food).ToString())
                {
                    newUO = new Food(ConstsUniverse, dictionary);
                }
                else if (type == typeof(Cell).ToString())
                {
                    newUO = new Cell(ConstsUniverse, dictionary);
                }
                else
                    throw new Exception(@"Unknown UniverseObject type!");

                AddUniverseObject(newUO.GetX(), newUO.GetY(), newUO, true);
                if (newUO is Cell)
                    cellList.Add(newUO as Cell);
                index++;
            }
        }

        void Initialize(int width, int height, ConstsUniverse ctUniverse)
        {
            if (width <= 0 || height <= 0)
                throw new Exception(@"Can`t create so small Universe!");
            ConstsUniverse = ctUniverse;
            this.width = width;
            this.height = height;
            universeMatrix = new UniverseObject[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    AddUniverseObject(i, j, new EmptySpace(ConstsUniverse), true);
                }
            }

            disposed = false;
            foodForTick = 0;
            poisonForTick = 0;
            ticksCount = 0;
            blockCellDesc = 0;
            MaxCellsPersent=1;
            SetFoodPlace(0, 0, width, height);
            SetPoisonPlace(0, 0, width, height);
        }

        public Dictionary<string, object> ToDictionary()
        {
            //Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //dictionary.Add(@"ObjType", ToString());
            //foreach (var atr in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            //{
            //    object value;
            //    if (atr == null || (value = atr.GetValue(this)) == null)
            //        continue;
            //    string name = atr.Name;

            //    if (typeof(IFormattable).IsAssignableFrom(atr.FieldType))
            //        dictionary.Add(name, value);
            //    else if (name == "universeMatrix")
            //    {
            //        List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>();
            //        foreach (UniverseObject unObj in universeMatrix)
            //            dictList.Add(unObj.ToDictionary());
            //        dictionary.Add(name, dictList);
            //    }
            //}
            //return dictionary;
            object[] ignoreList = new object[] { cellList } ;
            return CBFHandler.ToDictionaryDefault(this, ignoreList);
        }

        public void Dispose()
        {
            disposed = true;
            cellList = null;
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (GetMatrixElement(i,j) != null)
                    {
                        GetMatrixElement(i, j).Dispose();
                        GetMatrixElement(i, j).SetUniverse(null);
                    }
                    SetMatrixElement(i, j, null);
                }
            }
            universeMatrix = null;

        }

        public bool IsDisposed()
        {
            return disposed;
        }

        void HandleCellMove(Cell cell)
        {
            int x1=cell.GetX(), y1=cell.GetY(), x2, y2;
            MoveDirection md = cell.CalcMoveDirectionAspiration();
            switch (md)
            {
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

            if (GetMatrixElement(x2,y2).isDisposed())
            {
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else if (GetMatrixElement(x2,y2) is Food)
            {
                cell.AddEnergy((GetMatrixElement(x2,y2) as Food).EnergyLevel);
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else if (GetMatrixElement(x1, y1).GetDesc() == GetMatrixElement(x2,y2).GetDesc())
            {
                cell.AddEnergy(ConstsUniverse.EnergyLevel_MovesFriendly);
                (GetMatrixElement(x2,y2) as Cell).AddEnergy(ConstsUniverse.EnergyLevel_MovesFriendly);
            }
            else
            {
                cell.AddEnergy((float)(ConstsUniverse.EnergyLevel_MovesAggression * 0.8));
                (GetMatrixElement(x2,y2) as Cell).AddEnergy(-ConstsUniverse.EnergyLevel_MovesAggression);
                
            }

        }

        void HandleAllCellsMoves()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                HandleCellMove(cellList[i]);
            }
        }

        void KillCell(int x, int y)
        {
            if (GetMatrixElement(x, y) is Cell)
                AddUniverseObject(x, y, new Food(ConstsUniverse,FoodType.deadCell), true);
        }

        void CheckAllCells()
        {
            bool notUniverseOverflow = cellList.Count <= maxCellsCount;
            List<Cell> bufCellList = new List<Cell>(0);

            for(int i=0; i<cellList.Count; i++)
            {
                if (cellList[i].isDisposed())
                    continue;         
                if (cellList[i].IncAge() || cellList[i].DecEnergy())
                {
                    KillCell(cellList[i].GetX(), cellList[i].GetY());
                    continue;
                }
                else
                    bufCellList.Add(cellList[i]);

                if (notUniverseOverflow && cellList[i].CalcReproduction() && cellList[i].GetDesc()!=blockCellDesc)
                {
                    List<MoveDirection> md = new List<MoveDirection>(0);
                    int x = cellList[i].GetX();
                    int y = cellList[i].GetY();
                    if (ValidateCords(x,y-1) && GetMatrixElement(x,y-1).GetDesc()<100)
                        md.Add(MoveDirection.up);
                    if (ValidateCords(x, y + 1) && GetMatrixElement(x,y + 1).GetDesc() < 100)
                        md.Add(MoveDirection.down);
                    if (ValidateCords(x-1, y ) && GetMatrixElement(x - 1,y).GetDesc() < 100)
                        md.Add(MoveDirection.left);
                    if (ValidateCords(x+1, y ) && GetMatrixElement(x + 1,y).GetDesc() < 100)
                        md.Add(MoveDirection.right);


                    MoveDirection choice = MoveDirection.stand;
                    if (md.Count>0)
                        choice = md[StableRandom.rd.Next(md.Count)];

                    if (choice == MoveDirection.stand)
                        continue;

                    Cell newCell = cellList[i].Reproduct();
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

        public void DoUniverseTick()
		{
            if (!disposed)
            {
                
                HandleAllCellsMoves();
                GenerateFood(foodForTick);
                GeneratePoison(poisonForTick);
                CheckAllCells();
                CalcMostFitCell();
                if (GetCellsCount() == 0)
                    GenerateCells(1);

                ticksCount++;
            }
        }

      
	}
}
