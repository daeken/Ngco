using System.Collections.Generic;

namespace Ngco.Widgets
{
    public abstract class BaseContainer : BaseWidget
    {
        public readonly List<BaseWidget> Children = new List<BaseWidget>();

        public override IEnumerator<BaseWidget> GetEnumerator() => Children.GetEnumerator();

        public void Add(BaseWidget widget)
        {
            widget.Parent = this;
            Children.Add(widget);
        }

        public void Remove(BaseWidget widget)
        {
            Children.Remove(widget);
            widget.Parent = null;
        }
    }
}