using SkiaSharp;
using System;
using System.Collections.Generic;

using static Ngco.Helpers;

namespace Ngco.Widgets
{
    public class CheckBox : BaseWidget
    {
        int _BoxSpace = 10;
        int _BoxLength = 20;
        int _CheckBoxSignPadding = 1;

        Point _CheckBoxPosition;
        Point _CheckBoxSize;

        float _PathScale;

        SKMatrix _PositionMatrix;
        SKMatrix _ScaleMatrix;

        SKPath _CheckMarkPath = SKPath.ParseSvgPathData(@"M 22.566406 4.730469 L 20.773438 3.511719 
C 20.277344 3.175781 19.597656 3.304688 19.265625 3.796875 
L 10.476563 16.757813 L 6.4375 12.71875 C 6.015625 12.296875 
5.328125 12.296875 4.90625 12.71875 L 3.371094 14.253906 
C 2.949219 14.675781 2.949219 15.363281 3.371094 15.789063 
L 9.582031 22 C 9.929688 22.347656 10.476563 22.613281 
10.96875 22.613281 C 11.460938 22.613281 11.957031 22.304688 
12.277344 21.839844 L 22.855469 6.234375 C 23.191406 5.742188 
23.0625 5.066406 22.566406 4.730469 Z ");

        SKPath _RenderedCheck;

        BaseWidget _Label;

        public override string[] PropertyKeys { get; } = new string[] { "checked", "label" };

        public BaseWidget Label
        {
            get => _Label;
            set
            {
                _Label = value;
                if (value != null) value.Parent = this;
            }
        }

        bool _Check = false;

        public bool Checked
        {
            get => _Check;
            set
            {
                _Check = value;
            }
        }

        public override bool IsFocusable => true;

        public CheckBox(BaseWidget label = null) =>
            Label = label;

        public override void Load(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("checked", out string isChecked))
            {
                Checked = ParseBool(isChecked);
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
            _CheckBoxPosition  = new Point(Layout.Padding.Left, (BoundingBox.Size.Height / 2) - (_BoxLength / 2));
            _CheckBoxPosition += BoundingBox.TopLeft;
            _CheckBoxSize      = new Point(_BoxLength, _BoxLength);

            _PositionMatrix = SKMatrix.MakeTranslation(_CheckBoxPosition.X + _CheckBoxSignPadding, _CheckBoxPosition.Y + _CheckBoxSignPadding);
            _RenderedCheck  = new SKPath(_CheckMarkPath);

            _RenderedCheck.Transform(_ScaleMatrix);
            _RenderedCheck.Transform(_PositionMatrix);

            Point labelPosition    = new Point();
            labelPosition.X       += _BoxSpace + _BoxLength + Layout.Padding.Left;
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
            Label.OnMeasure(new Size(region.Width - (Layout.Padding.Left + Layout.Padding.Right + _BoxLength + _BoxSpace),
                region.Height - (Layout.Padding.Up + Layout.Padding.Down)));

            BoundingBox = new Rect(new Point(0, 0), new Size(Label.BoundingBox.Size.Width + _BoxSpace + _BoxLength
                + Layout.Padding.Left + Layout.Padding.Right,
                 Label.BoundingBox.Size.Height + Layout.Padding.Up + Layout.Padding.Down));

            // measure check sign path properties
            var checkSize = _CheckMarkPath.Bounds;
            _PathScale    = _BoxLength / Math.Max(checkSize.Height, checkSize.Width) * 0.70f;
            _ScaleMatrix  = SKMatrix.MakeScale(_PathScale, _PathScale);

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            canvas.Save();
            canvas.ClipRect(BoundingBox);

            canvas.DrawRect(
                _CheckBoxPosition, _CheckBoxSize,
                new SKPaint { Color = Color.White, IsAntialias = true }
            );

            canvas.DrawRect(
                _CheckBoxPosition, _CheckBoxSize,
                new SKPaint { Color = Style.OutlineColor, IsAntialias = true, IsStroke = true, StrokeWidth = 1 }
            );

            if (Checked)
            {
                canvas.DrawPath(_RenderedCheck, _CheckBoxPosition,
                  new SKPaint { Color = Color.Black, IsAntialias = true });
            }

            Label.Render(canvas);
            canvas.Restore();
        }

        public override void MouseUp(MouseButton button, Point location)
        {
            if (BoundingBox.Contains(location) && button == MouseButton.Left)
                Checked = !Checked;

            MouseCurrentlyClicked = false;
        }
    }
}
