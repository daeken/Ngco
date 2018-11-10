using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;
using PrettyPrinter;
using SkiaSharp;

namespace Ngco {
	public abstract class BaseWidget : IEnumerable<BaseWidget> {
		public string Id;
		public readonly List<string> Classes = new List<string>();
		public readonly Style Style = new Style();
		bool StylesDirty = true;

		public BaseWidget Parent;
		
		public Rect BoundingBox { get; protected set; }
		
		public abstract void Render(RICanvas canvas);
		public abstract Rect CalculateBoundingBox(Rect region);

		public bool MouseOver;

		public virtual bool MouseDown(MouseButton button, Point location) {
			if(!BoundingBox.Contains(location)) return false;
			foreach(var child in this)
				child.MouseDown(button, location);
			return true;
		}

		public virtual void MouseUp(MouseButton button, Point location) {
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

		public void UpdateAll(Action<BaseWidget> callback) {
			callback(this);
			this.ForEach(x => x.UpdateAll(callback));
		}

		public void Add(string selector) {
			Id = Regex.Match(selector, @"#([^#.]+)").Groups[1].Value;
			Classes.AddRange(Regex.Matches(selector, @"\.([^#.]+)").Select(x => x.Groups[1].Value));
			Id = Id.Length == 0 ? null : Id;
		}

		public BaseWidget AddStyle(string selector) {
			Add(selector);
			StylesDirty = true;
			return this;
		}

		public void UpdateStyles() {
			if(!StylesDirty) return;
			StylesDirty = false;
			Style.Parents.Clear();
			foreach(var style in Context.Instance.Styles)
				if(style.Match(this))
					Style.Parents.Add(style);
			var parent = Parent;
			while(parent != null) {
				Style.Parents.AddRange(parent.Style.Parents);
				parent = parent.Parent;
			}
		}

		public virtual IEnumerator<BaseWidget> GetEnumerator() => Enumerable.Empty<BaseWidget>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}