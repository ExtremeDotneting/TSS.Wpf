using System;

namespace TSS.UniverseLogic
{
    /// <summary>
    /// An important part of the gaming universe.
    /// <para></para>
    /// Составная часть игровой вселенной.
    /// </summary>
    [Serializable]
    abstract class UniverseObject : IDisposable
    {
        protected int descriptor=0;
        protected int x =-1;
        protected int y=-1;
        ConstsUniverse constsUniverse;
        public ConstsUniverse ConstsUniverseProperty
        {
            get { return constsUniverse; }
            set { constsUniverse = value; }
        }

        public UniverseObject(ConstsUniverse constsUniverse)
        {
            ConstsUniverseProperty = constsUniverse;
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

        /// <summary>
        /// A very important element of the whole system - a descriptor of UniverseObject. It is used to obtain information about an object in the simplest way.
        /// Each child class has his own. Food: -1, -2, -3; Cell: 100+. It contains information not only about the type,
        /// but also the object itself. For example, Food it shows what this type of food for the Cell,
        /// he is also an identifier of its genome (now, after completion of the project, I understand that it was logical to make GetDescriptor abstract,
        /// to the descendants themselves decide how to implement it. Then would be the ability to store in his cell genome,
        /// it would have saved a lot of problems.). Cells use it to make decisions about movement, engine - learns how to draw object.
        /// By the way, the remote default method Dispose object has a handle of 0.
        /// <para></para>
        /// Очень важный элемент всей системы - это дескриптор UniverseObject. Он используется для получения сведений об объекте самым простым путем.
        /// У каждого дочернего класса он свой. У Food: -1,-2,-3; Cell: 100+. Он несет информацию не только о типе, но и о самом объекте.
        /// Например, для Food он показывает что это за тип еды, для Cell он является еще и идентификатором ее генома
        /// (сейчас, после завершения проекта я понимаю, что логичней было сделать GetDescriptor абстрактным, чтоб потомки сами решали
        /// как его реализовывать. Тогда была бы возможность хранить его в геноме клеток, это избавило бы от многих проблем.).
        /// Клетки используют его для принятия решений о движении, движок - узнает как отрисовывать объект. Кстати, удаленный методом Dispose
        /// объект по умолчанию имеет дескриптор равный 0.
        /// </summary>
        public int GetDescriptor()
        {
            return descriptor;
        }
        public virtual void Dispose()
        {
            descriptor = 0;
        }
        public bool IsDisposed()
        {
            return (descriptor == 0);      
        }
	}
}
