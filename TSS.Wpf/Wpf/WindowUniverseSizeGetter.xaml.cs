using System;
using System.Windows;
using System.Windows.Input;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// The dialog box for the size of the universe.
    /// <para></para>
    /// Диалоговое окно для получения размера вселенной.
    /// </summary>
    public partial class WindowUniverseSizeGetter : Window
    {
        Tuple<int, int> size;
        public static Tuple<int, int> ShowModal()
        {
            var w = new WindowUniverseSizeGetter();
            w.ShowDialog();
            return w.size;
        }
        WindowUniverseSizeGetter()
        {
            InitializeComponent();
            labelWidth.Content = LanguageHandler.GetInstance().LabelWidthText;
            labelHeight.Content = LanguageHandler.GetInstance().LabelHeightText;
            buttonOk.Content = LanguageHandler.GetInstance().ButtonOk;
            buttonCancel.Content = LanguageHandler.GetInstance().ButtonCancel;
        }
        void TryToConfirm()
        {
            try
            {
                int w, h;
                w = Convert.ToInt32(textBoxWidth.Text);
                h = Convert.ToInt32(textBoxHeight.Text);
                if (w < 2 || w > 1000 || h < 2 || h > 1000)
                    throw new Exception();
                size = new Tuple<int, int>(w, h);
                Close();
            }
            catch
            {
                MessageBox.Show(LanguageHandler.GetInstance().UniverseSizeWarning);
            }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            TryToConfirm();
        }
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
                TryToConfirm();
        }
    }
}
