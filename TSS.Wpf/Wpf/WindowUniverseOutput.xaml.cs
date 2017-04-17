using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TSS.UniverseLogic;
using System.Threading;
using System.Diagnostics;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// The window that displays the simulation. Used UniverseOutputManager. Has a user interface.
    /// <para></para>
    /// Окно, которое отображает симуляцию. Используется в UniverseOutputManager. Имеет пользовательский интерфейс.
    /// </summary>
    public partial class WindowUniverseOutput : Window, IUniverseOutputUIElement
    {
        internal static void ShowModal(Universe universe, Size resolution)
        {
            WindowUniverseOutput w = new WindowUniverseOutput();
            UniverseOutputManager universeOutputManager=null;
            try
            {
                universeOutputManager = new UniverseOutputManager(universe, w, resolution.Width, resolution.Height);
                //This thread urgently complete the simulation, if taken too much memory.
                //Этот поток экстренно завершит симуляцию, если занято слишком много памяти.
                Thread memoryCheckThread = new Thread(() =>
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.Highest;
                        while (true)
                        {
                            int totalMBytesOfMemoryUsed = (int)Process.GetCurrentProcess().WorkingSet64 / 1048576;
                            if (totalMBytesOfMemoryUsed > 900)
                            {
                                OnOutOfMemory(w, universeOutputManager);
                                Thread.CurrentThread.Abort();
                            }
                            Thread.Sleep(300);
                        }

                    });
                memoryCheckThread.Start();
                w.ShowDialog();
                memoryCheckThread.Abort();
                universeOutputManager.Dispose();
            }
            catch(OutOfMemoryException ex)
            {
                OnOutOfMemory(w, universeOutputManager);
            }

            GC.Collect();
        }
        static void OnOutOfMemory(WindowUniverseOutput window, UniverseOutputManager universeOutputManager)
        {
            try
            {
                universeOutputManager.Dispose();
                GC.Collect();
                window.Dispatcher.Invoke(() =>
                {
                    window.Close();
                });
            }
            catch { }
            MessageBox.Show(LanguageHandler.GetInstance().OutOfMemory);
        }

        Image IUniverseOutputUIElement.ImageUniverseField
        {
            get { return imageUniverseField; }
        }
        Image IUniverseOutputUIElement.ImageInfo
        {
            get { return imageSimulationInfo; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnStart
        {
            set { buttonStart.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnPause
        {
            set { buttonPause.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnExit
        {
            set { this.Closing += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnOpenUniverseConstsRedactor
        {
            set { buttonConstsRedactor.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnOpenFoodPlaceRedactor
        {
            set { buttonFoodPlaceRedactot.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnOpenPoisonPlaceRedactor
        {
            set { buttonPosinPlaceRedactot.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnClearUniverseField
        {
            set { buttonClearField.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnGenerateCells
        {
            set
            {
                buttonGenerateCells.Click += value.Invoke;
                textBoxCellsCount.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        value(s, e);
                    }
                };
            }
        }
        int? IUniverseOutputUIElement.CountOfCellsToGenerate
        {
            set { textBoxCellsCount.Text = Convert.ToString(value); }
            get
            {
                try
                {
                    int res = Convert.ToInt32(textBoxCellsCount.Text);
                    if (res > 50000)
                        throw new Exception();
                    return Convert.ToInt32(textBoxCellsCount.Text);
                }
                catch
                {
                    MessageBox.Show(LanguageHandler.GetInstance().CellsCountWarningMessage);
                    return null;
                }
            }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnGenerateFoodOnAllField
        {
            set { buttonGenerateFoodOnAll.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnResetResolution
        {
            set
            {
                buttonResetResolution.Click += value.Invoke;
                textBoxResolutionWidth.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        value(s, e);
                    }
                };
                textBoxResolutionHeight.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        value(s, e);
                    }
                };
            }
        }
        Size? IUniverseOutputUIElement.ResolutionToReset
        {
            set
            {
                Size size = (Size)value;
                textBoxResolutionWidth.Text = Convert.ToString(size.Width);
                textBoxResolutionHeight.Text = Convert.ToString(size.Height);
            }
            get
            {
                try
                {
                    return new Size(
                        Convert.ToInt32(textBoxResolutionWidth.Text),
                        Convert.ToInt32(textBoxResolutionHeight.Text)
                        );
                }
                catch
                {
                    MessageBox.Show(LanguageHandler.GetInstance().ResolutionWarningMessage);
                    return null;
                }
            }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnSaveUniverse
        {
            set { buttonSaveUniverse.Click += value.Invoke; }
        }
        Action<object, EventArgs> IUniverseOutputUIElement.OnGetWorkDeley
        {
            set
            {
                sliderWithValueLabel_Delay.slider.ValueChanged += value.Invoke;
            }
        }
        int? IUniverseOutputUIElement.WorkDelay
        {
            set { sliderWithValueLabel_Delay.slider.Value = (int)value; }
            get { return (int)sliderWithValueLabel_Delay.slider.Value; }
        }

        WindowUniverseOutput()
        {
            InitializeComponent();
            sliderWithValueLabel_Delay.slider.Maximum = 3000;
            sliderWithValueLabel_Delay.slider.Minimum = 0;

            Title = LanguageHandler.GetInstance().TitleOfUniverseOutputWindow;
            tabItem_SimulationInfo.Header = LanguageHandler.GetInstance().TabItem_SimulationInfoHeader;
            tabItem_Game.Header = LanguageHandler.GetInstance().TabItem_GameHeader;
            tabItem_Controls.Header = LanguageHandler.GetInstance().TabItem_ControlsHeader;
            buttonClearField.Content = LanguageHandler.GetInstance().ButtonClearFieldText;
            buttonConstsRedactor.Content = LanguageHandler.GetInstance().ButtonConstsRedactorText;
            buttonFoodPlaceRedactot.Content = LanguageHandler.GetInstance().ButtonFoodPlaceRedactotText;
            buttonGenerateCells.Content = LanguageHandler.GetInstance().ButtonGenerateCellsText;
            buttonGenerateFoodOnAll.Content = LanguageHandler.GetInstance().ButtonGenerateFoodOnAllText;
            buttonPause.Content = LanguageHandler.GetInstance().ButtonPauseText;
            buttonPosinPlaceRedactot.Content = LanguageHandler.GetInstance().ButtonPosinPlaceRedactotText;
            buttonResetResolution.Content = LanguageHandler.GetInstance().ButtonResetResolutionText;
            buttonSaveUniverse.Content = LanguageHandler.GetInstance().ButtonSaveUniverseText;
            buttonStart.Content = LanguageHandler.GetInstance().ButtonStarText;
            labelCellsCount.Content = LanguageHandler.GetInstance().LabelCellsCountText;
            labelDelay.Content = LanguageHandler.GetInstance().LabelDelayText;
            labelHeight.Content = LanguageHandler.GetInstance().LabelHeightText;
            labelWidth.Content = LanguageHandler.GetInstance().LabelWidthText;
        }
    }
}
