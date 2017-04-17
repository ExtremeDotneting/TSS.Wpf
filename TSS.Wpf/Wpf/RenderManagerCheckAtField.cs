using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// It used to draw pictures in the editor of food and poison. To work requires basic RenderManagerMainField.
    /// <para></para>
    /// Используется для отрисовке картинки в редакторе еды и яда. Для работы требует основной RenderManagerMainField.
    /// </summary>
    class RenderManagerCheckAtField
    {
        bool[,] fieldDesciptionPrev;
        bool[,] fieldDesciption;
        ImageSource imageSourceForCheck;
        RenderManagerMainField renderManager;
        int width, height;
        RenderTargetBitmap prevRenderedField;
        bool haveChanges = true;
        public bool HaveChanges
        {
            get { return haveChanges; }
            private set { haveChanges = value; }
        }
        public RenderManagerCheckAtField(RenderManagerMainField renderManager, ImageSource imageSourceForCheck, bool[,] fieldDesciption)
        {
            width = fieldDesciption.GetLength(0);
            height = fieldDesciption.GetLength(1);
            fieldDesciptionPrev = new bool[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    fieldDesciptionPrev[i, j] = false;
                }
            }
            this.fieldDesciption = fieldDesciption.Clone() as bool[,];
            this.imageSourceForCheck = imageSourceForCheck;
            imageSourceForCheck.Freeze();
            this.renderManager = renderManager;
            prevRenderedField = renderManager.RenderEmptyField();
            prevRenderedField.Freeze();
        }
        
        /// <summary>
        /// The algorithm is constructed in such a way that it only draws changes. But sometimes, for reliability, the picture is completely redrawn.
        /// <para></para>
        /// Алгоритм построен таким образом, что он только дорисовывает изменения. Но иногда, для надежности, картинка полностью перерисовывается.
        /// </summary>
        public RenderTargetBitmap Render(bool fullRenderFromEmpty)
        {
            if (!haveChanges)
                return prevRenderedField;
            haveChanges = false;
            DrawingVisual fieldDrawing = new DrawingVisual();
            DrawingContext dc = fieldDrawing.RenderOpen();
            dc.DrawImage(
                prevRenderedField,
                new Rect(new Size(prevRenderedField.Width, prevRenderedField.Height))
                );
      
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (fieldDesciption[i, j] != fieldDesciptionPrev[i, j] || fullRenderFromEmpty)
                    {
                        fieldDesciptionPrev[i, j] = fieldDesciption[i, j];
                        Point point = renderManager.GetPixelsPosition(i, j);
                        double xPixel = point.X;
                        double yPixel = point.Y;
                        if (fieldDesciption[i, j])
                            renderManager.DrawAtFieldImageInCube(dc, xPixel, yPixel, imageSourceForCheck);
                        else
                            renderManager.DrawAtFieldClearCube(dc, xPixel, yPixel);
                    }
                }
            }
            dc.Close();
            RenderTargetBitmap res = GraphicsHelper.DrawingVisualRender(fieldDrawing);
            //fieldDesciptionPrev = fieldDesciption.Clone() as bool[,];
            prevRenderedField = res;
            return res;
        }
        public void SetValueAt(int xCube, int yCube, bool value)
        {
            fieldDesciption[xCube, yCube] = value;
            haveChanges = true;
        }
        public void SetValueAt(double xPixel, double yPixel, double stretchedWidth, double stretchedHeight, bool value)
        {
            PointInt positionCube = renderManager.GetCubePosition(xPixel, yPixel, stretchedWidth, stretchedHeight);
            if (!(positionCube.X >= 0 && positionCube.X < width && positionCube.Y >= 0 && positionCube.Y < height))
                return;
            SetValueAt(positionCube.X, positionCube.Y, value);
        }
        public void CheckAll()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    SetValueAt(i, j, true);
                }
            }
        }
        public void UncheckAll()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    SetValueAt(i, j, false);
                }
            }
        }
        public bool[,] GetFieldDescription()
        {
            return fieldDesciption;
        }


    }
}
