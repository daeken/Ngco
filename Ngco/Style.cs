using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using SkiaSharp;

namespace Ngco {
	public class Style {
		public readonly List<Style> Parents = new List<Style>();
		public readonly Selector Selector;

		public Style(string selector = "") =>
			Selector = new Selector(selector);
		
		Color backgroundColor;
		Color _BackgroundColor {
			get {
				if(backgroundColor != null) return backgroundColor;
				foreach(var style in Parents) {
					var val = style._BackgroundColor;
					if(val != null) return val;
				}
				return null;
			}
		}
		public Color BackgroundColor {
			get => _BackgroundColor ?? (Context.Instance.BaseStyle.backgroundColor ?? throw new NoNullAllowedException());
			set => backgroundColor = value;
		}
		
		Color outlineColor;
		Color _OutlineColor {
			get {
				if(outlineColor != null) return outlineColor;
				foreach(var style in Parents) {
					var val = style._OutlineColor;
					if(val != null) return val;
				}
				return null;
			}
		}
		public Color OutlineColor {
			get => _OutlineColor ?? (Context.Instance.BaseStyle.outlineColor ?? throw new NoNullAllowedException());
			set => outlineColor = value;
		}
		
		Color textColor;
		Color _TextColor {
			get {
				if(textColor != null) return textColor;
				foreach(var style in Parents) {
					var val = style._TextColor;
					if(val != null) return val;
				}
				return null;
			}
		}
		public Color TextColor {
			get => _TextColor ?? (Context.Instance.BaseStyle.textColor ?? throw new NoNullAllowedException());
			set => textColor = value;
		}
		
		int? textSize;
		int? _TextSize {
			get {
				if(textSize != null) return textSize;
				foreach(var style in Parents) {
					var val = style._TextSize;
					if(val != null) return val;
				}
				return null;
			}
		}
		public int TextSize {
			get => _TextSize ?? (Context.Instance.BaseStyle.textSize ?? throw new NoNullAllowedException());
			set => textSize = value;
		}
		
		string fontFamily;
		string _FontFamily {
			get {
				if(fontFamily != null) return fontFamily;
				foreach(var style in Parents) {
					var val = style.FontFamily;
					if(val != null) return val;
				}
				return null;
			}
		}
		public string FontFamily {
			get => _FontFamily ?? (Context.Instance.BaseStyle.fontFamily ?? throw new NoNullAllowedException());
			set => fontFamily = value;
		}
		
		bool? focusable;
		bool? _Focusable {
			get {
				if(focusable != null) return focusable;
				foreach(var style in Parents) {
					var val = style._Focusable;
					if(val != null) return val;
				}
				return null;
			}
		}
		public bool Focusable {
			get => _Focusable ?? (Context.Instance.BaseStyle.focusable ?? throw new NoNullAllowedException());
			set => focusable = value;
		}
		
		bool? enabled;
		bool? _Enabled {
			get {
				if(enabled != null) return enabled;
				foreach(var style in Parents) {
					var val = style._Enabled;
					if(val != null) return val;
				}
				return null;
			}
		}
		public bool Enabled {
			get => _Enabled ?? (Context.Instance.BaseStyle.enabled ?? throw new NoNullAllowedException());
			set => enabled = value;
		}

		int? cornerRadius;
		int? _CornerRadius {
			get {
				if(cornerRadius != null) return cornerRadius;
				foreach(var style in Parents) {
					var val = style._CornerRadius;
					if(val != null) return val;
				}
				return null;
			}
		}
		public int CornerRadius {
			get => _CornerRadius ?? (Context.Instance.BaseStyle.cornerRadius ?? throw new NoNullAllowedException());
			set => cornerRadius = value;
		}

		bool? multiline;
		bool? _Multiline {
			get {
				if (multiline != null) return multiline;
				foreach (var style in Parents) {
					var val = style._Multiline;
					if (val != null) return val;
				}
				return null;
			}
		}
		public bool Multiline {
			get => _Multiline ?? (Context.Instance.BaseStyle.multiline ?? throw new NoNullAllowedException());
			set => multiline = value;
		}
	}
}