using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ngco {
	public class Selector {
		public readonly List<ISelector> Selectors = new List<ISelector>();
		
		public Selector(string selector) {
			foreach(var match in Regex.Matches(selector, @"(\.[^.#> ]+|#[^.#> ]+|[^.#> ]+|>)")) {
				var sel = match.ToString();
				if(sel[0] == '#')
					Selectors.Add(new IdSelector { Id = sel.Substring(1) });
				else if(sel[0] == '.')
					Selectors.Add(new ClassSelector { Class = sel.Substring(1) });
				else if(sel == ">")
					Selectors.Add(new DescendentSelector());
				else
					Selectors.Add(new WidgetSelector { Class = sel });
			}
			Selectors.Reverse();
		}
		
		public bool Match(BaseWidget widget) =>
			Selectors.All(sel => (widget = sel.Match(widget)) != null);
		
		public static implicit operator Selector(string selector) => new Selector(selector);
	}
	
	public interface ISelector {
		BaseWidget Match(BaseWidget widget);
	}

	public class IdSelector : ISelector {
		public string Id;

		public BaseWidget Match(BaseWidget widget) => Id == widget.Id ? widget : null;
	}

	public class ClassSelector : ISelector {
		public string Class;

		public BaseWidget Match(BaseWidget widget) => widget.Classes.Contains(Class) ? widget : null;
	}

	public class WidgetSelector : ISelector {
		public string Class;

		public BaseWidget Match(BaseWidget widget) => Class.ToLower() == widget.GetType().Name.ToLower() ? widget : null;
	}

	public class DescendentSelector : ISelector {
		public BaseWidget Match(BaseWidget widget) => widget.Parent;
	}
}