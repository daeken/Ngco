using Ngco;
using Ngco.Standalone;
using Ngco.Widgets;
using PrettyPrinter;

namespace TestApp {
	class Program : AppWindow {
		static void Main() => new Program().Run();

		Program() {
			Context.BaseStyle = new Style {
				TextSize   = 30,
				FontFamily = "Arial",
				TextColor  = Color.White,
				Focusable  = false,
				Enabled    = true
			};

			Context.Add(new Style("hbox > .testing") {
				TextColor = Color.Green,
				TextSize  = 75
			});

			var buttonStyle = Context.Add(new Style("button") {
				TextColor    = Color.Black,
				TextSize     = 16,
				Focusable    = true,
				CornerRadius = 5.0f
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
					new Button(new Label("Foo")),
					new Button(new Label("Bar")),
					new Button(new Label("Baz"))
				},
				new HBox {
					new Label("And even more").AddStyle(".testing"),
					new Label("Aaaaand more")
				}
			};
		}
	}
}