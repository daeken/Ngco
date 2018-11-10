using System;
using System.Collections.Generic;
using PrettyPrinter;
using SkiaSharp;

namespace Ngco {
	public class Context {
		public static Context Instance;
		
		public readonly IRenderer Renderer;
		public BaseWidget Widget;

		public Point MouseLocation;
		public MouseButton MouseButtons;

		public Style BaseStyle;
		
		public readonly List<Style> Styles = new List<Style>();

		public Context(IRenderer renderer) {
			Instance = this;
			Renderer = renderer;
		}

		public void Render() {
			Widget?.UpdateAll(x => x.UpdateStyles());
			Renderer.Render(canvas => {
				canvas.Clear();
				Widget?.CalculateBoundingBox(new Rect(0, 0, (int) Math.Ceiling(Renderer.Width / Renderer.Scale), (int) Math.Ceiling(Renderer.Height / Renderer.Scale)));
				Widget?.Render(canvas);
			});
		}

		public bool HandleKeyDown() {
			return false;
		}

		public bool HandleKeyUp() {
			return false;
		}

		public bool HandleKeyPress(char c) {
			c.Print();
			return false;
		}

		public bool MouseDown(MouseButton button) {
			if(!(Widget?.MouseDown(button, MouseLocation) ?? false)) return false;
			MouseButtons |= button;
			return true;
		}
		
		public void MouseUp(MouseButton button) {
			MouseButtons &= ~button;
			Widget?.MouseUp(button, MouseLocation);
		}

		public bool MouseMove(Point location) {
			MouseLocation = location;
			return Widget?.MouseMove(MouseButtons, location) ?? false;
		}

		public Style Add(Style style) {
			Styles.Add(style);
			return style;
		}
	}
}