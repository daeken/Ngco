using System;
using SkiaSharp;

namespace Ngco.Widgets {
	public class Label : BaseWidget {
		public string Text;
		
		SKPaint Paint => new SKPaint {
			Color = Style.TextColor,
            IsAntialias = true,
            TextSize = Style.TextSize.Value, 
			Typeface = SKTypeface.FromFamilyName(Style.FontFamily ?? "Arial")
		};

		public Label(string text = "Label") =>
			Text = text;

		public override Rect CalculateBoundingBox(Rect region) {
			var bb = new Rect(region.TopLeft, new Size((int) Math.Ceiling(Paint.MeasureText(Text)), Style.TextSize.Value));
			return BoundingBox = bb.ClipTo(region);
		}

		public override void Render(RICanvas canvas) {
			var paint = Paint;
			canvas.Save();
			canvas.ClipRect(BoundingBox.Inset(BoundingBox.Size * -0.1f));
			canvas.DrawText(Text, BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y - (paint.FontSpacing - Style.TextSize.Value) + Style.TextSize.Value, paint);
			canvas.Restore();
		}
	}
}