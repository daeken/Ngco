using Ngco;
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
				TextColor = Color.White
			};
			
			Context.Add(new Style("hbox > .testing") {
				TextColor = Color.Green, 
				TextSize = 75
			});
			
			var buttonStyle = Context.Add(new Style("button") {
				TextColor = Color.Blue, 
				TextSize = 40
			});
			
			Title = "TestApp";
			Context.Widget = new VBox {
				new Label("Testing labels!"), 
				new Label("Some more testing"),
				new Button { Label = new Label("Button!") }
					.Click(_ => Context.Renderer.Scale += 0.1f),
				new HBox {
					new Label("And even more").AddStyle(".testing"), 
					new Label("Aaaaand more")
				}
			};
		}
	}
}