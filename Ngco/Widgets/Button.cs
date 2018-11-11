using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Ngco.Widgets {
	public class Button : BaseWidget {
		BaseWidget _Label;

		public BaseWidget Label {
			get => _Label;
			set => (_Label = value).Parent = this;
		}

		public event Action<Button> Clicked;

		public override bool IsFocusable => true;

		public Button(BaseWidget label = null) =>
			Label = label;

		public override IEnumerator<BaseWidget> GetEnumerator() =>
			new List<BaseWidget> { Label }.GetEnumerator();

		public override Rect CalculateBoundingBox(Rect region) {
			var labelBb = Label.CalculateBoundingBox(region.Inset(new Size(10, 10)));
			var bb = new Rect(region.TopLeft, labelBb.Size + new Size(20, 20));

			return BoundingBox = bb.ClipTo(region);
		}

		public override void Render(RICanvas canvas) {
			canvas.Save();
			canvas.ClipRect(BoundingBox);

			var left = BoundingBox.TopLeft.X;
			var right = BoundingBox.TopLeft.Y;
			var width = BoundingBox.Size.Width;
			var height = BoundingBox.Size.Height;
			var radiusX = Style.CornerRadius.Value;
			var radiusY = Style.CornerRadius.Value;

			canvas.DrawRoundRect(
				left, right, width, height, radiusX, radiusY,
				new SKPaint { Color = Color.Win10Grey, IsAntialias = true }
			);

			canvas.DrawRoundRect(
				left, right, width, height, radiusX, radiusY,
				new SKPaint { Color = Color.Win10GreyDark, IsAntialias = true, IsStroke = true, StrokeWidth = 1 }
			);

			if(MouseOver || Focused) {
				canvas.DrawRoundRect(
					left, right, width, height, radiusX, radiusY,
					new SKPaint { Color = Color.Win10Blue, IsAntialias = true, IsStroke = true, StrokeWidth = 1 }
				);

				canvas.DrawRoundRect(
					left, right, width, height, radiusX, radiusY,
					new SKPaint { Color = Color.Win10BlueOver, IsAntialias = true }
				);

				if(MouseCurrentlyClicked) {
					canvas.DrawRoundRect(
						left, right, width, height, radiusX, radiusY,
						new SKPaint { Color = Color.Win10BlueOverDark, IsAntialias = true }
					);
				}
			}

			Label.Render(canvas);
			canvas.Restore();
		}

		public override bool KeyUp(Key key) {
			if(key != Key.Enter && key != Key.Space) return false;
			Click();
			return true;
		}

		public override void MouseUp(MouseButton button, Point location) {
			if(BoundingBox.Contains(location) && button == MouseButton.Left)
				Clicked?.Invoke(this);

			MouseCurrentlyClicked = false;
		}

		public void Click() =>
			Clicked?.Invoke(this);

		public Button Click(Action<Button> callback) {
			Clicked += callback;
			return this;
		}
	}
}