using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSS.UniverseLogic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.IO;
using TSS.Helpers;
using System.Windows.Threading;
using System.Windows.Input;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// Also important class, it is transmitted a pointer to form where the game is displayed. 
    /// Acts as an intermediary between the user and the gaming processes. In a separate thread to control the game and rendering,
    /// it synchronizes the user's actions.
    /// <para></para>
    /// Тоже важный класс, в него передается указатель на форму, где отображается игра.
    /// Выступает посредником между пользователем и игровыми процесами. В отдельном потоке управляет игрой и отрисовкой, синхронизирует это с действиями пользователя.
    /// </summary>
    class UniverseOutputManager
    {
        Thread allWorkThread;
        Universe universe;
        public Universe UniverseProperty
        {
            get { return universe; }
        }
        RenderManagerMainField renderManagerMainField;
        public RenderManagerMainField RenderManagerMainFieldProperty
        {
            get { return renderManagerMainField; }
        }
        RenderManagerInfoText renderManagerInfoText;
        public RenderManagerInfoText RenderManagerInfoTextProperty
        {
            get { return renderManagerInfoText; }
        }
        IUniverseOutputUIElement universeOutputUIElement;
        System.Windows.Input.MouseButtonEventHandler mouseDownOnFieldImage;
        double maxInfoTextWidth = 500;
        double infoTextFontSize = 20;
        int pauseBetweenRender = 0;
        int PauseBetweenRender
        {
            get { return pauseBetweenRender; }
            set { pauseBetweenRender = value; }
        }
        enum WorkStatus { inRun, finishing, stoped}
        WorkStatus currentWorkStatus;
        bool disposed;
        List<Action> invoked = new List<Action>();

        public UniverseOutputManager(Universe universe, IUniverseOutputUIElement universeOutputUIElement,double pixelsWidth, double pixelsHeight)
        {
            this.universe = universe;
            this.universeOutputUIElement = universeOutputUIElement;
            renderManagerMainField = new RenderManagerMainField(UniverseProperty.Width, UniverseProperty.Height, pixelsWidth, pixelsHeight);
            renderManagerInfoText = new RenderManagerInfoText();
            universeOutputUIElement.ResolutionToReset = new Size(pixelsWidth, pixelsHeight);
            universeOutputUIElement.WorkDelay = PauseBetweenRender;
            universeOutputUIElement.CountOfCellsToGenerate = 10;
            StartWork();
            InitializeEvents();
        }
        public void Dispose()
        {
            if (!IsDisposed())
            {
                universeOutputUIElement.ImageUniverseField.MouseDown -= mouseDownOnFieldImage;
                StopWork();
                universe = null;
                renderManagerMainField = null;
                renderManagerInfoText = null;
                universeOutputUIElement = null;
                disposed = true;
            }
        }
        public bool IsDisposed()
        {
            return disposed;
        }
        public void StartWork()
        {
            if (allWorkThread != null)
            {
                PauseWork();
            }
            currentWorkStatus = WorkStatus.inRun;
            allWorkThread = new Thread(() =>
              {
                  Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                  while (allWorkThread == Thread.CurrentThread && currentWorkStatus == WorkStatus.inRun)
                  {
                      var invokedBuf = invoked.ToArray();
                      foreach (var method in invokedBuf)
                      {
                          method();
                          invoked.Remove(method);
                      }

                      UniverseProperty.DoUniverseTick();
                      ImageSource img = RenderManagerMainFieldProperty.RenderField(UniverseProperty.GetAllDescriptors());
                      img.Freeze();
                      ImageSource info = RenderManagerInfoTextProperty.DrawUniverseInfo(UniverseProperty, maxInfoTextWidth, infoTextFontSize, Brushes.Black);
                      info.Freeze();
                      universeOutputUIElement.ImageUniverseField.Dispatcher.Invoke(() =>
                      {
                          try
                          {
                              universeOutputUIElement.ImageUniverseField.Source = img;
                          }
                          catch { }
                      });
                      universeOutputUIElement.ImageInfo.Dispatcher.Invoke(() =>
                      {
                          try
                          {
                              universeOutputUIElement.ImageInfo.Source = info;
                          }
                          catch { }
                      });
                      Thread.Sleep(PauseBetweenRender);

                  }
              });
            allWorkThread.Start();
        }

        /// <summary>
        /// Pauses game asynchronously.
        /// <para></para>
        /// Приостанавливает игру асинхронно.
        /// </summary>
        public async void PauseWorkAsync()
        {
            Task waiter=new Task(delegate {
                PauseWork();
            });
            waiter.Start();
            await waiter;
        }

        /// <summary>
        /// Pauses game synchronously. But can not be used int the main stream, because it blocks.
        /// <para></para>
        /// Приостанавливает игру синхронно. Но не может использоватся из главного потока, так как блокирует его.
        /// </summary>
        public void PauseWork()
        {
            if (allWorkThread == null)
                return;
            currentWorkStatus = WorkStatus.finishing;
            allWorkThread.Join(5000);
            StopWork();       
        }

        /// <summary>
        /// Immediately interrupts the game thread.
        /// <para></para>
        /// Сразу же обрывает работу потока игры.
        /// </summary>
        public void StopWork()
        {
            currentWorkStatus = WorkStatus.stoped;
            if (allWorkThread == null)
                return;
            allWorkThread.Abort();
            allWorkThread = null;
            
        }

        /// <summary>
        /// allWorkThread accurately completed, will run the action, allWorkThread restarted. The action will run asynchronously from the calling thread.
        /// <para></para>
        /// allWorkThread аккуратно завершается, действие выполнится, allWorkThread запускается заново. Действие выполнится асинхронно от вызывающего потока.
        /// </summary>
        public void InvokeByPause(Action action)
        {
            Dispatcher disp = Dispatcher.CurrentDispatcher;
            new Thread(() =>
            {
                WorkStatus bufWorkStatus = currentWorkStatus;
                PauseWork();
                disp.Invoke(action);
                if (bufWorkStatus == WorkStatus.inRun)
                    StartWork();
                else
                    currentWorkStatus = bufWorkStatus;
            }).Start();
        }

        /// <summary>
        /// Action is performed in allWorkThread.
        /// <para></para>
        /// Действие выполнится в allWorkThread.
        /// </summary>
        public void InvokeAsynch(Action action)
        {
            Dispatcher disp=Dispatcher.CurrentDispatcher;
            invoked.Add(() =>
            { 
                disp.Invoke(()=>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    action();
                    
                    
                });
                //invoke cursor this to set default cursor after next frame
                invoked.Add(() =>
                  {
                      disp.Invoke(() =>
                        {
                            Mouse.OverrideCursor = Cursors.Arrow;
                        });
                  });

            });
        }

        /// <summary>
        /// It attaches events to IUniverseOutputUIElement.
        /// <para></para>
        /// Прикрепляет к IUniverseOutputUIElement события.
        /// </summary>
        void InitializeEvents()
        {
            mouseDownOnFieldImage = (s, ev) =>
            {
                try
                {
                    if (UniverseProperty != null)
                    {
                        Image image = s as Image;
                        Point pos = ev.GetPosition(image);
                        PointInt posCube = RenderManagerMainFieldProperty.GetCubePosition(pos.X, pos.Y, image.ActualWidth, image.ActualHeight);
                        UniverseObject uo = UniverseProperty.GetMatrixElement(posCube.X, posCube.Y);
                        if (uo is Cell)
                        {
                            WindowImage.ShowModal(
                                RenderManagerInfoTextProperty.DrawCellInfo(uo as Cell, maxInfoTextWidth, infoTextFontSize, Brushes.Black),
                                LanguageHandler.GetInstance().CellInfoWindowTitle
                                );
                        }
                    }
                }
                catch { }
            };
            universeOutputUIElement.ImageUniverseField.MouseDown += mouseDownOnFieldImage;

            universeOutputUIElement.OnStart = delegate
              {
                  StartWork();
              };
            universeOutputUIElement.OnPause = delegate
              {
                  PauseWorkAsync();
              };
            universeOutputUIElement.OnExit = delegate
              {
                  Dispose();
              };
            universeOutputUIElement.OnOpenUniverseConstsRedactor = delegate
              {
                  OpenUniverseConstsRedactor();
              };
            universeOutputUIElement.OnOpenFoodPlaceRedactor = delegate
              {
                  OpenFoodPlaceRedactor(); 
            };
            universeOutputUIElement.OnOpenPoisonPlaceRedactor = delegate
              {
                  OpenPoisonPlaceRedactor(); ;
              };
            universeOutputUIElement.OnClearUniverseField = delegate
              {
                  ClearUniverseField();
              };
            universeOutputUIElement.OnGenerateCells = delegate
              {
                  if (universeOutputUIElement.CountOfCellsToGenerate!=null)
                    GenerateCells((int)universeOutputUIElement.CountOfCellsToGenerate);
              };
            universeOutputUIElement.OnGenerateFoodOnAllField = delegate
              {
                  GenerateFoodOnAllField();
              };
            universeOutputUIElement.OnResetResolution = delegate
              {
                  if (universeOutputUIElement.ResolutionToReset == null)
                      return;
                  Size resolution = (Size)universeOutputUIElement.ResolutionToReset;
                  ResetResolution(resolution.Width, resolution.Height);
              };
            universeOutputUIElement.OnSaveUniverse = delegate
              {
                  SaveUniverse();
              };
            universeOutputUIElement.OnGetWorkDeley = delegate
              {
                  GetWorkDelay();
              };
        }
        void OpenUniverseConstsRedactor()
        {
            InvokeByPause(() =>
            {
                WindowValuesRedactor.ShowModal(UniverseProperty.ConstsUniverseProperty);
            });
        }
        void OpenFoodPlaceRedactor()
        {
            InvokeByPause(() =>
            {
                UniverseProperty.SetAllPlaceOfFood(
                WindowCheckAtField.ShowModal(
                    RenderManagerMainFieldProperty,
                    RenderManagerMainFieldProperty.BtmDefFood,
                    UniverseProperty.GetAllPlaceOfFood(),
                    LanguageHandler.GetInstance().ModalWindowAboutFoodPlace
                    )
                );
            });
        }
        void OpenPoisonPlaceRedactor()
        {
            InvokeByPause(() =>
            {
                UniverseProperty.SetAllPlaceOfPoison(
                WindowCheckAtField.ShowModal(
                    RenderManagerMainFieldProperty,
                    RenderManagerMainFieldProperty.BtmPoison,
                    UniverseProperty.GetAllPlaceOfPoison(),
                    LanguageHandler.GetInstance().ModalWindowAboutPoisonPlace
                    )
                );
            });
        }
        void ClearUniverseField()
        {
            InvokeAsynch(() =>
            {
                UniverseProperty.ClearField(false);
            });
        }
        void GenerateCells(int count)
        {
            InvokeAsynch(() =>
            {
                UniverseProperty.GenerateCells(count, false);
            });
        }
        void GenerateFoodOnAllField()
        {
            InvokeAsynch(() =>
            {
                UniverseProperty.GenerateFoodOnAllField(false);
            });
        }
        void ResetResolution(double pixelsWidth, double pixelsHeight)
        {
            if (pixelsWidth < 100 || pixelsWidth >2000 || pixelsHeight < 100 || pixelsHeight > 2000)
            {
                MessageBox.Show(LanguageHandler.GetInstance().ResolutionWarningMessage);
                return;
            }
            InvokeByPause(() =>
            {
                renderManagerMainField = new RenderManagerMainField(UniverseProperty.Width, UniverseProperty.Height, pixelsWidth, pixelsHeight);
            });
        }
        void SaveUniverse()
        {
            InvokeByPause(() =>
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName ="tss_universe.tssun";
                dlg.Filter = "TSS Universe (.tssun)|*.tssun";
                bool? dialogRes= dlg.ShowDialog();
                if (dialogRes==true)
                {
                    File.WriteAllText(dlg.FileName, SerializeHandler.ToBase64String(UniverseProperty));
                }      
            });
        }
        void GetWorkDelay()
        {
            if (universeOutputUIElement.WorkDelay != null)
                PauseBetweenRender = (int)universeOutputUIElement.WorkDelay;

        }
    }
}
