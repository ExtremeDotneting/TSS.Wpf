using System.Windows;
using System.Windows.Media;

namespace TSS.Wpf
{
    /// <summary>
    /// Window to display the transmitted images.
    /// <para></para>
    /// Окно для отображения передаваемой картинки.
    /// </summary>
    public partial class WindowImage : Window
    {
        WindowImage()
        {
            InitializeComponent();
        }

        public static void ShowModal(ImageSource imageSource,string title)
        {
            WindowImage window = new WindowImage();
            window.Title = title;
            window.image.Dispatcher.Invoke(() =>
            {
                window.image.Source = imageSource;
                window.image.Height = window.image.Source.Height;
                window.image.Width = window.image.Source.Width;
            });
            window.ShowDialog();
            window.Close();
        }
    }
}
