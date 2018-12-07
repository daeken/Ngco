using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Ngco.Widgets
{
    public class Image : BaseWidget
    {
        private SKBitmap ImageLoaded;

        string _Path;

        public string Path
        {
            get => _Path;
            set
            {
                _Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
                LoadImage();
            }
        }

        public Image(string path = "")
        {
            _Path = path;
            LoadImage();
        }

        public override void Load(YamlNode propertiesNode)
        {
            var properties = (YamlSequenceNode)propertiesNode;
            for (int index = 0; index < properties.Children.Count; index++)
            {
                var    sub                  = properties.Children[index];
                var    (keyNode, valueNode) = ((YamlMappingNode)sub).Children.First();
                string key                  = keyNode.ToString().ToLower();

                string value = valueNode.ToString();

                switch (key)
                {
                    case "path":
                        Path = valueNode.ToString();
                        break;
                    default:
                        continue;
                }

                ((YamlSequenceNode)propertiesNode).Children.Remove(sub);
            }
        }

        private void LoadImage()
        {
            ImageLoaded?.Dispose();

            if (_Path != "" && File.Exists(_Path))
            {
                using (FileStream stream = new FileStream(_Path, FileMode.Open))
                {
                    ImageLoaded = SKBitmap.Decode(stream);
                }
            }
        }

        public override void Render(RICanvas canvas)
        {
            if (_Path != "" && File.Exists(_Path))
            {
                canvas.Save();
                canvas.ClipRect(BoundingBox);
                canvas.DrawImage(ImageLoaded, new Point(BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y), new SKPaint());
                canvas.Restore();
            }
        }

        public override void OnMeasure(Size region)
        {
            BoundingBox = _Path != "" && File.Exists(_Path) ? new Rect(new Point(), new Size(ImageLoaded.Width, ImageLoaded.Height)) : new Rect();

            ApplyLayoutSize();
        }

        public override void OnLayout(Rect region) {}
    }
}