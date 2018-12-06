﻿using Ngco;
using Ngco.Standalone;
using Ngco.Widgets;
using PrettyPrinter;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestApp
{
    class Program : AppWindow
    {
        static void Main() => 
            new Program().MainLoop();

        Program()
        {
            Title = "TestApp";

            List<BaseWidget> widgets = Loader.Load(Context, File.ReadAllText("layout.yml"));

            if (widgets.Count == 1)
            {
                Context.Widget = widgets[0];
            }
            else
            {
                VBox container = new VBox();

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