﻿using SkiaSharp;
using System;
using System.IO;

namespace Ngco.Widgets {
	public class Image : BaseWidget {
		private SKBitmap ImageLoaded;

		string _Path;

		public string Path {
			get => _Path;
			set {
				_Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
				LoadImage();
			}
		}

		public Image(string path = "") {
			_Path = path;
			LoadImage();
		}

		private void LoadImage() {
			if (_Path != "" && File.Exists(_Path))
				using (FileStream stream = new FileStream(_Path, FileMode.Open))
					ImageLoaded = SKBitmap.Decode(stream);
		}

		public override void Render(RICanvas canvas) {
			if (_Path != "" && File.Exists(_Path)) {
				canvas.Save();
				canvas.ClipRect(BoundingBox);
				canvas.DrawImage(ImageLoaded, new Point(BoundingBox.TopLeft.X, BoundingBox.TopLeft.Y), new SKPaint());
				canvas.Restore();
			}
		}

        public override void Measure(Size region) {
            BoundingBox = _Path != "" && File.Exists(_Path) ? new Rect(new Point(), new Size(ImageLoaded.Width, ImageLoaded.Height)) : new Rect();
            ApplyLayoutSize();
        }

        public override void Layout(Rect region) { }
    }
}