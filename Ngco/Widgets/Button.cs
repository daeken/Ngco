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

			var position = new Point(BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y);
			var size     = new Point(BoundingBox.Size.Width, BoundingBox.Size.Height);
			var round    = new Point(Style.CornerRadius, Style.CornerRadius);

			canvas.DrawRect(
				position, size,
				new SKPaint { Color = Style.BackgroundColor, IsAntialias = true },
				round
			);

			canvas.DrawRect(
				position, size,
				new SKPaint { Color = Style.OutlineColor, IsAntialias = true, IsStroke = true, StrokeWidth = 1 },
				round
			);

			Label.Render(canvas);
			canvas.Restore();
		}

		public override bool KeyDown(Key key) {
			if(key != Key.Enter && key != Key.Space) return false;
			MouseCurrentlyClicked = true;
			return true;
		}

		public override bool KeyUp(Key key) {
			if(key != Key.Enter && key != Key.Space) return false;
			MouseCurrentlyClicked = false;
			Click();
			return true;
		}

		public override void MouseUp(MouseButton button, Point location) {
			if(BoundingBox.Contains(location) && button == MouseButton.Left)
				Click();

			MouseCurrentlyClicked = false;
		}

		public void Click() {
			Focused = true;
			Clicked?.Invoke(this);
		}

		public Button Click(Action<Button> callback) {
			Clicked += callback;
			return this;
		}
	}
}