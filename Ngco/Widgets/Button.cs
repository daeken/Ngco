using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Ngco.Widgets
{
    public class Button : BaseWidget
    {
        BaseWidget _Label;

        public BaseWidget Label
        {
            get => _Label;
            set
            {
                _Label = value;
                if (value != null) value.Parent = this;
            }
        }

        public event Action<Button> Clicked;

        public override bool IsFocusable => true;

        public override IEnumerator<BaseWidget> GetEnumerator() => new List<BaseWidget> { Label }.GetEnumerator();

        public Button(BaseWidget label = null) => Label = label;

        public override void Measure(Size region)
        {
            Label.Measure(new Size(region.Width  - (Style.Layout.Padding.Left + Style.Layout.Padding.Right),
                                   region.Height - (Style.Layout.Padding.Up   + Style.Layout.Padding.Down)));

            BoundingBox = new Rect(new Point(0, 0), new Size(Label.BoundingBox.Size.Width  + Style.Layout.Padding.Left + Style.Layout.Padding.Right,
                                                             Label.BoundingBox.Size.Height + Style.Layout.Padding.Up   + Style.Layout.Padding.Down));

            ApplyLayoutSize();
        }

        public override void Layout(Rect region)
        {
            Point labelPosition = new Point();

            switch (Label.Style.Layout.ContentAlignment.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    labelPosition.X += Style.Layout.Padding.Left;
                    break;

                case HorizontalAlignment.Center:
                    int availableWidth    = region.Size.Width - (Style.Layout.Padding.Left + Style.Layout.Padding.Right);
                    int availableWidthMid = availableWidth / 2;
                    int labelMidX         = Label.BoundingBox.Size.Width / 2;

                    // Center label
                    labelPosition.X += Style.Layout.Padding.Left + availableWidthMid - labelMidX;
                    break;

                case HorizontalAlignment.Right:
                    labelPosition.X = region.Size.Width - Style.Layout.Padding.Right - Label.BoundingBox.Size.Width;
                    break;
            }

            switch (Label.Style.Layout.ContentAlignment.VerticalAlignment)
            {
                case VerticalAlignment.Up:
                    labelPosition.Y += Style.Layout.Padding.Up;
                    break;

                case VerticalAlignment.Center:
                    int availableHeight    = region.Size.Height - (Style.Layout.Padding.Up + Style.Layout.Padding.Down);
                    int availableHeightMid = availableHeight / 2;
                    int labelMidY          = Label.BoundingBox.Size.Height / 2;

                    // Center label
                    labelPosition.Y += Style.Layout.Padding.Up + availableHeightMid - labelMidY;

                    break;

                case VerticalAlignment.Down:
                    labelPosition.Y = region.Size.Height - Style.Layout.Padding.Down - Label.BoundingBox.Size.Height;
                    break;

                case VerticalAlignment.Stretch:
                    // ... TODO ?
                    break;
            }

            Label.SetPosition(region.TopLeft + labelPosition);
            Label.BoundingBox.ClipTo(region);
            Label.Layout(Label.BoundingBox);
        }

        public override void Render(RICanvas canvas)
        {
            canvas.Save();
            canvas.ClipRect(BoundingBox);

            Point position = new Point(BoundingBox.TopLeft.X,  BoundingBox.TopLeft.Y);
            Point size     = new Point(BoundingBox.Size.Width, BoundingBox.Size.Height);
            Point round    = new Point(Style.CornerRadius,     Style.CornerRadius);

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

        public override bool KeyDown(Key key)
        {
            if (key != Key.Enter && key != Key.Space) return false;

            MouseCurrentlyClicked = true;

            return true;
        }

        public override bool KeyUp(Key key)
        {
            if (key != Key.Enter && key != Key.Space) return false;

            MouseCurrentlyClicked = false;

            Click();

            return true;
        }

        public override void MouseUp(MouseButton button, Point location)
        {
            if (BoundingBox.Contains(location) && button == MouseButton.Left) Click();

            MouseCurrentlyClicked = false;
        }

        public void Click()
        {
            Focused = true;
            Clicked?.Invoke(this);
        }

        public Button Click(Action<Button> callback)
        {
            Clicked += callback;

            return this;
        }
    }
}