using SkiaSharp;

namespace Ngco {
	public class Color {
		public static Color White = new Color(255, 255, 255);
		public static Color Black = new Color(0, 0, 0);
		public static Color Red = new Color(255, 0, 0);
		public static Color Green = new Color(0, 255, 0);
		public static Color Blue = new Color(0, 0, 255);
		public static Color Purple = new Color(255, 0, 255);
		public static Color Yellow = new Color(255, 255, 0);
		public static Color Teal = new Color(0, 255, 255);
		
		public readonly SKColor SkiaColor;

		public Color(byte red, byte green, byte blue, byte alpha = 255) =>
			SkiaColor = new SKColor(red, green, blue, alpha);

		public static implicit operator SKColor(Color color) => color.SkiaColor;
	}
}