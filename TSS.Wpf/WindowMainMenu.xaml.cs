using System;
using System.Windows;
using System.Windows.Controls;
using TSS.Helpers;
using TSS.UniverseLogic;
using System.IO;
using TSS.Another;

namespace TSS.Wpf
{
    public partial class WindowMainMenu : Window
    {
        int defPixelsWidth=1000, defPixelsHeight=1000;

        public WindowMainMenu()
        {
            InitializeComponent();
            imageLogo.Source = DynamicResources.GetInstance().ImageSource_Logo;
        }
        void InitTextOfControls()
        {
            if(!IsInitialized)
                    return;
            Title = LanguageHandler.GetInstance().MainWindowTitle;
            buttonCreatUniverse.Content = LanguageHandler.GetInstance().ButtonCreateUniverse;
            buttonLoadUniverse.Content = LanguageHandler.GetInstance().ButtonLoadUniverse;
            buttonAbout.Content = LanguageHandler.GetInstance().ButtonAbout;
        }

        private void buttonCreatUniverse_Click(object sender, RoutedEventArgs e)
        {
            Tuple<int,int> unSize=WindowUniverseSizeGetter.ShowModal();
            if (unSize != null)
            {
                WindowUniverseOutput.ShowModal(
                    new Universe(unSize.Item1, unSize.Item2),
                    new Size(defPixelsWidth, defPixelsHeight)
                    );
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            switch(Convert.ToString((sender as ComboBoxItem).Content))
            {
                case "en":
                    LanguageHandler.SetLanguage(LanguageHandlerCulture.en);
                    break;
                case "ru":
                    LanguageHandler.SetLanguage(LanguageHandlerCulture.ru);
                    break;
            }
            InitTextOfControls();
        }
        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(LanguageHandler.GetInstance().AboutText);
        }
        private void buttonLoadUniverse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".tssun";
            dlg.Filter = "TSS Universe (.tssun)|*.tssun";
            bool? dialogRes = dlg.ShowDialog();
            if (dialogRes == true)
            {
                string filename = dlg.FileName;
                Universe universe=null;
                try
                {
                    universe=SerializeHandler.FromBase64String<Universe>(File.ReadAllText(filename));
                }
                catch
                {
                    MessageBox.Show(LanguageHandler.GetInstance().UniverseFileCorrupted);
                    return;
                }
                if (universe != null)
                {
              
                    WindowUniverseOutput.ShowModal(
                        universe,
                        new Size(defPixelsWidth, defPixelsHeight)
                    );
                }
            }
            
        }
    }
}
