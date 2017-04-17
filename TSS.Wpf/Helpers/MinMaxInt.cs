using System;

namespace TSS.Helpers
{
    /// <summary>
    /// Quite an interesting class. IFormattable makes it clear that it is converted to a string.
    /// But to reverse the action I had to do Parseble attribute that makes it clear for ValuesRedactor,
    /// that this type has a method "public static MinMaxInt Parse(string str)".
    /// <para></para>
    /// Довольно интересный класс. IFormattable дает понять, что он конвертируется в строку.
    /// Но вот для обратного действия мне пришлось сделать атрибут Parseble, который дает понять ValuesRedactor,
    /// что этот тип имеет метод "public static MinMaxInt Parse(string str)".
    /// </summary>
    [Parseble]
    [Serializable]
    class MinMaxInt : IFormattable
    {
        public int Min = 0;
        public int Max = 0;

        public MinMaxInt()
        {
        }
        public MinMaxInt(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public string ToString(string format = null, IFormatProvider provider = null)
        {
            return Min.ToString() + "/" + Max.ToString();
        }
        public static MinMaxInt Parse(string str)
        {
            try
            {
                string[] arr = str.Split('/');
                MinMaxInt res = new MinMaxInt();
                res.Min = Convert.ToInt32(arr[0]);
                res.Max = Convert.ToInt32(arr[1]);
                return res;
            }
            catch
            {
                //I don`t now why, but program doesn`t throw this exception here.
                //throw new Exception("Can`t parse to MinMaxInt!");
                return null;
            }
        }
    }
}
