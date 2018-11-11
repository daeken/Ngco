using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ngco {
	public class Context : IEnumerable<BaseWidget> {
		public static Context Instance;

		public readonly IRenderer Renderer;
		public readonly List<Style> Styles = new List<Style>();

		public Point MouseLocation;
		public MouseButton MouseButtons;
		public Modifier Modifiers;

		public BaseWidget Widget;
		public BaseWidget Focused;
		public Style BaseStyle;

		public Context(IRenderer renderer) {
			Instance = this;
			Renderer = renderer;
		}

		public void Render() {
			Widget?.UpdateAll(x => x.UpdateStyles());
			Renderer.Render(canvas => {
				canvas.Clear(Color.Win10Grey);
				Widget?.CalculateBoundingBox(new Rect(0, 0, (int) Math.Ceiling(Renderer.Width / Renderer.Scale),
					(int) Math.Ceiling(Renderer.Height / Renderer.Scale)));
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

		public bool HandleKeyDown(Key key) {
			switch(key) {
				case Key.Shift:
					Modifiers |= Modifier.Shift;
					break;
				case Key.Alt:
					Modifiers |= Modifier.Alt;
					break;
				case Key.Ctrl:
					Modifiers |= Modifier.Ctrl;
					break;
				case Key.Win:
					Modifiers |= Modifier.Win;
					break;
			}

			if(Focused != null && CallAll(Focused, x => x.KeyDown(key))) return true;
			return false;
		}

		public bool HandleKeyUp(Key key) {
			switch(key) {
				case Key.Shift:
					Modifiers &= ~Modifier.Shift;
					break;
				case Key.Alt:
					Modifiers &= ~Modifier.Alt;
					break;
				case Key.Ctrl:
					Modifiers &= ~Modifier.Ctrl;
					break;
				case Key.Win:
					Modifiers &= ~Modifier.Win;
					break;
			}

			if(Focused != null && CallAll(Focused, x => x.KeyUp(key))) return true;
			if(key == Key.Tab) {
				var next = FindNextFocusable(Focused ?? Widget, !Modifiers.HasFlag(Modifier.Shift));
				if(next == null) {
					if(Focused != null)
						next = FindNextFocusable(Widget, !Modifiers.HasFlag(Modifier.Shift));
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

		BaseWidget FindNextFocusable(BaseWidget cur, bool forward) {
			BaseWidget FindBelow(BaseWidget widget) {
				if(cur != widget && widget.Focusable) return widget;
				return (forward ? widget : widget.Reverse()).Select(FindBelow).FirstOrDefault(bn => bn != null);
			}

			BaseWidget FindAbove(BaseWidget widget) {
				if(!forward && cur != widget && widget.Focusable) return widget;
				if(widget.Parent == null) return null;
				var found = false;
				var elems = new List<BaseWidget>();
				foreach(var elem in forward ? widget.Parent : widget.Parent.Reverse()) {
					if(elem == widget)
						found = true;
					else if(found)
						elems.Add(elem);
				}

				foreach(var elem in elems) {
					var bn = FindBelow(elem);
					if(bn != null) return bn;
				}

				return FindAbove(widget.Parent);
			}

			if(forward) {
				var next = FindBelow(cur);
				if(next != null) return next;
				return FindAbove(cur);
			} else {
				var next = FindAbove(cur);
				if(next != null) return next;
				return FindBelow(cur);
			}
		}

		public bool HandleKeyPress(char key) => Focused != null && CallAll(Focused, x => x.KeyPress(key));

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

		public IEnumerator<BaseWidget> GetEnumerator() => new List<BaseWidget> { Widget }.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}