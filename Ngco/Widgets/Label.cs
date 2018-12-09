using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

using static Ngco.Helpers;

namespace Ngco.Widgets
{
    public class Label : BaseWidget
    {
        public string Text;

        public override string[] PropertyKeys { get; } = new string[] { "label", "multiline" };

        SKPaint Paint => new SKPaint
        {
            Color       = Style.TextColor,
            IsAntialias = true,
            TextSize    = Style.TextSize,
            Typeface    = SKTypeface.FromFamilyName(Style.FontFamily ?? "Arial")
        };

        private bool _multiline = false;

        public bool Multiline
        {
            get => _multiline;
            set => _multiline = value;
        }

        public Label(string text = "Label") => Text = text;

        public override void Load(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("label", out string imagePath))
            {
                Text = imagePath;
            }

            if (properties.TryGetValue("multiline", out string isMultiline))
            {
                Multiline = ParseBool(isMultiline);
            }
        }

        public override void OnMeasure(Size region)
        {
            string[] lines = Multiline ? Text.Split("\n", StringSplitOptions.RemoveEmptyEntries) : new string[] { Text };

            BoundingBox = new Rect(new Point(), new Size((int)Math.Ceiling(Paint.MeasureText(lines.OrderByDescending(s => s.Length).First())),
                                                         Style.TextSize * lines.Length));

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            var paint = Paint;

            canvas.Save();
            canvas.ClipRect(BoundingBox.Inset(BoundingBox.Size * -0.1f));
            canvas.DrawText(Text, BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y - (paint.FontSpacing - Style.TextSize) + Style.TextSize, paint, Multiline);
            canvas.Restore();
        }

        public override void OnLayout(Rect region) { }
    }
}