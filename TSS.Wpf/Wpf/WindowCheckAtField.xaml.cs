using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// Form to display the field of editor (food / poison editor). For uses need RenderManagerCheckAtField.
    /// <para></para>
    /// Форма для отображения редактора поля (редактор еды/яда). Для работы использует RenderManagerCheckAtField.
    /// </summary>
    public partial class WindowCheckAtField : Window
    {
        internal static bool[,] ShowModal(RenderManagerMainField renderManager, ImageSource imageSourceForCheck, bool[,] fieldDesciption, string title="Field redactor")
        {
            if (fieldDesciption.Length > 40000)
            {
                MessageBox.Show(LanguageHandler.GetInstance().FieldRedactorSizeWarning);
                return fieldDesciption;
            }
            RenderManagerCheckAtField renderManagerCheckAtField = new RenderManagerCheckAtField(renderManager, imageSourceForCheck, fieldDesciption);
            var wd = new WindowCheckAtField(renderManagerCheckAtField);
            wd.Title = title;
            wd.StartRenderField();
            wd.ShowDialog();
            wd.StopRenderField();
            bool[,] res;
            if (wd.applyed)
            {
                res = renderManagerCheckAtField.GetFieldDescription();
            }
            else
            {
                res = fieldDesciption;                
            }
            return res;
        }

        RenderManagerCheckAtField renderManagerCheckAtField;
        Thread fieldRenderThread;
        bool applyed = false;
        WindowCheckAtField(RenderManagerCheckAtField renderManagerCheckAtField)
        {
            InitializeComponent();
            applyButton.Content = LanguageHandler.GetInstance().ApplyButtonText;
            buttonCheckAll.Content = LanguageHandler.GetInstance().CheckAllButton;
            buttonUncheckAll.Content = LanguageHandler.GetInstance().UncheckAllButton;
            this.renderManagerCheckAtField = renderManagerCheckAtField;
        }
      
        /// <summary>
        /// Runs field rendering thread.
        /// <para></para>
        /// Запускает поток отрисовки поля.
        /// </summary>
        public void StartRenderField()
        {
            if (fieldRenderThread != null)
                return;
            fieldRenderThread= new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                int renderNum = 1;
                while (fieldRenderThread == Thread.CurrentThread)
                {
                    //if (renderManagerCheckAtField.HaveChanges)
                    //{
                    ImageSource img = renderManagerCheckAtField.Render(renderNum++ % 20 == 0);
                    img.Freeze();
                    try
                    {
                        imageField.Dispatcher.Invoke(() =>
                        {
                            imageField.Source = img;
                        });
                    }
                    catch
                    {
                        fieldRenderThread = null;
                    }
                    //}  
                    Thread.Sleep(200);
                }
            });
            fieldRenderThread.Start();
        }
        public void StopRenderField()
        {
            //fieldRenderThread.Abort();
            fieldRenderThread = null;
        }

        private void imageField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            double w= img.ActualWidth, h = img.ActualHeight;
            new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                while (e.MouseDevice.DirectlyOver == sender as Image && e.ButtonState == MouseButtonState.Pressed)
                {
                    bool checkValue;
                    if (e.ChangedButton == MouseButton.Left)
                        checkValue = true;
                    else if (e.ChangedButton == MouseButton.Right)
                        checkValue = false;
                    else
                        return;
                    Point pos=new Point();
                    img.Dispatcher.Invoke(() =>
                    {
                        pos = e.GetPosition(img);
                    });
                    renderManagerCheckAtField.SetValueAt(pos.X, pos.Y, w,h , checkValue);
                    //Thread.Sleep(30);
                }
            }).Start();
        }
        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            applyed = true;
            Close();
        }
        private void buttonCheckAll_Click(object sender, RoutedEventArgs e)
        {
            renderManagerCheckAtField.CheckAll();
        }
        private void buttonUncheckAll_Click(object sender, RoutedEventArgs e)
        {
            renderManagerCheckAtField.UncheckAll();
        }
    }

    
}
