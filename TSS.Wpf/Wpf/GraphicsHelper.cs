using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace TSS.Wpf
{
    /// <summary>
    /// This class is used for simplified graphics work.
    /// <para></para>
    /// Этот класс используется для упрощенной работы с графикой.
    /// </summary>
    static class GraphicsHelper
    {
        public static BitmapImage LoadImage(string path)
        {
            Uri imageAbsolutePath = new Uri(path);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = imageAbsolutePath;
            image.EndInit();
            return image;
        }
        public static void SaveImage(string path, BitmapSource image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var filestream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(filestream);
                filestream.Close();
            }
        }

        /// <summary>
        /// This method is good to optimize, because he spends a lot of memory.
        /// <para></para>
        /// Этот метод неплохо бы оптимизировать, ведь он тратит много памяти.
        /// </summary>
        public static BitmapImage BitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            //string path = Environment.CurrentDirectory + "/bufimg.png";
            //SaveImage(path, bitmapImage);
            //bitmapImage = LoadImage(path);
            //File.Delete(path);
            return bitmapImage;
        }

        /// <summary>
        /// This method is good to optimize, because he spends a lot of memory.
        /// <para></para>
        /// Этот метод неплохо бы оптимизировать, ведь он тратит много памяти.
        /// </summary>
        public static BitmapImage ResizeImage(ImageSource btmBorder, int width, int height)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(btmBorder, new Rect(new Point(0, 0), new Size(width, height)));
            drawingContext.Close();
            if (width < 2)
                width = 2;
            if (height < 2)
                height = 2;
            return BitmapSourceToBitmapImage(
                DrawingVisualRender(drawingVisual, width, height)
                );
        }

        /// <summary>
        /// It is render method.
        /// <para></para>
        /// Этот метод рендерит графику.
        /// </summary>
        public static RenderTargetBitmap DrawingVisualRender(DrawingVisual drawingVisual, int btmWidth, int btmHeight)
        {
            RenderTargetBitmap mergedImage = new RenderTargetBitmap(btmWidth, btmHeight, 96, 96, PixelFormats.Pbgra32);
            mergedImage.Render(drawingVisual);
            return mergedImage;
        }
        public static RenderTargetBitmap DrawingVisualRender(DrawingVisual drawingVisual)
        {
            RenderTargetBitmap mergedImage = new RenderTargetBitmap(
                (int)drawingVisual.ContentBounds.Width, (int)drawingVisual.ContentBounds.Height, 
                96, 96, PixelFormats.Pbgra32
                );
            mergedImage.Render(drawingVisual);
            return mergedImage;
        }
        public static SolidColorBrush BrushByDescriptor(int descriptor)
        {
            byte[] bytes = BitConverter.GetBytes(descriptor);
            Color color = Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
            return new SolidColorBrush(color);
        }
    }
}
