using System;

namespace Ngco.Widgets
{
    public class VBox : BaseContainer
    {
        public int Spacing  = 10;
        public int HPadding = 10;
        public int VPadding = 10;

        public override void Measure(Size region)
        {
            Size rowSize     = new Size(region.Width, region.Height / Children.Count);
            Rect bb          = new Rect(new Point(), new Size());
            Size contentSize = new Size();

            for (int i = 0; i < Children.Count; i++)
            {
                BaseWidget widget = Children[i];
                widget.Measure(rowSize);

                if (i == 0)
                {
                    contentSize.Width  = Math.Max(contentSize.Width, widget.BoundingBox.Size.Width);
                    contentSize.Height = contentSize.Height + widget.BoundingBox.Size.Height;
                }
                else
                {
                    contentSize.Width  = Math.Max(contentSize.Width, widget.BoundingBox.Size.Width);
                    contentSize.Height = contentSize.Height + widget.BoundingBox.Size.Height + Style.Layout.Spacing;
                }
            }

            Size paddedSize = new Size(contentSize.Width  + Style.Layout.Padding.Left + Style.Layout.Padding.Right,
                                       contentSize.Height + Style.Layout.Padding.Up   + Style.Layout.Padding.Down);

            BoundingBox = bb;

            SetSize(paddedSize);
            ApplyLayoutSize();
        }

        public override void Layout(Rect region)
        {
            Point currentWidgetPosition = region.TopLeft;
            int   rowWidth              = region.Size.Width;

            for (int i = 0; i < Children.Count; i++)
            {
                BaseWidget widget         = Children[i];
                Point      widgetPosition = new Point();

                if (i == 0)
                {
                    currentWidgetPosition.X += Style.Layout.Padding.Left;
                    currentWidgetPosition.Y += Style.Layout.Padding.Up;
                }

                int paddingOffset = -Style.Layout.Padding.Right;
                int midX          = widgetPosition.X + (int)Math.Ceiling((float)rowWidth / 2);

                // Reposition widget
                int widgetMidX = (int)Math.Ceiling((float)widget.BoundingBox.Size.Width / 2);

                widgetPosition.X = midX - widgetMidX;

                // Apply offset
                widgetPosition.X        += paddingOffset;
                widgetPosition           = widgetPosition + currentWidgetPosition;
                currentWidgetPosition.Y += widget.BoundingBox.Size.Height + Style.Layout.Spacing;
                
                // Update position
                widget.SetPosition(widgetPosition);
                widget.BoundingBox.ClipTo(region);
                widget.Layout(widget.BoundingBox);
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