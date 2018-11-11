namespace Ngco.Widgets {
    public class VBox : BaseContainer {
		public int Spacing  = 10;
		public int HPadding = 10;
		public int VPadding = 10;

		public override Rect CalculateBoundingBox(Rect region) {
			var bb = new Rect(region.TopLeft, new Size(HPadding, VPadding));
			for(var i = 0; i < Children.Count; ++i) {
				if(i != 0) bb.Size.Height += Spacing;
				var widget = Children[i];
				bb = bb.Extend(widget.CalculateBoundingBox(new Rect(
					new Point(region.TopLeft.X + HPadding, region.TopLeft.Y + bb.Size.Height), 
					Size.Infinite
				).ClipTo(region)));
			}
			return BoundingBox = bb.ClipTo(region);
		}

		public override void Render(RICanvas canvas) {
			foreach(var widget in this)
				widget.Render(canvas);
		}
	}
}