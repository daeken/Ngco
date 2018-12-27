using SkiaSharp;
using System;
using System.Collections.Generic;

using static Ngco.Helpers;

namespace Ngco.Widgets
{
    public class RadioButton : BaseWidget
    {
        int _RadioRadius = 10;
        int _RadioSpace  = 5;

        Point _RadioPosition;

        BaseWidget _Label;

        public override string[] PropertyKeys { get; } = new string[] { "checked", "label" };

        bool _RequestChecked = false;

        public BaseWidget Label
        {
            get => _Label;
            set
            {
                _Label = value;
                if (value != null) value.Parent = this;
            }
        }

        public bool Checked
        {
            get
            {
                if(Parent is BaseContainer container)
                {
                    if(container.Token.Owner == this)
                    {
                        return true;
                    }
                }

                return false;
            }
            set
            {
                if (Parent is BaseContainer container)
                {
                    if (container.Token.Owner != this)
                    {
                        if (value)
                        {
                            MutualExclusionToken token = new MutualExclusionToken()
                            {
                                Owner = this
                            };
                            container.Token = token;
                        }
                        else
                            container.Token = new MutualExclusionToken();
                    }
                }
            }
        }

        public override bool IsFocusable => true;

        public bool RequestChecked { get => _RequestChecked; set => _RequestChecked = value; }

        public RadioButton(BaseWidget label = null) =>
            Label = label;

        public override void Load(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("checked", out string isChecked))
            {
                RequestChecked = ParseBool(isChecked);
            }

            if (properties.TryGetValue("label", out string label))
            {
                Label = new Label(label);
            }
        }

        public override IEnumerator<BaseWidget> GetEnumerator() =>
            new List<BaseWidget> { Label }.GetEnumerator();

        public override void OnLayout(Rect region)
        {
            _RadioPosition  = new Point(Layout.Padding.Left + _RadioRadius, (BoundingBox.Size.Height / 2));
            _RadioPosition += BoundingBox.TopLeft;

            Point labelPosition    = new Point();
            labelPosition.X       += _RadioSpace + _RadioRadius * 2 + Layout.Padding.Left;
            int availableHeight    = region.Size.Height - (Layout.Padding.Up + Layout.Padding.Down);
            int availableHeightMid = availableHeight / 2;
            int labelMidY          = Label.BoundingBox.Size.Height / 2;

            // center label
            labelPosition.Y += Layout.Padding.Up + availableHeightMid - labelMidY;

            Label.SetPosition(region.TopLeft + labelPosition);
            Label.BoundingBox.ClipTo(region);
            Label.OnLayout(Label.BoundingBox);
        }

        public override void OnMeasure(Size region)
        {
            Label.OnMeasure(new Size(region.Width - (Layout.Padding.Left + Layout.Padding.Right + _RadioRadius * 2 + _RadioSpace),
                region.Height - (Layout.Padding.Up + Layout.Padding.Down)));

            BoundingBox = new Rect(new Point(0, 0), new Size(Label.BoundingBox.Size.Width + _RadioRadius * 2 + _RadioSpace
                + Layout.Padding.Left + Layout.Padding.Right,
                 Label.BoundingBox.Size.Height + Layout.Padding.Up + Layout.Padding.Down));

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            canvas.Save();
            canvas.ClipRect(BoundingBox);

            canvas.DrawCircle(_RadioPosition, _RadioRadius,
                new SKPaint { Color = Color.White, IsAntialias = true });

            canvas.DrawCircle(_RadioPosition, _RadioRadius,
                new SKPaint { Color = Style.OutlineColor, IsAntialias = true, IsStroke = true, StrokeWidth = 1 });

            if (Checked)
                canvas.DrawCircle(_RadioPosition, _RadioRadius / 2,
                                  new SKPaint { Color = Color.Black, IsAntialias = true });

            Label.Render(canvas);
            canvas.Restore();
        }

        public override void MouseUp(MouseButton button, Point location)
        {
            if (BoundingBox.Contains(location) && button == MouseButton.Left)
                Checked = true;

            MouseCurrentlyClicked = false;
        }
    }
}
