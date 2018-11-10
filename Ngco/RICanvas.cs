using SkiaSharp;

namespace Ngco {
	public class RICanvas {
		readonly SKCanvas Canvas;
		readonly float Scale;
		
		public RICanvas(SKCanvas canvas, float scale) {
			Canvas = canvas;
			Scale = scale;
		}

		SKPaint ScalePaint(SKPaint paint) {
			paint = paint.Clone();
			paint.TextSize *= Scale;
			paint.StrokeWidth *= Scale;
			return paint;
		}

		public void Clear() => Canvas.Clear();
		public void Clear(SKColor color) => Canvas.Clear(color);

		public void Save() => Canvas.Save();
		public void Restore() => Canvas.Restore();

		public void ClipRect(SKRect rect) =>
			Canvas.ClipRect(new SKRect(
				rect.Left * Scale, rect.Top * Scale, 
				rect.Right * Scale, rect.Bottom * Scale
			));

		public void DrawRect(float left, float top, float w, float h, SKPaint paint) =>
			Canvas.DrawRect(left * Scale, top * Scale, w * Scale, h * Scale, ScalePaint(paint));

		public void DrawText(string text, float x, float y, SKPaint paint) =>
			Canvas.DrawText(text, x * Scale, y * Scale, ScalePaint(paint));
	}
}