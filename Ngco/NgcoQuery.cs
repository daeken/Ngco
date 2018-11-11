using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Ngco {
	public static class NgcoQuery {
		public static IEnumerable<BaseWidget> All(this IEnumerable<BaseWidget> set) {
			IEnumerable<BaseWidget> Sub(BaseWidget elem) =>
				elem.Select(Sub).SelectMany(x => x).Concat(new[] { elem });
			return set.Select(Sub).SelectMany(x => x);
		}

		public static IEnumerable<BaseWidget> Find(this IEnumerable<BaseWidget> set, Selector selector) =>
			set.All().Where(selector.Match);

		public static IEnumerable<BaseWidget> DoPersist(this IEnumerable<BaseWidget> set, Action<BaseWidget> func) {
			var temp = set.ToList();
			temp.ForEach(func);
			return temp;
		}

		public static IEnumerable<BaseWidget> AddClass(this IEnumerable<BaseWidget> set, string name) =>
			DoPersist(set, widget => widget.AddClass(name));
	}
}