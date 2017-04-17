using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    partial class Universe
    {
        Cell MostFitGenome_OneCell;
        int MostFitGenome_CellsCount = 100;

        public bool SetFoodPlace(int x1, int y1, int x2, int y2)
        {
            if (ValidateCords2(x1, y1) && ValidateCords2(x2, y2))
                if (x1 <= x2 && y1 <= y2)
                {
                    foodPlaceX1 = x1;
                    foodPlaceY1 = y1;
                    foodPlaceX2 = x2;
                    foodPlaceY2 = y2;
                    return true;
                }
            return false;
        }

        public bool SetPoisonPlace(int x1, int y1, int x2, int y2)
        {
            if (ValidateCords2(x1, y1) && ValidateCords2(x2, y2))
                if (x1 <= x2 && y1 <= y2)
                {
                    poisonPlaceX1 = x1;
                    poisonPlaceY1 = y1;
                    poisonPlaceX2 = x2;
                    poisonPlaceY2 = y2;
                    return true;
                }
            return false;
        }

        public int[,] GetAllDescriptors()
        {
            int[,] descriptors = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    descriptors[i,j] = GetMatrixElement(i,j).GetDesc();
                }
            }
            return descriptors;
        }

        public DescAndMoveDir[,] GetAllDescriptorsAndMoveDisp()
        {
            DescAndMoveDir[,] descriptors = new DescAndMoveDir[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    descriptors[i,j].desc=universeMatrix[i,j].GetDesc();
                    if (universeMatrix[i,j] is Cell)
                        descriptors[i,j].moveDir = (universeMatrix[i,j] as Cell).MoveDisperation;
                }
            }
            return descriptors;
        }

        public int GetCellsCount()
        {
            return cellList.Count;
        }

        Tuple<Cell, int> GetMostFit()
        {
            return Tuple.Create<Cell, int>(
                MostFitGenome_OneCell, 
                MostFitGenome_CellsCount
                );
        }

        void CalcMostFitCell()
        {
            
            List<int> descriptors = GetCellsDescriptors();
            if (descriptors.Count <= 0)
                return;

            descriptors.Sort();

            List<int> uniqDescs = new List<int>(0);
            List<int> uniqDescsRepeats = new List<int>(0);
            uniqDescs.Add(descriptors[0]);
            uniqDescsRepeats.Add(1);
            int index = 0;
            for (int i = 1; i < descriptors.Count; i++)
            {
                if (descriptors[i] == uniqDescs[index])
                {
                    uniqDescsRepeats[index]++;
                }
                else
                {
                    uniqDescs.Add(descriptors[i]);
                    uniqDescsRepeats.Add(1);
                    index++;
                }
            }
            int highIndex = 0, highValue = uniqDescsRepeats[0];
            for (int i = 1; i < uniqDescs.Count; i++)
            {
                if (uniqDescsRepeats[i] > highValue)
                {
                    highValue = uniqDescsRepeats[i];
                    highIndex = i;
                }
            }

            if (highValue > ConstsUniverse.CellsCount_MaxWithOneType)
                blockCellDesc = uniqDescs[highIndex];
            else
                blockCellDesc = 0;

            MostFitGenome_OneCell = FindCellByDesc(uniqDescs[highIndex]);
            MostFitGenome_CellsCount = highValue;

        }

        public Cell FindCellByDesc(int descriptor)
        {
            if (cellList.Count > 0)
            {
                for (int i = 0; i < cellList.Count; i++)
                {
                    if (cellList[i].GetDesc() == descriptor)
                        return cellList[i];
                }
                return cellList[0];
            }
            return null;
        }

        public List<int> GetCellsDescriptors()
        {
            List<int> descriptors = new List<int>(0);
            for (int i = 0; i < cellList.Count; i++)
            {
                descriptors.Add(cellList[i].GetDesc());
            }
            return descriptors;
        }


        public void GenerateCells(int count)
        {
            if (cellList.Count > 0)
            {
                foreach(Cell cell in cellList.ToArray())
                    KillCell(cell.GetX(), cell.GetY());
            }
            List<Cell> bufCellList = new List<Cell>(0);
            for (int i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(width);
                int y = StableRandom.rd.Next(height);
                Cell cell = new Cell(ConstsUniverse,new Genome(ConstsUniverse));
                if (AddUniverseObject(x, y, cell, false))
                    bufCellList.Add(cell);
            }
            cellList = bufCellList;
        }

        public void GenerateFood(int count)
        {
            if (count < 0)
                return;
            for (int i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(foodPlaceX1, foodPlaceX2);
                int y = StableRandom.rd.Next(foodPlaceY1, foodPlaceY2);
                AddUniverseObject(x, y, new Food(ConstsUniverse,FoodType.defaultFood), false);
            }
        }

        public void GeneratePoison(int count)
        {
            if (count < 0)
                return;
            for (int i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(poisonPlaceX1, poisonPlaceX2);
                int y = StableRandom.rd.Next(poisonPlaceY1, poisonPlaceY2);
                AddUniverseObject(x, y, new Food(ConstsUniverse,FoodType.poison), false);
            }
        }
    }
}
