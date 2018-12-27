using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Ngco.Helpers;

namespace Ngco.Widgets
{
    public class Label : BaseWidget
    {
        bool shiftPressed;
        
        public override string[] PropertyKeys { get; } = new string[] { "text", "multiline" };

        private string text;

        public TextPresenter TextPresenter { get; set; }

        public string Text { get => text; set => text = value; }

        private bool _multiline = false;

        public bool Multiline
        {
            get => _multiline;
            set => _multiline = value;
        }

        public Label(string text = "Label")
        {
            Text = text;

            TextPresenter = new TextPresenter(this);
        }

        public override void Load(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("text", out string text))
            {
                Text = text;
            }

            if (properties.TryGetValue("multiline", out string isMultiline))
            {
                Multiline = ParseBool(isMultiline);
            }
        }

        public override void OnMeasure(Size region)
        {

            string[] lines = Multiline ? Text.Split("\n", StringSplitOptions.RemoveEmptyEntries) : new string[] { Text };

            BoundingBox = new Rect(new Point(), new Size((int)Math.Ceiling(TextPresenter.Foreground.MeasureText(lines.OrderByDescending(s => s.Length).First())),
                                                         Style.TextSize * lines.Length));

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            if (TextPresenter.ShowCaret)
            {
                TextPresenter.CaretFrameCount++;

                if (TextPresenter.CaretFrameCount > TextPresenter.CaretFrameInterval)
                {
                    TextPresenter.RenderCaret = !TextPresenter.RenderCaret;

                    TextPresenter.CaretFrameCount = 0;
                }
            }

            canvas.Save();
            canvas.ClipRect(BoundingBox.Inset(BoundingBox.Size * -0.1f));

            canvas.DrawText(Text, BoundingBox.TopLeft.X,
                BoundingBox.TopLeft.Y - (TextPresenter.Foreground.FontSpacing - Style.TextSize) + Style.TextSize, TextPresenter,
                Multiline);

            canvas.Restore();
        }

        public override bool MouseDown(MouseButton button, Point location)
        {
            if (Parent is TextBox && Parent.Focused)
            {
                if (BoundingBox.Contains(location) && button == MouseButton.Left)
                {
                    float xOffset = (location.X - BoundingBox.TopLeft.X) / Context.Instance.Renderer.Scale;
                    float yOffset = (location.Y - BoundingBox.TopLeft.Y) / Context.Instance.Renderer.Scale;

                    TextPresenter.CaretPosition = TextPresenter.GetTextPosition(
                        Text.Split('\n', StringSplitOptions.RemoveEmptyEntries),
                        xOffset, yOffset, TextPresenter.Foreground);

                    if (!shiftPressed)
                        TextPresenter.SelectionStart = TextPresenter.SelectionEnd = TextPresenter.CaretPosition;
                    else
                        TextPresenter.SelectionEnd = TextPresenter.CaretPosition;
                }
            }

            return base.MouseDown(button, location);
        }

        public override bool MouseMove(MouseButton buttons, Point location)
        {
            if (Parent is TextBox && Parent.Focused)
            {
                if (BoundingBox.Contains(location) && buttons == MouseButton.Left)
                {
                    float xOffset = (location.X - BoundingBox.TopLeft.X) / Context.Instance.Renderer.Scale;
                    float yOffset = (location.Y - BoundingBox.TopLeft.Y) / Context.Instance.Renderer.Scale;

                    int newPostition = TextPresenter.GetTextPosition(
                        Text.Split('\n', StringSplitOptions.RemoveEmptyEntries), xOffset, yOffset, TextPresenter.Foreground);

                    TextPresenter.SelectionEnd = TextPresenter.CaretPosition = newPostition;
                }
            }

            return base.MouseMove(buttons, location);
        }

        public override bool KeyDown(Key key)
        {
            StringBuilder builder = new StringBuilder(Text);

            switch (key)
            {
                case Key.Left:
                    TextPresenter.CaretPosition = Math.Max(0, --TextPresenter.CaretPosition);
                    break;
                case Key.Right:
                    TextPresenter.CaretPosition = Math.Min(Text.Length, ++TextPresenter.CaretPosition);
                    break;
                case Key.Shift:
                    shiftPressed = true;
                    break;
            }

            if (!shiftPressed)
                TextPresenter.SelectionStart = TextPresenter.SelectionEnd = TextPresenter.CaretPosition;
            else
                TextPresenter.SelectionEnd = TextPresenter.CaretPosition;

            if (key != Key.Enter && key != Key.Space) return false;

            return true;
        }

        public override bool KeyUp(Key key)
        {
            if (key == Key.Shift)
                shiftPressed = false;

            if (key != Key.Enter && key != Key.Space) return false;

            return true;
        }

        public override void OnLayout(Rect region) { }
    }
}