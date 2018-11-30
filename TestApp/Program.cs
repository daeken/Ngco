﻿using System.IO;
using System.Linq;
using System.Reflection;
using Ngco;
using Ngco.Standalone;
using Ngco.Widgets;
using PrettyPrinter;

namespace TestApp {
	class Program : AppWindow {
        static void Main() => new Program().MainLoop();

		Program() {
			Title = "TestApp";

			var widgets = Loader.Load(Context, File.ReadAllText("layout.yml"));
			if (widgets.Count == 1)
				Context.Widget = widgets[0];
			else {
				var container = new VBox();
				widgets.ForEach(container.Add);
				Context.Widget = container;
			}

			Context.Find("#button-a").Click(_ => "A".Print());
			Context.Find("#button-b").Click(_ => "B".Print());

			Context.Find("hbox > button").Skip(1).Take(1).AddClass("testing");

            Context.InvalidateLayout();
		}
	}
}