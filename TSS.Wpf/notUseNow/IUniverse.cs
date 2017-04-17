using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    interface IUniverse:IDisposable
    {
        ConstsUniverse ConstsUniverseProperty { get; set; }
        int Width { get; }
        int Height { get; }
        void SetFoodCanPlaceAt(int x, int y, bool value);
        void SetPoisonCanPlaceAt(int x, int y, bool value);
        bool GetFoodCanPlaceAt(int x, int y);
        bool GetPoisonCanPlaceAt(int x, int y);
        void DoUniverseTick();
        long GetTicksCount();
        Cell GetMostFit();
        long GetTotalUniverseEnergy();
        int GetCellsCount();
        int[,] GetAllDescriptors();
        int GetObjectDescriptor(int x, int y);
        bool IsDisposed();
    }
}
