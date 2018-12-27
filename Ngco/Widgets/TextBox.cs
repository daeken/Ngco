using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;
using System.Text;

namespace Ngco.Widgets
{
    public class TextBox : BaseWidget
    {
        BaseWidget _Label;

        public override string[] PropertyKeys { get; } = new string[] { "text" };

        public BaseWidget Label
        {
            get => _Label;
            set
            {
                _Label = value;
                if (value != null) value.Parent = this;
            }
        }

        public event Action<TextBox> Clicked;

        public override bool IsFocusable => true;

        public TextBox(BaseWidget label = null)
        {
            Label = label;

            Focusable = true;
        }

        public override void Load(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("text", out string text))
            {
                Label = new Label(text);
            }
        }

        public override IEnumerator<BaseWidget> GetEnumerator() => new List<BaseWidget> { Label }.GetEnumerator();

        public override void Render(RICanvas canvas)
        {
            canvas.Save();
            canvas.ClipRect(BoundingBox);

            if (Focused)
                ((Label)Label).TextPresenter.ShowCaret = true;

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
            if (Focused)
            {
                Label         label   = (Label)Label;
                StringBuilder builder = new StringBuilder(label.Text);

                switch (key)
                {
                    case Key.Backspace:
                        if (label.TextPresenter.CaretPosition > 0)
                        {
                            builder.Remove(label.TextPresenter.CaretPosition - 1, 1);

                            label.Text = builder.ToString();

                            label.TextPresenter.CaretPosition--;
                        }
                        break;
                    case Key.Delete:
                        if (label.TextPresenter.CaretPosition < label.Text.Length)
                        {
                            builder.Remove(label.TextPresenter.CaretPosition, 1);

                            label.Text = builder.ToString();
                        }
                        break;
                }

                label.KeyDown(key);

                if (key != Key.Enter && key != Key.Space) return false;

                MouseCurrentlyClicked = true;
            }

            return true;
        }

        public override bool KeyUp(Key key)
        {
            if (Focused)
            {
                Label.KeyUp(key);

                if (key != Key.Enter && key != Key.Space) return false;

                MouseCurrentlyClicked = false;

                Click();
            }

            return true;
        }

        public override bool KeyPress(char key)
        {
            if (Focused)
            {
                Label         label   = (Label)Label;
                StringBuilder builder = new StringBuilder(label.Text);

                builder.Insert(label.TextPresenter.CaretPosition, key);

                label.Text = builder.ToString();

                label.TextPresenter.CaretPosition++;

                label.TextPresenter.SelectionStart = label.TextPresenter.SelectionEnd = label.TextPresenter.CaretPosition;
            }

            return false;
        }

        public override void MouseUp(MouseButton button, Point location)
        {
            if (BoundingBox.Contains(location) && button == MouseButton.Left) Click();

            MouseCurrentlyClicked = false;

            Label.MouseUp(button, location);
        }

        public void Click()
        {
            Focused = true;
            Clicked?.Invoke(this);
        }

        public TextBox Click(Action<TextBox> callback)
        {
            Clicked += callback;

            return this;
        }

        public override void OnMeasure(Size region)
        {
            ((Label)Label).TextPresenter = new TextPresenter(Label);

            Label.OnMeasure(new Size(region.Width  - (Layout.Padding.Left + Layout.Padding.Right),
                                     region.Height - (Layout.Padding.Up   + Layout.Padding.Down)));

            BoundingBox = new Rect(new Point(0, 0), new Size(Label.BoundingBox.Size.Width  + Layout.Padding.Left + Layout.Padding.Right,
                                                             Label.BoundingBox.Size.Height + Layout.Padding.Up   + Layout.Padding.Down));

            ApplyLayoutSize();
        }

        public override void OnLayout(Rect region)
        {
            Point labelPosition = new Point();

            labelPosition.X += Layout.Padding.Left;
            labelPosition.Y += Layout.Padding.Up;

            Label.SetPosition(region.TopLeft + labelPosition);
            Label.BoundingBox.ClipTo(region);
            Label.OnLayout(Label.BoundingBox);
        }
    }
}