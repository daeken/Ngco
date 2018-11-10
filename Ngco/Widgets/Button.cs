using System;
using System.Collections.Generic;
using PrettyPrinter;
using SkiaSharp;

namespace Ngco.Widgets {
	public class Button : BaseWidget {
		Label _Label;
		public Label Label {
			get => _Label;
			set => (_Label = value).Parent = this;
		}
		public event Action<Button> Clicked; 

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
			Label.Render(canvas);
			canvas.DrawRect(
				BoundingBox.TopLeft.X + 4, BoundingBox.TopLeft.Y + 4, 
				BoundingBox.Size.Width - 4, BoundingBox.Size.Height - 4, 
				new SKPaint { Color = Color.White, IsStroke = true, StrokeWidth = MouseOver || Focused ? 4 : 2 }
			);
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
		}

		public void Click() =>
			Clicked?.Invoke(this);

		public Button Click(Action<Button> callback) {
			Clicked += callback;
			return this;
		}
	}
}