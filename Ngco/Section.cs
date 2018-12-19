using System;
using System.Collections.Generic;
using System.Text;

namespace Ngco
{
    public class Section
    {
        public List<BaseWidget> Widgets { get; } = new List<BaseWidget>();

        public Rect Bounds { get; set; } = new Rect();

        public int Length { get; set; }

        public void Add(BaseWidget widget)
        {
            Widgets.Add(widget);
        }

        public void RemoveAt(int index)
        {
            Widgets.RemoveAt(index);
        }

        public void Remove(BaseWidget widget)
        {
            Widgets.Remove(widget);
        }
    }
}
