using System;
using System.Windows;
using System.Windows.Controls;

namespace TSS.Wpf
{
    /// <summary>
    /// In my project it is implemented by the form in which the simulation runs - WindowUniverseOutput.
    /// <para></para>
    /// В моем проекте этот интерфейс реализует форма, в которой работает симуляция - WindowUniverseOutput. 
    /// </summary>
    interface IUniverseOutputUIElement
    {
        Image ImageUniverseField { get; }
        Image ImageInfo { get; }
        Action<object, EventArgs> OnStart { set; }
        Action<object, EventArgs> OnPause { set; }
        Action<object, EventArgs> OnExit { set; }
        Action<object, EventArgs> OnOpenUniverseConstsRedactor { set; }
        Action<object, EventArgs> OnOpenFoodPlaceRedactor { set; }
        Action<object, EventArgs> OnOpenPoisonPlaceRedactor { set; }
        Action<object, EventArgs> OnClearUniverseField { set; }
        Action<object, EventArgs> OnGenerateCells { set; }
        int? CountOfCellsToGenerate { set; get; }
        Action<object, EventArgs> OnGenerateFoodOnAllField { set; }
        Action<object, EventArgs> OnResetResolution { set; }
        Size? ResolutionToReset { set; get; }
        Action<object, EventArgs> OnSaveUniverse { set; }
        Action<object, EventArgs> OnGetWorkDeley { set; }
        int? WorkDelay { set; get; }
    }
}
