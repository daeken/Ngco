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

		public BaseWidget Focused;
		
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

		bool CallAll(BaseWidget inner, Func<BaseWidget, bool> func) {
			var clist = new List<BaseWidget>();
			while(inner != null) {
				clist.Add(inner);
				inner = inner.Parent;
			}
			clist.Reverse();

			var handled = false;
			foreach(var elem in clist)
				if(func(elem))
					handled = true;
			return handled;
		}

		public bool HandleKeyDown(Key key) => Focused != null && CallAll(Focused, x => x.KeyDown(key));
		public bool HandleKeyUp(Key key) {
			if(Focused != null && CallAll(Focused, x => x.KeyUp(key))) return true;
			if(key == Key.Tab) {
				var next = FindNextFocusable(Focused ?? Widget);
				if(next == null) {
					if(Focused != null)
						next = FindNextFocusable(Widget);
					if(next == null)
						return false;
				}

				if(next == Focused)
					Focused = null;
				else
					next.Focused = true;
				return true;
			}
			return false;
		}

		BaseWidget FindNextFocusable(BaseWidget cur) {
			BaseWidget FindBelow(BaseWidget widget) {
				if(cur != widget && widget.Focusable)
					return widget;
				foreach(var elem in widget) {
					var bn = FindBelow(elem);
					if(bn != null) return bn;
				}
				return null;
			}
			BaseWidget FindAbove(BaseWidget widget) {
				if(widget.Parent == null) return null;
				var found = false;
				foreach(var elem in widget.Parent) {
					if(elem == widget)
						found = true;
					else if(found) {
						var bn = FindBelow(elem);
						if(bn != null) return bn;
					}
				}
				return null;
			}

			var next = FindBelow(cur);
			if(next != null) return next;
			return FindAbove(cur);
		}

		public bool HandleKeyPress(char key)  => Focused != null && CallAll(Focused, x => x.KeyPress(key));

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