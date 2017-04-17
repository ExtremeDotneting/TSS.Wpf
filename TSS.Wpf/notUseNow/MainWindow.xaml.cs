using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Diagnostics;
using TSS.UniverseLogic;

namespace TSS.Wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /*Alternative version to show Universe. Used in tests.*/
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }

        Thread thr;
        Universe un;
        RenderManagerMainField rm ;
        RenderManagerInfoText ritm ;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            int w =200, h =120;
            double wPix=1000, hPix=1000;
            image.StretchDirection = StretchDirection.Both;
            image1.StretchDirection = StretchDirection.Both;
            MouseButtonEventHandler mouseDownOnImage = (s, ev)=>
            {
                if (un != null)
                {
                    //double modificator = 
                    Point pos = ev.GetPosition(s as Image);
                    PointInt posCube = rm.GetCubePosition(pos.X, pos.Y, image.ActualWidth, image.ActualHeight);
                    UniverseObject uo = un.GetMatrixElement(posCube.X, posCube.Y);
                    if(uo is Cell)
                    {
                        WindowImage.ShowModal(
                            ritm.DrawCellInfo(uo as Cell, 400, 20, Brushes.Black),
                            LanguageHandler.GetInstance().CellInfoWindowTitle
                            );
                    }
                }
            };
            image.MouseDown += mouseDownOnImage;
            thr = new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                
                un = new Universe(w, h);
                un.GenerateCells(5);
                rm = new RenderManagerMainField(w, h, wPix, hPix);
                ritm = new RenderManagerInfoText();
                while (thr == Thread.CurrentThread)
                {
                    un.DoUniverseTick();
                    ImageSource img = rm.RenderField(un.GetAllDescriptors());
                    img.Freeze();                  
                    ImageSource info =ritm.DrawUniverseInfo(un,300,20, Brushes.Black);
                    info.Freeze();
                    try
                    {
                        image.Dispatcher.Invoke(() =>
                        {
                            image.Source = img;
                        });
                        image1.Dispatcher.Invoke(() =>
                        {
                            image1.Source = info;
                        });
                    }
                    catch
                    {
                        thr = null;
                    }
                    Thread.Sleep(30);
                    
                }
                image.MouseDown -= mouseDownOnImage; 
            }) ;
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                thr = null;
            };
            thr.Start();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (un == null)
                return;
            WindowValuesRedactor.ShowModal(un.ConstsUniverseProperty);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (un == null)
                return;
            un.SetAllPlaceOfFood(
                WindowCheckAtField.ShowModal(
                    rm,
                    rm.BtmDefFood,
                    un.GetAllPlaceOfFood()
                    )
                );
            un.SetAllPlaceOfPoison(
                WindowCheckAtField.ShowModal(
                    rm,
                    rm.BtmPoison,
                    un.GetAllPlaceOfPoison()
                    )
                );
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (un == null)
                return;
            un.ClearField();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (un == null)
                return;
            un.GenerateCells(1);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            WindowUniverseOutput.ShowModal(
                new Universe(100,50),
                new Size(4000,4000)
                );
        }
    }

    static class ProjectTesting
    {
        public static void StringToLogFile(string str, string name)
        {
            string path = Environment.CurrentDirectory + @"/work_log___" + name + @".txt";
            File.WriteAllLines(path, str.Split('\n'));
            Process proc = new Process();
            proc.StartInfo.FileName = path;
            proc.Start();
            //while (!proc.HasExited)
            //    System.Threading.Thread.Sleep(100);
        }

        //string CoordsToString(UniverseObject[,] uoArr)
        //{
        //    string res = @"";
        //    for (int j = 0; j < uoArr.GetLength(1); j++)  
        //    {
        //        for (int i = 0; i < uoArr.GetLength(0); i++)
        //        {
        //            string buf = @"";
        //            if (uoArr[i, j] is EmptySpace)
        //                buf += @"E,";
        //            else if (uoArr[i, j] is Food)
        //                buf += @"F,";
        //            else if (uoArr[i, j] is Cell)
        //                buf += @"C,";
        //            if(uoArr[i, j].GetDesc().ToString().Length>=4)
        //                buf += uoArr[i, j].GetDesc().ToString().Substring(0,4)+":";
        //            else
        //                buf += uoArr[i, j].GetDesc().ToString().Substring(0) + ":";
        //            buf += uoArr[i, j].GetX().ToString() + @"," + uoArr[i, j].GetY().ToString();


        //            for (int k = 12 - buf.Length; k > 0; k--)
        //                buf += " ";
        //            res += buf;
        //        }
        //        res += "\n\n\n";
        //    }
        //    return res;
        //}
    }
}
