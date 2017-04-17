using System;

namespace TSS.UniverseLogic
{
    /// <summary>
    /// An important part of the gaming universe.
    /// <para></para>
    /// ��������� ����� ������� ���������.
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
        /// ����� ������ ������� ���� ������� - ��� ���������� UniverseObject. �� ������������ ��� ��������� �������� �� ������� ����� ������� �����.
        /// � ������� ��������� ������ �� ����. � Food: -1,-2,-3; Cell: 100+. �� ����� ���������� �� ������ � ����, �� � � ����� �������.
        /// ��������, ��� Food �� ���������� ��� ��� �� ��� ���, ��� Cell �� �������� ��� � ��������������� �� ������
        /// (������, ����� ���������� ������� � �������, ��� �������� ���� ������� GetDescriptor �����������, ���� ������� ���� ������
        /// ��� ��� �������������. ����� ���� �� ����������� ������� ��� � ������ ������, ��� �������� �� �� ������ �������.).
        /// ������ ���������� ��� ��� �������� ������� � ��������, ������ - ������ ��� ������������ ������. ������, ��������� ������� Dispose
        /// ������ �� ��������� ����� ���������� ������ 0.
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
