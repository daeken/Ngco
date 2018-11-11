using System;
using System.Linq;
using SkiaSharp;

namespace Ngco.Widgets {
	public class Label : BaseWidget {
		public string Text;
		
		SKPaint Paint => new SKPaint {
			Color = Style.TextColor,
			IsAntialias = true,
			TextSize = Style.TextSize, 
			Typeface = SKTypeface.FromFamilyName(Style.FontFamily ?? "Arial")
		};

		public Label(string text = "Label") =>
			Text = text;

        public override void Measure(Size region)
        {
            var lines = Style.Multiline ? Text.Split("\\n") : new string[] { Text };
            BoundingBox = new Rect(new Point(), new Size((int)Math.Ceiling(Paint.MeasureText(lines.OrderByDescending(s => s.Length).First())),
                Style.TextSize * lines.Length));
            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas) {
			var paint = Paint;
			canvas.Save();
			canvas.ClipRect(BoundingBox.Inset(BoundingBox.Size * -0.1f));
			canvas.DrawText(Text, BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y - (paint.FontSpacing - Style.TextSize) + Style.TextSize, paint, Style.Multiline);
			canvas.Restore();
		}

        public override void Layout(Rect region){ }
    }
}