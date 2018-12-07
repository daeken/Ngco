using SkiaSharp;
using System;
using System.Linq;
using YamlDotNet.RepresentationModel;

using static Ngco.Parsers;

namespace Ngco.Widgets
{
    public class Label : BaseWidget
    {
        public string Text;

        SKPaint Paint => new SKPaint
        {
            Color = Style.TextColor,
            IsAntialias = true,
            TextSize = Style.TextSize,
            Typeface = SKTypeface.FromFamilyName(Style.FontFamily ?? "Arial")
        };

        private bool _multiline = false;

        public bool Multiline
        {
            get => _multiline;
            set => _multiline = value;
        }

        public Label(string text = "Label") => Text = text;

        public override void Load(YamlNode propertiesNode)
        {
            var properties = (YamlSequenceNode)propertiesNode;
            for (int index = 0; index < properties.Children.Count; index++)
            {
                var sub = properties.Children[index];
                var (keyNode, valueNode) = ((YamlMappingNode)sub).Children.First();
                string key = keyNode.ToString().ToLower();

                string value = valueNode.ToString();

                switch (key)
                {
                    case "label":
                        Text = valueNode.ToString();
                        break;
                    case "multiline":
                        Multiline = ParseBool(valueNode.ToString());
                        break;
                    default:
                        continue;
                }

                ((YamlSequenceNode)propertiesNode).Children.Remove(sub);
            }
        }

        public override void Measure(Size region)
        {
            string[] lines = Multiline ? Text.Split("\n", StringSplitOptions.RemoveEmptyEntries) : new string[] { Text };

            BoundingBox = new Rect(new Point(), new Size((int)Math.Ceiling(Paint.MeasureText(lines.OrderByDescending(s => s.Length).First())),
                                                         Style.TextSize * lines.Length));

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            var paint = Paint;

            canvas.Save();
            canvas.ClipRect(BoundingBox.Inset(BoundingBox.Size * -0.1f));
            canvas.DrawText(Text, BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y - (paint.FontSpacing - Style.TextSize) + Style.TextSize, paint, Multiline);
            canvas.Restore();
        }

        public override void Layout(Rect region) { }
    }
}