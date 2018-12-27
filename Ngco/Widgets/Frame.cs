using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Ngco.Widgets
{
    public class Frame : BaseContainer
    {
        public BaseWidget Content
        {
            get => Children.FirstOrDefault();
            set
            {
                Children.Clear();

                Add(value);

                Context.Instance.InvalidateLayout();
            }
        }

        public override void Load(Dictionary<string, string> properties)
        {
            base.Load(properties);
        }

        public override void Add(BaseWidget widget)
        {
            if (Children.Count == 0)
                base.Add(widget);
            else
                throw new InvalidOperationException("Frame can only have one child.");
        }

        public override void OnLayout(Rect region)
        {
            if (Content != null)
            {
                Point contentPosition = region.TopLeft;

                contentPosition.X += Layout.Padding.Left;
                contentPosition.Y += Layout.Padding.Up;

                Content.SetPosition(contentPosition);
                Content.BoundingBox.ClipTo(region);
                Content.OnLayout(Content.BoundingBox);
            }
        }

        public override void OnMeasure(Size region)
        {
            if (Content != null)
            {
                Content.OnMeasure(new Size(region.Width - Layout.Padding.Left - Layout.Padding.Right, 
                                          region.Height - Layout.Padding.Up   - Layout.Padding.Down));

                BoundingBox = new Rect(new Point(0, 0), new Size(Content.BoundingBox.Size.Width + Layout.Padding.Left + Layout.Padding.Right,
                                                                Content.BoundingBox.Size.Height + Layout.Padding.Up   + Layout.Padding.Down));
            }

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            if (Content != null)
            {
                Content.Render(canvas);
            }
        }
    }
}
