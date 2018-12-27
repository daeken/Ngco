using System;

namespace Ngco.Widgets
{
    public class HBox : BaseContainer
    {
        public override string[] ChildPropertyKeys => new string[0];

        public override void OnMeasure(Size region)
        {
            Size rowSize     = new Size(region.Width, region.Height / Children.Count);
            Rect bb          = new Rect(new Point(), new Size());
            Size contentSize = new Size();

            for (int i = 0; i < Children.Count; i++)
            {
                BaseWidget widget = Children[i];

                widget.OnMeasure(rowSize);

                if (i == 0)
                {
                    contentSize.Height = Math.Max(contentSize.Height, widget.BoundingBox.Size.Height);
                    contentSize.Width  = contentSize.Width + widget.BoundingBox.Size.Width;
                }
                else
                {
                    contentSize.Height = Math.Max(contentSize.Height, widget.BoundingBox.Size.Height);
                    contentSize.Width  = contentSize.Width + widget.BoundingBox.Size.Width + Layout.Spacing;
                }
            }

            Size paddedSize = new Size(contentSize.Width  + Layout.Padding.Left + Layout.Padding.Right,
                                       contentSize.Height + Layout.Padding.Up   + Layout.Padding.Down);

            BoundingBox = bb;

            SetSize(paddedSize);
            ApplyLayoutSize();
        }

        public override void OnLayout(Rect region)
        {
            Point currentWidgetPosition = region.TopLeft;
            int   columnHeight          = region.Size.Height;

            for (int i = 0; i < Children.Count; i++)
            {
                BaseWidget widget         = Children[i];
                Point      widgetPosition = new Point();

                if (i == 0)
                {
                    currentWidgetPosition.X += Layout.Padding.Left;
                    currentWidgetPosition.Y += Layout.Padding.Up;
                }

                int paddingOffset = -Layout.Padding.Down;
                int midY          = widgetPosition.Y + (int)Math.Ceiling((float)columnHeight / 2);

                // Reposition widget
                int widgetMidY = (int)Math.Ceiling((float)widget.BoundingBox.Size.Height / 2);

                widgetPosition.Y = midY - widgetMidY;

                // Apply offset
                widgetPosition.Y        += paddingOffset;
                widgetPosition           = widgetPosition + currentWidgetPosition;
                currentWidgetPosition.X += widget.BoundingBox.Size.Width + Layout.Spacing;

                // Update position
                widget.SetPosition(widgetPosition);
                widget.BoundingBox.ClipTo(region);
                widget.OnLayout(widget.BoundingBox);
            }
        }

        public override void Render(RICanvas canvas)
        {
            foreach (var widget in this)
            {
                widget.Render(canvas);
            }
        }
    }
}