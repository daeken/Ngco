using MoreLinq;
using Ngco.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ngco
{
    public static class NgcoQuery
    {
        public static IEnumerable<BaseWidget> All(this IEnumerable<BaseWidget> set)
        {
            IEnumerable<BaseWidget> Sub(BaseWidget elem) => elem.Select(Sub).SelectMany(x => x).Concat(new[] { elem });

            return set.Select(Sub).SelectMany(x => x);
        }

        public static IEnumerable<BaseWidget> Find(this IEnumerable<BaseWidget> set, Selector selector) =>
            set.All().Where(selector.Match);

        public static IEnumerable<BaseWidget> DoPersist(this IEnumerable<BaseWidget> set, Action<BaseWidget> func)
        {
            List<BaseWidget> temp = set.ToList();

            temp.ForEach(func);

            return temp;
        }

        public static IEnumerable<BaseWidget> AddClass(this IEnumerable<BaseWidget> set, string name) =>
            DoPersist(set, widget => widget.AddClass(name));
        
        public static IEnumerable<BaseWidget> Click(this IEnumerable<BaseWidget> set, Action<Button> callback) =>
            DoPersist(set, widget => 
            {
                if(widget is Button button) button.Click(callback);
            });
    }
}