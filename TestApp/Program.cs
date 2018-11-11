using System.Linq;
using Ngco;
using Ngco.Standalone;
using Ngco.Widgets;
using PrettyPrinter;

namespace TestApp {
	class Program : AppWindow {
		static void Main() => new Program().Run();

		Program() {
			Context.BaseStyle = new Style {
				BackgroundColor = Color.Transparent,
				OutlineColor = Color.Purple, 
				TextColor  = Color.White,
				TextSize   = 30,
				FontFamily = "Arial",
				Focusable  = false,
				Enabled    = true, 
				CornerRadius = 0
			};

			Context.Add(new Style("hbox > .testing") {
				TextColor = Color.Green,
				TextSize  = 75
			});

			var buttonStyle = Context.Add(new Style("button") {
				BackgroundColor = Color.Win10Grey, 
				OutlineColor = Color.Win10GreyDark, 
				TextColor    = Color.Black,
				TextSize     = 16,
				Focusable    = true
			});

			Context.Add(new Style("button :hover") {
				BackgroundColor = new Color(180, 213, 230), 
				OutlineColor = Color.Win10Blue
			});

			Context.Add(new Style("button :active") {
				BackgroundColor = new Color(156, 213, 230)
			});

			Context.Add(new Style("button .radius") {
				CornerRadius = 10
			});

			Title = "TestApp";

			Context.Widget = new VBox {
				new Label("Testing labels!"),
				new Label("Some more testing"),
				new Button(new Label("Button A"))
					.Click(_ => buttonStyle.TextSize += 5),
				new Button(new Label("Button B"))
					.Click(_ => "B".Print()).AddClass("radius"),
				new HBox {
					new Button(new Label("Foo")),
					new Button(new Label("Bar")),
					new Button(new Label("Baz"))
				},
				new HBox {
					new Label("And even more").AddClass("testing"),
					new Label("Aaaaand more")
				}
			};
			
			Context.Find("hbox > button").Skip(1).Take(1).AddClass("testing");
		}
	}
}