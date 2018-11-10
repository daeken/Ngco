namespace Ngco {
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