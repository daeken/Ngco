using System;

namespace Ngco {
	public struct Size {
		public static Size Infinite = new Size(-1, -1);
		public int Width, Height;
		
		public Size(int width, int height) {
			Width = width;
			Height = height;
		}
		
		public static Size operator +(Size a, Size b) => new Size(
			a.Width == -1 || b.Width == -1 ? -1 : a.Width + b.Width, 
			a.Height == -1 || b.Height == -1 ? -1 : a.Height + b.Height
		);
		
		public static Size operator -(Size a, Size b) => new Size(
			a.Width == -1 || b.Width == -1 ? -1 : a.Width - b.Width, 
			a.Height == -1 || b.Height == -1 ? -1 : a.Height - b.Height
		);
		
		public static Size operator *(Size a, int scalar) => new Size(
			a.Width == -1 ? -1 : a.Width * scalar, 
			a.Height == -1 ? -1 : a.Height * scalar
		);
		
		public static Size operator *(Size a, float scalar) => new Size(
			(int) Math.Round(a.Width == -1 ? -1 : a.Width * scalar), 
			(int) Math.Round(a.Height == -1 ? -1 : a.Height * scalar)
		);
	}
}