using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    partial class Universe
    {
        void SetMatrixElement(int x, int y, UniverseObject universeObject)
        {
            universeMatrix[x, y] = universeObject;
            universeObject.SetCords(x, y);
        }

        public UniverseObject GetMatrixElement(int x, int y)
        {
            return universeMatrix[x, y];
        }

        void RelocateUniverseObject(int x1, int y1, int x2, int y2)
        {
            SetMatrixElement(x2, y2, GetMatrixElement(x1, y1));
            SetMatrixElement(x1, y1, new EmptySpace(ConstsUniverse));
        }

        public int GetObjectDescriptor(int x, int y)
        {
            if (ValidateCords(x, y))
                return GetMatrixElement(x, y).GetDesc();
            return 0;
        }

        bool AddUniverseObject(int x, int y, UniverseObject universeObject, bool canReSetPrevObject)
        {
            if (!ValidateCords(x, y))
                return false;
            if (GetMatrixElement(x, y) == null)
            {
                SetMatrixElement(x, y, universeObject);
                universeObject.SetUniverse(this);
                return true;
            }
            if (canReSetPrevObject || GetMatrixElement(x, y).isDisposed())
            {
                GetMatrixElement(x, y).Dispose();
                SetMatrixElement(x, y, universeObject);
                universeObject.SetUniverse(this);
                return true;
            }
            return false;
        }

        bool ValidateCords(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < width && y < height);
        }

        bool ValidateCords2(int x, int y)
        {
            return (x >= 0 && y >= 0 && x <= width && y <= height);
        }
    }
}
