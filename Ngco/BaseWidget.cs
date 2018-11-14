using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ngco {
	public abstract class BaseWidget : IEnumerable<BaseWidget> {
		public Rect BoundingBox { get; protected set; }
		public bool Focusable => IsFocusable && Style.Focusable;

		public bool Focused {
			get => this == Context.Instance.Focused;
			set {
				if(!Style.Focusable) return;
				if(value)
					Context.Instance.Focused = this;
				else if(Focused)
					Context.Instance.Focused = null;
			}
		}

		public readonly List<string> Classes = new List<string>();
		public readonly Style Style = new Style();

		public virtual bool IsFocusable => false;

		public abstract void Render(RICanvas canvas);
		public abstract Rect CalculateBoundingBox(Rect region);

		public string Id;
		public BaseWidget Parent;

		bool _MouseOver;
		public bool MouseOver {
			get => _MouseOver;
			set {
				StylesDirty = StylesDirty || _MouseOver != value;
				_MouseOver = value;
			}
		}
		bool _MouseCurrentlyClicked;
		public bool MouseCurrentlyClicked {
			get => _MouseCurrentlyClicked;
			set {
				StylesDirty = StylesDirty || _MouseCurrentlyClicked != value;
				_MouseCurrentlyClicked = value;
			}
		}

		internal bool StylesDirty = true;

		public virtual bool MouseDown(MouseButton button, Point location) {
			if(button == MouseButton.Left && !BoundingBox.Contains(location)) {
				MouseCurrentlyClicked = false;
				return false;
			}

			foreach(var child in this)
				child.MouseDown(button, location);

			if(button == MouseButton.Left)
				MouseCurrentlyClicked = true;
			
			return true;
		}

		public virtual void MouseUp(MouseButton button, Point location) {
			if(button == MouseButton.Left)
				MouseCurrentlyClicked = false;
			
			if(BoundingBox.Contains(location))
				foreach(var child in this)
					child.MouseUp(button, location);
		}

		public virtual bool MouseMove(MouseButton buttons, Point location) {
			if(!BoundingBox.Contains(location)) {
				UpdateAll(x => x.MouseOver = false);
				return false;
			}

			MouseOver = true;
			foreach(var child in this)
				child.MouseMove(buttons, location);
			return true;
		}

		public virtual bool KeyDown(Key key) => false;
		public virtual bool KeyUp(Key key) => false;
		public virtual bool KeyPress(char key) => false;

		public void UpdateAll(Action<BaseWidget> callback) {
			callback(this);
			this.ForEach(x => x.UpdateAll(callback));
		}

		public BaseWidget SetId(string id) {
			Id = id;
			return this;
		}

		public BaseWidget AddClass(string name) {
			Classes.AddRange(name.Split(' ').Where(x => x.Length != 0));
			StylesDirty = true;
			return this;
		}

		public void UpdateStyles() {
			if(!StylesDirty) return;
			StylesDirty = false;
			Style.Parents.Clear();
			foreach(var style in Context.Instance.Styles)
				if(style.Selector.Match(this))
					Style.Parents.Add(style);
			var parent = Parent;
			while(parent != null) {
				Style.Parents.AddRange(parent.Style.Parents);
				parent = parent.Parent;
			}
			foreach(var child in this) {
				child.StylesDirty = true;
				child.UpdateStyles();
			}
		}

		public virtual IEnumerator<BaseWidget> GetEnumerator() => Enumerable.Empty<BaseWidget>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}