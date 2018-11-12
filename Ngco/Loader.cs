using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ngco.Widgets;
using PrettyPrinter;
using YamlDotNet.RepresentationModel;

namespace Ngco {
	public class Loader {
		public static List<BaseWidget> Load(Context context, string document) {
			var yaml = new YamlStream();
			yaml.Load(new StringReader(document));
			
			var loader = new Loader(context);
			var doc = (YamlMappingNode) yaml.Documents[0].RootNode;
			var widgets = new List<BaseWidget>();
			foreach(var (key, node) in doc)
				switch(key.ToString()) {
					case "styles":
						foreach(var style in (YamlSequenceNode) node)
							loader.ParseStyle((YamlMappingNode) style);
						break;
					case "widgets":
						foreach(var widgetNode in (YamlSequenceNode) node) {
							var widget = loader.ParseNode((YamlMappingNode) widgetNode);
							if(widget != null) widgets.Add(widget);
						}
						break;
					case string x: throw new NotSupportedException(x);
				}
			return widgets;
		}

		readonly Context Context;

		Loader(Context context) =>
			Context = context;

		BaseWidget ParseNode(YamlMappingNode node) {
			var (clsNode, body) = node.Children.First();
			var cls = clsNode.ToString().ToLower();

			BaseWidget widget;
			switch(cls) {
				case "button": widget = new Button(); break;
				case "hbox": widget = new HBox(); break;
				case "label": widget = new Label(); break;
				case "vbox": widget = new VBox(); break;
				default: throw new NotSupportedException($"Unknown widget class: {cls}");
			}

			foreach(var sub in (YamlSequenceNode) body) {
				if(sub is YamlScalarNode scalar) {
					Debug.Assert(cls == "label");
					((Label) widget).Text = scalar.Value;
					continue;
				}
				var (keyNode, valueNode) = ((YamlMappingNode) sub).Children.First();
				var key = keyNode.ToString().ToLower();
				
				if(valueNode is YamlSequenceNode || valueNode is YamlMappingNode) {
					var child = ParseNode((YamlMappingNode) sub);
					if(cls == "button")
						((Button) widget).Label = child;
					else
						((BaseContainer) widget).Add(child);
				} else {
					Debug.Assert(valueNode is YamlScalarNode);
					var value = valueNode.ToString();
					switch(key) {
						case "id":
							widget.SetId(value);
							break;
						case "class":
							widget.AddClass(value);
							break;
						default: throw new NotSupportedException($"Unknown property for widget: {key}");
					}
				}
			}

			return widget;
		}

		void ParseStyle(YamlMappingNode node) {
			var (selNode, body) = node.Children.First();
			var selector = selNode.ToString();
			var style = new Style(selector == "$base" ? "" : selector);

			foreach(var (key, _value) in (YamlMappingNode) body) {
				var value = _value.ToString();
				switch(key.ToString()) {
					case "background-color":
						style.BackgroundColor = ParseColor(value);
						break;
					case "text-color":
						style.TextColor = ParseColor(value);
						break;
					case "outline-color":
						style.OutlineColor = ParseColor(value);
						break;
					case "text-size":
						style.TextSize = int.Parse(value);
						break;
					case "font-family":
						style.FontFamily = value;
						break;
					case "corner-radius":
						style.CornerRadius = int.Parse(value);
						break;
					case "focusable":
						style.Focusable = ParseBool(value);
						break;
					case "enabled":
						style.Enabled = ParseBool(value);
						break;
					case string x: throw new NotSupportedException($"Unknown style property {x}");
				}
			}

			if(selector == "$base")
				Context.BaseStyle = style;
			else
				Context.Add(style);
		}

		Color ParseColor(string value) {
			switch(value.ToLower()) {
				case "transparent": return Color.Transparent;
				case "white": return Color.White;
				case "yellow": return Color.Yellow;
				case "purple": return Color.Purple;
				case "teal": return Color.Teal;
				case "red": return Color.Red;
				case "green": return Color.Green;
				case "blue": return Color.Blue;
				case "black": return Color.Black;
				case string x when x.StartsWith("rgb"):
					var rgb = value.Split('(', 2)[1].Split(')', 2)[0].Split(',').Select(y => byte.Parse(y.Trim())).ToArray();
					return new Color(rgb[0], rgb[1], rgb[2]);
				default:
					throw new NotSupportedException($"Unknown color {value}");
			}
		}

		bool ParseBool(string value) {
			switch(value.ToLower()) {
				case "true": case "1":
					return true;
				default:
					return false;
			}
		}
	}
}