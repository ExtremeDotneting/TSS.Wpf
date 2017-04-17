using System;
using System.Collections.Generic;
using System.Reflection;

namespace TSS.UniverseLogic
{

    abstract class UniverseObject
    {
        protected int descriptor = 0;
        protected int x = -1;
        protected int y = -1;
        protected Universe universe;
        ConstsUniverse constsUniverse;

        public ConstsUniverse ConstsUniverse
        {
            get { return constsUniverse; }
            private set { constsUniverse = value; }
        }

        protected UniverseObject(ConstsUniverse ctUn)
        {
            ConstsUniverse = ctUn;
        }

        public Universe GetUniverse()
        {
            return universe;
        }

        public void SetCords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        public int GetDesc()
        {
            return descriptor;
        }

        public void Dispose()
        {
            //universe.destroyObject(x, y);
            //disposed = true;
            //disposed = true;
            descriptor = 0;

        }

        public bool isDisposed()
        {
            //return disposed;
            return (descriptor == 0);

        }
    }
}
