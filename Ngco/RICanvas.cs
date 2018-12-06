using SkiaSharp;

namespace Ngco
{
    public class RICanvas
    {
        readonly SKCanvas Canvas;
        readonly float    Scale;
        
        public RICanvas(SKCanvas canvas, float scale)
        {
            Canvas = canvas;
            Scale  = scale;
        }

        SKPaint ScalePaint(SKPaint paint)
        {
            paint              = paint.Clone();
            paint.TextSize    *= Scale;
            paint.StrokeWidth *= Scale;

            return paint;
        }

        public void Clear()              => Canvas.Clear();
        public void Clear(SKColor color) => Canvas.Clear(color);

        public void Save()    => Canvas.Save();
        public void Restore() => Canvas.Restore();

        public void ClipRect(SKRect rect) =>
            Canvas.ClipRect(new SKRect(
                rect.Left  * Scale, rect.Top    * Scale, 
                rect.Right * Scale, rect.Bottom * Scale
            ));

        public void DrawImage(SKBitmap bitmap, Point point, SKPaint paint = null)
        {
            SKBitmap scaled = bitmap.Resize(new SKImageInfo(bitmap.Width * (int)Scale, bitmap.Height * (int)Scale), SKBitmapResizeMethod.Lanczos3);

            Canvas.DrawBitmap(scaled, 
                              point.X * Scale, 
                              point.Y * Scale, 
                              ScalePaint(paint));
        }

        public void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint) =>
            Canvas.DrawRect(x0 * Scale, y0 * Scale, x1 * Scale, y1 * Scale, ScalePaint(paint));

        public void DrawRect(Point position, Point size, SKPaint paint, Point? round = null)
        {
            round = round ?? new Point();

            Canvas.DrawRoundRect(position.X    * Scale, position.Y    * Scale, 
                                 size.X        * Scale,  size.Y       * Scale, 
                                 round.Value.X * Scale, round.Value.Y * Scale,
                                 ScalePaint(paint));
        }

        public void DrawText(string text, float x, float y, SKPaint paint, bool multiline)
        {
            string[] lines = multiline ? text.Split("\n") : new string[] { text };

            for (int i = 0; i < lines.Length; i++)
            {
                Canvas.DrawText(lines[i], 
                                x                          * Scale, 
                                (y + (i * paint.TextSize)) * Scale, 
                                ScalePaint(paint));
            }
        }
    }
}