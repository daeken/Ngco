using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ngco {
	public class Style {
		public readonly List<Style> Parents = new List<Style>();
		public readonly Selector Selector;

		public Style(string selector = "") =>
			Selector = new Selector(selector);

		Color _TextColor;
		public Color TextColor {
			get {
				if(_TextColor != null) return _TextColor;
				foreach(var style in Parents) {
					var val = style.TextColor;
					if(val != null) return val;
				}
				return Parents.Count != 0 ? null : Context.Instance.BaseStyle._TextColor;
			}
			set => _TextColor = value;
		}
		
		int? _TextSize;
		public int? TextSize {
			get {
				if(_TextSize != null) return _TextSize;
				foreach(var style in Parents) {
					var val = style.TextSize;
					if(val != null) return val;
				}
				return Parents.Count != 0 ? null : Context.Instance.BaseStyle._TextSize;
			}
			set => _TextSize = value;
		}
		
		string _FontFamily;
		public string FontFamily {
			get {
				if(_FontFamily != null) return _FontFamily;
				foreach(var style in Parents) {
					var val = style.FontFamily;
					if(val != null) return val;
				}
				return Parents.Count != 0 ? null : Context.Instance.BaseStyle._FontFamily;
			}
			set => _FontFamily = value;
		}
		
		bool? _Focusable;
		public bool? Focusable {
			get {
				if(_Focusable != null) return _Focusable;
				foreach(var style in Parents) {
					var val = style.Focusable;
					if(val != null) return val;
				}
				return Parents.Count != 0 ? null : Context.Instance.BaseStyle._Focusable;
			}
			set => _Focusable = value;
		}
		
		bool? _Enabled;
		public bool? Enabled {
			get {
				if(_Enabled != null) return _Enabled;
				foreach(var style in Parents) {
					var val = style.Enabled;
					if(val != null) return val;
				}
				return Parents.Count != 0 ? null : Context.Instance.BaseStyle._Enabled;
			}
			set => _Enabled = value;
		}

		int? _CornerRadius;
		public int? CornerRadius {
			get {
				if(_CornerRadius != null) return _CornerRadius;
				foreach(var style in Parents) {
					var val = style.CornerRadius;
					if(val != null) return val;
				}

				return Parents.Count != 0 ? 0 : Context.Instance.BaseStyle._CornerRadius;
			}
			set => _CornerRadius = value;
		}
	}
}