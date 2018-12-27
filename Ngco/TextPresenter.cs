using SkiaSharp;
using System;

namespace Ngco
{
    public class TextPresenter
    {
        public const int CaretFrameInterval = 15;

        private readonly BaseWidget Owner;

        public int SelectionStart  { get; set; }
        public int SelectionEnd    { get; set; }
        public int CaretPosition   { get; set; }
        public int CaretFrameCount { get; set; }

        public bool RenderCaret { get; set; }
        public bool ShowCaret   { get; set; }

        public SKPaint SelectionBackground { get; set; }
        public SKPaint SelectionForeground { get; set; }
        public SKPaint Foreground          { get; set; }
        public SKPaint Background          { get; set; }

        public TextPresenter(BaseWidget owner)
        {
            Owner = owner;

            SelectionBackground = new SKPaint
            {
                Color       = Color.Blue,
                IsAntialias = true,
            };

            SelectionForeground = new SKPaint
            {
                Color       = Color.White,
                IsAntialias = true,
                TextSize    = owner.Style.TextSize,
                Typeface    = SKTypeface.FromFamilyName(owner.Style.FontFamily ?? "Arial")
            };

            Foreground = new SKPaint
            {
                Color       = owner.Style.TextColor,
                IsAntialias = true,
                TextSize    = owner.Style.TextSize,
                Typeface    = SKTypeface.FromFamilyName(owner.Style.FontFamily ?? "Arial")
            };

            ShowCaret = false;
        }

        public TextPosition GetRealPosition(string text, int position,SKPaint paint)
        {
            TextPosition textPosition = new TextPosition();

            position = Math.Min(text.Length, position);

            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int trimmedPosition       = position;
            int lineNumber            = 0;
            int checkedLength         = 0;
            int previousCheckedLength = 0;

            foreach(var line in lines)
            {
                previousCheckedLength = checkedLength;
                checkedLength += line.Length;

                if(trimmedPosition <= checkedLength)
                {
                    textPosition.Line   = lineNumber;
                    textPosition.Column = trimmedPosition - previousCheckedLength;

                    string prePositionText = line.Substring(0, textPosition.Column);

                    textPosition.XOffset = paint.MeasureText(prePositionText);
                    textPosition.YOffset = lineNumber * paint.TextSize;

                    break;
                }
                else
                {
                    trimmedPosition--;
                }

                lineNumber++;
            }

            return textPosition;
        }

        public int GetTextPosition(string[] textLines, float x, float y, SKPaint paint)
        {
            int characterPostion = 0;

            string line = string.Empty;

            for (int i = 0; i < textLines.Length; i++)
            {
                line = textLines[i];

                float yOffset = paint.TextSize * (i + 1);

                if (y > yOffset)
                {
                    characterPostion += line.Length;
                }
                else
                {
                    characterPostion += (int)paint.BreakText(line, x);
                    break;
                }

                characterPostion++;
            }

            return characterPostion;
        }
    }
}
