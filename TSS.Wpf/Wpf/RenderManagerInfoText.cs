using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using TSS.UniverseLogic;
using System.Globalization;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// Draws the image with the information of the cell, the universe.
    /// <para></para>
    /// Отрисовывает картинку с информацией о клетке, вселенной.
    /// </summary>
    class RenderManagerInfoText
    {
        Typeface typeface;

        public RenderManagerInfoText(FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch)
        {
            Typeface tf = new Typeface(fontFamily, fontStyle,fontWeight, fontStretch);
            Initialize(tf);
        }
        public RenderManagerInfoText()
        {
            Typeface tf = new Typeface(
                new FontFamily("Courier New"),
                FontStyles.Normal,
                FontWeights.Bold, 
                new FontStretch()
                );
            Initialize(tf);
        }
        public RenderManagerInfoText(Typeface typeface)
        {
            Initialize(typeface);
        }
        public RenderTargetBitmap DrawUniverseInfo(Universe universe, double maxTextWidth, double size, Brush textBrush)
        {
            int cellsCount = universe.GetCellsCount();
            long totalEnergy = universe.GetTotalUniverseEnergy();
            long tick = universe.GetTicksCount();
            Cell cell = universe.GetMostFitCell();
            int w = universe.Width;
            int h = universe.Height;
            int typesOfCellCount = universe.TypesOfCellsCount;

            string unInfoStr = string.Format(
                LanguageHandler.GetInstance().UniverseInfoStringFormatter,
                w, h, cellsCount, totalEnergy, tick, typesOfCellCount
                );

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();
            FormattedText ft = new FormattedText(
                unInfoStr,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                size,
                textBrush
                );
            ft.MaxTextWidth = maxTextWidth;
            dc.DrawText(ft, new Point(0, 0));
            ImageSource dvCellInfo = DrawCellInfo(cell, maxTextWidth, size, textBrush);
            dc.DrawImage(dvCellInfo, new Rect(
                new Point(size, ft.Height),
                new Size(dvCellInfo.Width, dvCellInfo.Height)
                ));

            dc.Close();
            return GraphicsHelper.DrawingVisualRender(drawingVisual);
        }
        public RenderTargetBitmap DrawCellInfo(Cell cell, double maxTextWidth, double size, Brush textBrush)
        {
            int desc, hunger, aggression, reproduction, 
                friendly, poisonAddiction, corpseAddiction, cellsCount;
            if (cell?.GetGenome() == null)
            {
                desc = 0;
                hunger = 0;
                aggression = 0;
                reproduction = 0;
                friendly = 0;
                poisonAddiction = 0;
                corpseAddiction =0;
                cellsCount = -1;
            }
            else
            {
                desc = cell.GetDescriptor();
                hunger = cell.GetGenome().GetHunger();
                aggression = cell.GetGenome().GetAggression();
                reproduction = cell.GetGenome().GetReproduction();
                friendly = cell.GetGenome().GetFriendly();
                poisonAddiction = cell.GetGenome().GetPoisonAddiction();
                corpseAddiction = cell.GetGenome().GetCorpseAddiction();
                cellsCount = cell?.GetCellsCountWithThisDescriptor() ?? -1; //-V3022
            }
            
            string cellInfoStr = string.Format(
                LanguageHandler.GetInstance().CellInfoStringFormatter,
                hunger, aggression, reproduction, friendly, poisonAddiction, corpseAddiction, cellsCount
                );

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();
            FormattedText ft = new FormattedText(
                cellInfoStr,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                size,
                textBrush
                );
            ft.MaxTextWidth = maxTextWidth;
            dc.DrawText(ft, new Point(0, 0));
            dc.DrawRectangle(
                GraphicsHelper.BrushByDescriptor(desc),
                null,
                new Rect(new Point(size*0.2, ft.Height), new Size(ft.Width - size*2+1,size))
                );
            dc.Close();
            return GraphicsHelper.DrawingVisualRender(drawingVisual);
        }
        void Initialize(Typeface typeface)
        {
            this.typeface = typeface;
        }
    }
}
