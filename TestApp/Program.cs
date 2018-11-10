﻿using Ngco;
using Ngco.Standalone;
using Ngco.Widgets;
using PrettyPrinter;

namespace TestApp {
	class Program : AppWindow {
		static void Main() => new Program().Run();

		Program() {
			Context.BaseStyle = new Style {
				TextSize = 30, 
				FontFamily = "Arial", 
				TextColor = Color.White, 
				Focusable = false, 
				Enabled = true
			};
			
			Context.Add(new Style("hbox > .testing") {
				TextColor = Color.Green, 
				TextSize = 75
			});
			
			var buttonStyle = Context.Add(new Style("button") {
				TextColor = Color.Blue, 
				TextSize = 40, 
				Focusable = true
			});

			Title = "TestApp";
			Context.Widget = new VBox {
				new Label("Testing labels!"), 
				new Label("Some more testing"),
				new Button(new Label("Button A"))
					.Click(_ => buttonStyle.TextSize += 5),
				new Button(new Label("Button B"))
					.Click(_ => "B".Print()),
				new HBox {
					new Label("And even more").AddStyle(".testing"), 
					new Label("Aaaaand more")
				}
			};
		}
	}
}