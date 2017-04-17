using System;

namespace TSS.Another
{
    /// <summary>
    /// The classes in this file is used by all application.
    /// <para></para>
    /// Классы из этого файла используются всем приложением.
    /// </summary>
    class PointInt
    {
        public int X = 0;
        public int Y = 0;
        public PointInt()
        {
        }
        public PointInt(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    static class StableRandom
    {
        public static Random rd = new Random();
    }
}
