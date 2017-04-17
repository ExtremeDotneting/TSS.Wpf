using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// An important class. Draws a simulation.
    /// <para></para>
    /// Важный класс. Отрисовывает симуляцию.
    /// </summary>
    class RenderManagerMainField
    {
        Brush brushDefFood=Brushes.Green, 
            brushDeadCell=Brushes.Red, 
            brushPoison=Brushes.Orange,
            gridBrush = Brushes.DarkBlue;
        Brush emptyPlaceBrush = Brushes.WhiteSmoke;
        Brush imageBrush = Brushes.LightBlue;

        /// <summary>
        /// The buffer with the previous frame. It takes a lot of space, you can try to optimize it.
        /// <para></para>
        /// Буфер с предыдущим кадром. Занимает много места, возможно стоит оптимизировать.
        /// </summary>
        RenderTargetBitmap btmBackground;
        BitmapImage btmDefFood, btmDeadCell, btmPoison, btmBorder;

        // List of constants used for rendering. Calculated in the constructor method CalcFieldConsts.
        // Affect the size of objects, the grid boundaries and so forth.
        //
        // Список констант, используемых для отрисовки. Высчитываются в конструкторе, методом CalcFiеldConsts.
        // Влияют на размер объектов, сетки, границ и прочее.
        Rect fieldRect;
        int minLineSize = 3;
        int widthCount, heightCount, thickness;
        double cubeLineSize, halfCubeLineSize, widthPixels, heightPixels;
        Size cubeSize;
        double widthPixelsTrue, heightPixelsTrue;
        int[,] descriptorsPrev;
        double cubeLineSize_Optimized, halfCubeLineSize_Optimized, 
            cubeLineSize_Optimized_PercentForIndent, cubeLineSize_Optimized_PercentForIndentOfEmptySpace;
        Size cubeSize_Optimized, emptySpaceSize;
        // List of constants used for rendering...
        //
        // Список констант, используемых для отрисовки...

        public BitmapImage BtmDefFood
        {
            get { return btmDefFood; }
        }
        public BitmapImage BtmDeadCell
        {
            get { return btmDeadCell; }
        }
        public BitmapImage BtmPoison
        {
            get { return btmPoison; }
        }
        public BitmapImage BtmBorder
        {
            get { return btmBorder; }
        }

        public RenderManagerMainField(int wCount, int hCount, double wPixels, double hPixels)
        {
            descriptorsPrev = new int[wCount, hCount];
            widthCount = wCount;
            heightCount = hCount;
            widthPixels = wPixels;
            heightPixels = hPixels;
            CalcFildConsts();

            btmDefFood = DynamicResources.GetInstance().ImageSource_DefFood;
            btmDeadCell = DynamicResources.GetInstance().ImageSource_DeadCell;
            btmPoison = DynamicResources.GetInstance().ImageSource_Poison;
            btmBorder = DynamicResources.GetInstance().ImageSource_Border;
            btmDefFood = GraphicsHelper.ResizeImage(btmDefFood, (int)cubeLineSize_Optimized, (int)cubeLineSize_Optimized);
            btmDeadCell = GraphicsHelper.ResizeImage(btmDeadCell, (int)cubeLineSize_Optimized, (int)cubeLineSize_Optimized);
            btmPoison = GraphicsHelper.ResizeImage(btmPoison, (int)cubeLineSize_Optimized, (int)cubeLineSize_Optimized);
            btmBorder = GraphicsHelper.ResizeImage(btmBorder, (int)cubeLineSize_Optimized, (int)cubeLineSize_Optimized);
            btmBackground = RenderEmptyField();

            btmDefFood.Freeze();
            btmDeadCell.Freeze();
            btmPoison.Freeze();
            btmBorder.Freeze();
            btmBackground.Freeze();
        }

        /// <summary>
        /// It draws field by descriptors.
        /// <para></para>
        /// Отрисовывает поле по дескрипторам.
        /// </summary>
        public RenderTargetBitmap RenderField(int[,] descriptors)
        {
            btmBackground = GraphicsHelper.DrawingVisualRender(
                DrawField(descriptors), (int)widthPixelsTrue, (int)heightPixelsTrue
                );
            return btmBackground;
        }

        /// <summary>
        /// Draws the backdrop of the field. If the size of one cell is too small, then the circle is drawn with the Brush color (look at class fields).
        /// <para></para>
        /// Отрисовывает задник поля. Если размер одной ячейки слишком мал, то отрисовывается кружочек с цветом Brush (смотрите в полях класса).
        /// </summary>
        public RenderTargetBitmap RenderEmptyField()
        {
            RenderTargetBitmap res= GraphicsHelper.DrawingVisualRender(
                CreateGridDrawing(), (int)widthPixelsTrue, (int)heightPixelsTrue
                );
            return res;
        }
        public PointInt GetCubePosition(double xPixel, double yPixel)
        {
            int xCube = (int)((xPixel - thickness) / (cubeLineSize + thickness) - 1);
            int yCube = (int)((yPixel - thickness) / (cubeLineSize + thickness) - 1);
            return new PointInt(xCube, yCube);
        }
        public PointInt GetCubePosition(double xPixel, double yPixel, double stretchedWidth, double stretchedHeight)
        {
            xPixel = xPixel / stretchedWidth * widthPixelsTrue;
            yPixel = yPixel / stretchedHeight * heightPixelsTrue;
            return GetCubePosition(xPixel, yPixel);
        }
        public Point GetPixelsPosition(int xCube, int yCube)
        {
            double x = thickness + (cubeLineSize + thickness) * (xCube + 1),
                y = thickness + (cubeLineSize + thickness) * (yCube + 1);
            return new Point(x, y);
        }
        /*public void DrawAtFieldRect(Brush rectBrush, DrawingContext drawingContext, int xCube, int yCube)
        {
            
            drawingContext.DrawRectangle(
                rectBrush, null,
                new Rect(
                    new Point(x , y ),
                    cubeSize_Optimized
                )
            );
        }*/
        public void DrawAtFieldClearCube(DrawingContext drawingContext, double x, double y)
        {
            drawingContext.DrawRectangle(emptyPlaceBrush, null,
                new Rect(
                    new Point(x + cubeLineSize_Optimized_PercentForIndentOfEmptySpace, y + cubeLineSize_Optimized_PercentForIndentOfEmptySpace),
                    emptySpaceSize
                ));
        }
        public void DrawAtFieldImageInCube(DrawingContext drawingContext, double x, double y, ImageSource imageToDraw)
        {
            if (cubeLineSize_Optimized < minLineSize)
                drawingContext.DrawEllipse(imageBrush, null,
                    new Point(x + halfCubeLineSize, y + halfCubeLineSize),
                    halfCubeLineSize_Optimized, halfCubeLineSize_Optimized
                    );
            else
                drawingContext.DrawImage(
                    imageToDraw, 
                    new Rect(
                        new Point(x + cubeLineSize_Optimized_PercentForIndent, y + cubeLineSize_Optimized_PercentForIndent), cubeSize_Optimized
                        )
                    );
        }
        public void DrawAtFieldFood(DrawingContext drawingContext, double x, double y)
        {
            if (cubeLineSize_Optimized < minLineSize)
                drawingContext.DrawEllipse(brushDefFood, null,
                    new Point(x + halfCubeLineSize, y + halfCubeLineSize),
                    halfCubeLineSize_Optimized, halfCubeLineSize_Optimized
                    );
            else
                drawingContext.DrawImage(btmDefFood, new Rect(
                    new Point(x + cubeLineSize_Optimized_PercentForIndent, y + cubeLineSize_Optimized_PercentForIndent), cubeSize_Optimized));
        }
        public void DrawAtFieldPoison(DrawingContext drawingContext, double x, double y)
        {
            if (cubeLineSize_Optimized < minLineSize)
                drawingContext.DrawEllipse(brushPoison, null,
                    new Point(x + halfCubeLineSize, y + halfCubeLineSize),
                    halfCubeLineSize_Optimized, halfCubeLineSize_Optimized
                    );
            else
                drawingContext.DrawImage(btmPoison, new Rect(
                    new Point(x + cubeLineSize_Optimized_PercentForIndent, y + cubeLineSize_Optimized_PercentForIndent), cubeSize_Optimized));
        }
        public void DrawAtFieldCorpse(DrawingContext drawingContext, double x, double y)
        {
            if (cubeLineSize_Optimized < minLineSize)
                drawingContext.DrawEllipse(brushDeadCell, null,
                    new Point(x + halfCubeLineSize, y + halfCubeLineSize),
                    halfCubeLineSize_Optimized, halfCubeLineSize_Optimized
                    );
            else
                drawingContext.DrawImage(btmDeadCell, new Rect(
                    new Point(x + cubeLineSize_Optimized_PercentForIndent, y + cubeLineSize_Optimized_PercentForIndent), cubeSize_Optimized));
        }
        public void DrawAtFieldCell(DrawingContext drawingContext, double x, double y,int descriptor)
        {
            Brush cellBrush = GraphicsHelper.BrushByDescriptor(descriptor);
            //cellBrush.Opacity = 0.9;
            //drawingContext.DrawImage(btmCell, new Rect(new Point(x, y), cubeSize));
            drawingContext.DrawEllipse(cellBrush, null,
                new Point(x + halfCubeLineSize, y + halfCubeLineSize),
                halfCubeLineSize_Optimized, halfCubeLineSize_Optimized
                );
        }
        void DrawAtFieldByDescriptor(int descriptor, DrawingContext drawingContext, double x, double y)
        {
            DrawAtFieldClearCube(drawingContext, x, y);
            if (descriptor == 0)
            {
                
            }
            else if (descriptor < 0)
            {
                if (descriptor == -1)
                {
                    DrawAtFieldFood(drawingContext, x, y);
                }
                else if (descriptor == -2)
                {
                    DrawAtFieldCorpse(drawingContext, x, y);
                }
                else
                {
                    DrawAtFieldPoison(drawingContext, x, y);
                }
            }
            else
            {
                DrawAtFieldCell(drawingContext, x, y, descriptor);
            }
        }
        DrawingVisual CreateGridDrawing()
        {
            int widthCount = this.widthCount;
            int heightCount = this.heightCount;

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Rect rect = new Rect(new Point(0, 0), new Size((int)widthPixelsTrue, (int)heightPixelsTrue));

            if (thickness == 0)
            {
                drawingContext.DrawRectangle(emptyPlaceBrush, null, rect);
            }
            else
            {

                drawingContext.DrawRectangle(gridBrush, null, rect);
                double x = thickness * 2 + cubeLineSize, y = thickness * 2 + cubeLineSize;
                for (int i = 0; i < widthCount; i++)
                {
                    y = thickness * 2 + cubeLineSize;
                    for (int j = 0; j < heightCount; j++)
                    {
                        drawingContext.DrawRectangle(emptyPlaceBrush, null, new Rect(new Point(x, y), cubeSize));
                        y += cubeLineSize + thickness;
                    }
                    x += cubeLineSize + thickness;
                    AsyncCallGC();
                }
                widthCount += 2;
                heightCount += 2;
                y = thickness;
                x = thickness;
                for (int i = 0; i < widthCount; i++)
                {
                    drawingContext.DrawImage(btmBorder, new Rect(new Point(x, y), cubeSize));
                    x += cubeLineSize + thickness;
                }
                y = heightPixelsTrue - thickness - cubeLineSize;
                x = thickness;
                for (int i = 0; i < widthCount; i++)
                {
                    drawingContext.DrawImage(btmBorder, new Rect(new Point(x, y), cubeSize));
                    x += cubeLineSize + thickness;
                }
                y = thickness;
                x = thickness;
                for (int i = 0; i < heightCount; i++)
                {
                    drawingContext.DrawImage(btmBorder, new Rect(new Point(x, y), cubeSize));
                    y += cubeLineSize + thickness;
                }
                y = thickness;
                x = widthPixelsTrue - thickness - cubeLineSize;
                for (int i = 0; i < heightCount; i++)
                {
                    drawingContext.DrawImage(btmBorder, new Rect(new Point(x, y), cubeSize));
                    y += cubeLineSize + thickness;
                }
                AsyncCallGC();
            }

            drawingContext.Close();
            return drawingVisual;
        }
        DrawingVisual DrawField(int[,] descriptors)
        {
            DrawingVisual dvField = new DrawingVisual();
            DrawingContext drawingContext = dvField.RenderOpen();
            drawingContext.DrawImage(btmBackground, fieldRect);
            double x = thickness * 2 + cubeLineSize, y = thickness * 2 + cubeLineSize;
            for (int i = 0; i < widthCount; i++)
            {
                y = thickness * 2 + cubeLineSize;
                for (int j = 0; j < heightCount; j++)
                {
                    if (descriptorsPrev[i, j] != descriptors[i, j])
                        DrawAtFieldByDescriptor(descriptors[i, j], drawingContext, x, y);
                    y += cubeLineSize + thickness;
                    
                }
                x += cubeLineSize + thickness;
                AsyncCallGC();
            }
            //int totalMBytesOfMemoryUsed = (int)GC.GetTotalMemory(false) / 1048576;
            
            drawingContext.Close();
            descriptorsPrev = descriptors;
            return dvField;
        }
        bool isInRunCallGC = false;

        /// <summary>
        /// When rendering the memory is cleared bad, it is necessary to check the memory and call the garbage collector manually.
        /// <para></para>
        /// Память плохо очищается при отрисовке, по-этому приходится проверять память и вызывать сборщик мусора вручную.
        /// </summary>
        void AsyncCallGC() {
            if (!isInRunCallGC)
            {
                isInRunCallGC = true;
                new Thread(() =>
                  {
                      int totalMBytesOfMemoryUsed = (int)Process.GetCurrentProcess().WorkingSet64 / 1048576;
                      if (totalMBytesOfMemoryUsed > 400)
                          GC.Collect();
                      isInRunCallGC = false;

                  }).Start();
            }
        }
        void CalcFildConsts()
        {
            int widthCount = this.widthCount+2;
            int heightCount = this.heightCount+2;
            int count;
            double pixels;
            if (widthPixels / widthCount > heightPixels / heightCount)
            {
                count = heightCount;
                pixels = heightPixels;
            }
            else
            {
                count = widthCount;
                pixels = widthPixels;
            }
            thickness = (int)((pixels / count * 0.05) * (count / (count - 1 / count)));
            cubeLineSize = pixels / count - thickness;
            halfCubeLineSize = cubeLineSize / 2;
            cubeSize = new Size(cubeLineSize, cubeLineSize);
            if (thickness == 0)
            {
                cubeLineSize_Optimized = cubeLineSize;
                halfCubeLineSize_Optimized = cubeLineSize_Optimized / 2;
                cubeSize_Optimized = cubeSize;
                cubeLineSize_Optimized_PercentForIndent = 0;
                if (cubeLineSize_Optimized < 2)
                {
                    cubeLineSize_Optimized_PercentForIndentOfEmptySpace = -(cubeLineSize * 0.3);
                    emptySpaceSize = new Size(cubeLineSize_Optimized * 1.6, cubeLineSize * 1.6);
                }
                else
                {
                    cubeLineSize_Optimized_PercentForIndentOfEmptySpace = -(cubeLineSize * 0.1);
                    emptySpaceSize = new Size(cubeLineSize_Optimized * 1.2, cubeLineSize * 1.2);
                }
            }
            else
            {
                cubeLineSize_Optimized = cubeLineSize * 0.8;
                halfCubeLineSize_Optimized = cubeLineSize_Optimized / 2;
                cubeSize_Optimized = new Size(cubeLineSize_Optimized, cubeLineSize_Optimized);
                cubeLineSize_Optimized_PercentForIndent = cubeLineSize * 0.1;
                cubeLineSize_Optimized_PercentForIndentOfEmptySpace = cubeLineSize * 0.05;
                emptySpaceSize = new Size(cubeLineSize*0.9, cubeLineSize * 0.9);
            }
            widthPixelsTrue = (widthCount) * (cubeLineSize + thickness) + thickness;
            heightPixelsTrue = (heightCount) * (cubeLineSize + thickness) + thickness;

            fieldRect = new Rect(new Point(0, 0), new Size((int)widthPixelsTrue, (int)heightPixelsTrue));
        }
        
    }
}
