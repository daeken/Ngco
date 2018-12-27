using SkiaSharp;
using System;

namespace Ngco
{
    public class TextPresenter
    {
        public const int CaretFrameInterval = 15;

        private int selectionStart;
        private int selectionEnd;
        private int caretPosition;
        private int caretFrameCount;

        private bool renderCaret;
        private bool showCaret;

        private SKPaint selectionBackground;
        private SKPaint selectionForeground;
        private SKPaint foreground;
        private SKPaint background;

        private BaseWidget Owner;

        public int SelectionStart  { get => selectionStart;  set => selectionStart = value; }
        public int SelectionEnd    { get => selectionEnd;    set => selectionEnd = value; }
        public int CaretPosition   { get => caretPosition;   set => caretPosition = value; }
        public int CaretFrameCount { get => caretFrameCount; set => caretFrameCount = value; }

        public bool RenderCaret { get => renderCaret; set => renderCaret = value; }
        public bool ShowCaret   { get => showCaret;   set => showCaret = value; }

        public SKPaint SelectionBackground { get => selectionBackground; set => selectionBackground = value; }
        public SKPaint SelectionForeground { get => selectionForeground; set => selectionForeground = value; }
        public SKPaint Foreground          { get => foreground;          set => foreground = value; }
        public SKPaint Background          { get => background;          set => background = value; }

        public TextPresenter(BaseWidget owner)
        {
            Owner = owner;

            selectionBackground = new SKPaint
            {
                Color       = Color.Blue,
                IsAntialias = true,
            };

            selectionForeground = new SKPaint
            {
                Color       = Color.White,
                IsAntialias = true,
                TextSize    = owner.Style.TextSize,
                Typeface    = SKTypeface.FromFamilyName(owner.Style.FontFamily ?? "Arial")
            };

            foreground = new SKPaint
            {
                Color       = owner.Style.TextColor,
                IsAntialias = true,
                TextSize    = owner.Style.TextSize,
                Typeface    = SKTypeface.FromFamilyName(owner.Style.FontFamily ?? "Arial")
            };

            showCaret = false;
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
