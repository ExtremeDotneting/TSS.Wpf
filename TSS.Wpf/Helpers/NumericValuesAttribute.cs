using System;

namespace TSS.Helpers
{
    /// <summary>
    /// Gives to ValuesRedactor an idea of how to display the class fields.
    /// <para></para>
    /// Дает ValuesRedactor представление о том, как отображать поле класса.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    class NumericValuesAttribute : Attribute
    {
        public NumericValuesWayToShow WayToShow = NumericValuesWayToShow.Default;
        public double Min = double.MinValue, Max = double.MaxValue;

        public NumericValuesAttribute(double min, double max)
        {
            Min = min;
            Max = max;
        }
        public NumericValuesAttribute(double min, double max, NumericValuesWayToShow wayToShow)
        {
            Min = min;
            Max = max;
            WayToShow = wayToShow;
        }
        public NumericValuesAttribute(NumericValuesWayToShow wayToShow)
        {
            WayToShow = wayToShow;
        }
    }

    enum NumericValuesWayToShow { Default, Slider }
}

