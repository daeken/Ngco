using SkiaSharp;

namespace Ngco {
	public class Color {
		// Original Color
		public static readonly Color Transparent = new Color(255, 255, 255, 0);
		public static readonly Color White       = new Color(255, 255, 255);
		public static readonly Color Yellow      = new Color(255, 255,   0);
		public static readonly Color Purple      = new Color(255,   0, 255);
		public static readonly Color Teal        = new Color(  0, 255, 255);
		public static readonly Color Red         = new Color(255,   0,   0);
		public static readonly Color Green       = new Color(  0, 255,   0);
		public static readonly Color Blue        = new Color(  0,   0, 255);
		public static readonly Color Black       = new Color(  0,   0,   0);

		//Win10 Colors
		public static readonly Color Win10Grey         = new Color(225, 225, 225);
		public static readonly Color Win10GreyDark     = new Color(173, 173, 173);
		public static readonly Color Win10Blue         = new Color(  0, 120, 215);
		public static readonly Color Win10BlueOver     = new Color(135, 202, 235, 50);
		public static readonly Color Win10BlueOverDark = new Color( 87, 179, 235, 75);

		public readonly SKColor SkiaColor;

		public Color(byte red, byte green, byte blue, byte alpha = 255) =>
			SkiaColor = new SKColor(red, green, blue, alpha);

		public static implicit operator SKColor(Color color) => color.SkiaColor;
	}
}