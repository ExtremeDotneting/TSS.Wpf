using System;
using System.Windows.Media.Imaging;

namespace TSS.Wpf
{

    /// <summary>
    /// Using this class is convenient to follow the dynamic resource of application.
    /// <para></para>
    /// Используя этот класс удобно следить за динамическими ресурсами приложения.
    /// </summary>
    class DynamicResources
    {
        static DynamicResources instance;
        public static DynamicResources GetInstance()
        {
            if (instance == null)
            {
                instance = new DynamicResources();
            }
            return instance;
        }
        DynamicResources()
        {
        }
        public BitmapImage ImageSource_DefFood
        {
            get
            {
                return GraphicsHelper.LoadImage(Environment.CurrentDirectory + "/img_food.png");
            }
        }
        public BitmapImage ImageSource_DeadCell
        {
            get
            {
                return GraphicsHelper.LoadImage(Environment.CurrentDirectory + "/img_deadcell.png");
            }
        }
        public BitmapImage ImageSource_Poison
        {
            get
            {
                return GraphicsHelper.LoadImage(Environment.CurrentDirectory + "/img_poison.png");
            }
        }
        public BitmapImage ImageSource_Border
        {
            get
            {
                return GraphicsHelper.LoadImage(Environment.CurrentDirectory + "/img_border.png");
            }
        }
        public BitmapImage ImageSource_Logo
        {
            get
            {
                return GraphicsHelper.LoadImage(Environment.CurrentDirectory + "/logo.png");
            }
        }
        

    }
}
