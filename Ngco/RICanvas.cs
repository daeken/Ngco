using SkiaSharp;
using System;

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

        public void DrawCircle(Point center, float radius, SKPaint paint)
        {
            Canvas.DrawCircle(center.X * Scale, center.Y * Scale, radius * Scale, paint);
        }

        public void DrawPath(SKPath path, Point point, SKPaint paint)
        {
            path.Transform(SKMatrix.MakeScale(Scale, Scale));

            Canvas.DrawPath(path, paint);
        }

        public void DrawText(string text, float x, float y, TextPresenter textPresenter, bool multiline)
        {
            bool renderCaret   = textPresenter.ShowCaret && textPresenter.RenderCaret;
            int selectionStart = Math.Min(textPresenter.SelectionStart, textPresenter.SelectionEnd);
            int selectionEnd   = Math.Max(textPresenter.SelectionStart, textPresenter.SelectionEnd);

            var textToRender = multiline ? text : text.Replace("\n", string.Empty);

            var lines = textToRender.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            var caretPosition          = textPresenter.GetRealPosition(textToRender, textPresenter.CaretPosition, textPresenter.Foreground);
            var selectionStartPosition = textPresenter.GetRealPosition(textToRender, selectionStart, textPresenter.Foreground);
            var selectionEndPosition   = textPresenter.GetRealPosition(textToRender, selectionEnd, textPresenter.Foreground);

            SKPaint caretPaint = new SKPaint
            {
                Color = textPresenter.SelectionEnd != textPresenter.SelectionEnd ? Color.Win10GreyDark : Color.Black,
                StrokeWidth = 1
            };

            if (renderCaret)
            {
                Canvas.DrawLine(new SKPoint((caretPosition.XOffset + x) * Scale, (y + 2) * Scale),
                                new SKPoint((caretPosition.XOffset + x) * Scale, (y - textPresenter.Foreground.TextSize - 2) * Scale),
                                ScalePaint(caretPaint));
            }

            SKPaint currentForegroundPaint = textPresenter.Foreground;
            SKPaint currentBackgroundPaint = textPresenter.Background;

            string preSelection  = string.Empty;
            string selection     = string.Empty;
            string postSelection = string.Empty;

            bool inSelection = false;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (selectionStartPosition.Line == i)
                {
                    int selectionLimit = selectionEndPosition.Line == i ? selectionEndPosition.Column : line.Length;

                    selection    = line.Substring(selectionStartPosition.Column, selectionLimit - selectionStartPosition.Column);
                    preSelection = line.Substring(0, selectionStartPosition.Column);

                    if (preSelection.Length > 0)
                    {
                        Canvas.DrawText(preSelection, x * Scale, (y + selectionStartPosition.YOffset) * Scale, ScalePaint(currentForegroundPaint));
                    }

                    currentForegroundPaint = textPresenter.SelectionForeground;
                    currentBackgroundPaint = textPresenter.SelectionBackground;

                    if (selection.Length > 0)
                    {
                        float selectionWidth = currentForegroundPaint.MeasureText(selection);

                        Canvas.DrawRect((x + selectionStartPosition.XOffset) * Scale, (y + selectionStartPosition.YOffset) * Scale,
                            selectionWidth * Scale, -currentForegroundPaint.TextSize * Scale, ScalePaint(currentBackgroundPaint));

                        Canvas.DrawText(selection, (x + selectionStartPosition.XOffset) * Scale,
                            (y + selectionStartPosition.YOffset) * Scale, currentForegroundPaint);
                    }

                    inSelection = true;
                }
                if (selectionEndPosition.Line == i)
                {
                    int selectionLimit = selectionStartPosition.Line == i ? selectionStartPosition.Column : 0;

                    selection     = line.Substring(selectionLimit, selectionEndPosition.Column - selectionLimit);
                    postSelection = line.Substring(selectionEndPosition.Column, line.Length - selectionEndPosition.Column);

                    if (selection.Length > 0 && selectionStartPosition.Line != i)
                    {
                        float selectionWidth = currentForegroundPaint.MeasureText(selection);

                        Canvas.DrawRect((x + selectionEndPosition.XOffset - selectionWidth) * Scale, (y + selectionEndPosition.YOffset) * Scale,
                            selectionWidth * Scale, -currentForegroundPaint.TextSize * Scale, ScalePaint(currentBackgroundPaint));

                        Canvas.DrawText(selection, (x + selectionEndPosition.XOffset - selectionWidth) * Scale,
                            (y + selectionEndPosition.YOffset) * Scale, ScalePaint(currentForegroundPaint));
                    }

                    currentForegroundPaint = textPresenter.Foreground;
                    currentBackgroundPaint = textPresenter.Background;

                    if (postSelection.Length > 0)
                    {
                        Canvas.DrawText(postSelection, (x + selectionEndPosition.XOffset) * Scale,
                            (y + selectionEndPosition.YOffset) * Scale, ScalePaint(currentForegroundPaint));
                    }

                    inSelection = false;
                }
                else
                {
                    if (selectionStartPosition.Line != i && selectionEndPosition.Line != i)
                    {
                        float yOffset    = i * currentForegroundPaint.TextSize;
                        float lineLength = currentForegroundPaint.MeasureText(line);

                        if (inSelection)
                        {
                            Canvas.DrawRect(x * Scale, (y + yOffset) * Scale, lineLength * Scale, -currentForegroundPaint.TextSize * Scale,
                                ScalePaint(currentBackgroundPaint));
                        }

                        Canvas.DrawText(line, x * Scale, (y + yOffset) * Scale, ScalePaint(currentForegroundPaint));
                    }
                }
            }
        }
    }
}