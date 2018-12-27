using Ngco.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

using static Ngco.Helpers;

namespace Ngco
{
    public class Loader
    {
        readonly Context Context;

        Loader(Context context) => Context = context;

        public static List<BaseWidget> Load(Context context, string document)
        {
            YamlStream yaml = new YamlStream();
            yaml.Load(new StringReader(document));
            
            Loader          loader = new Loader(context);
            YamlMappingNode doc    = (YamlMappingNode)yaml.Documents[0].RootNode;

            List<BaseWidget> widgets = new List<BaseWidget>();

            foreach ((YamlNode key, YamlNode node) in doc)
            {
                switch (key.ToString())
                {
                    case "styles":
                        foreach (var style in (YamlSequenceNode)node)
                        {
                            loader.ParseStyle((YamlMappingNode)style);
                        }
                        break;

                    case "widgets":
                        foreach (var widgetNode in (YamlSequenceNode)node)
                        {
                            var widget  = loader.ParseNode((YamlMappingNode)widgetNode);
                            if (widget != null) widgets.Add(widget);
                        }
                        break;

                    case string x: throw new NotSupportedException(x);
                }
            }

            return widgets;
        }

        BaseWidget ParseNode(YamlMappingNode node)
        {
            var    (clsNode, body) = node.Children.First();
            string cls             = clsNode.ToString().ToLower();

            BaseWidget widget;

            switch (cls)
            {
                case "button":      widget = new Button();      break;
                case "checkbox":    widget = new CheckBox();    break;
                case "frame":       widget = new Frame();       break;
                case "hbox":        widget = new HBox();        break;
                case "image":       widget = new Image();       break;
                case "label":       widget = new Label();       break;
                case "radiobutton": widget = new RadioButton(); break;
                case "textbox":     widget = new TextBox();     break;
                case "vbox":        widget = new VBox();        break;

                default: throw new NotSupportedException($"Unknown widget class: {cls}");
            }

            var subNode = (YamlSequenceNode)body;

            Dictionary<string, string> properties = new Dictionary<string, string>();

            for (int index = 0; index < subNode.Children.Count; index++)
            {
                var    sub                  = subNode.Children[index];
                var    (keyNode, valueNode) = ((YamlMappingNode)sub).Children.First();
                string key                  = keyNode.ToString().ToLower();
                string value                = valueNode.ToString();

                if (widget.PropertyKeys.Contains(key))
                {
                    properties.Add(key, value);

                    subNode.Children.Remove(sub);
                    
                    index--;
                }
            }

            widget.Load(properties);

            foreach (YamlNode sub in (YamlSequenceNode)body)
            {
                if (!(sub is YamlScalarNode))
                {
                    var    (keyNode, valueNode) = ((YamlMappingNode)sub).Children.First();
                    string key                  = keyNode.ToString().ToLower();

                    if (widget is BaseContainer container)
                    {
                        if (valueNode is YamlSequenceNode || valueNode is YamlMappingNode)
                        {
                            BaseWidget child = ParseNode((YamlMappingNode)sub);

                            container.Add(child);
                        }
                    }
                    else
                    {
                        string value = valueNode.ToString();

                        switch (key)
                        {
                            case "id":
                                widget.SetId(value);
                                break;
                            case "class":
                                widget.AddClass(value);
                                break;
                            case "enabled":
                                widget.Enabled = ParseBool(value);
                                break;
                            case "focusable":
                                if (widget.IsFocusable)
                                    widget.Focusable = ParseBool(value);
                                else
                                    throw new NotSupportedException($"{key} is not valid for this widget");
                                break;
                            case "layout":
                                widget.Layout = ParseLayout(value);
                                break;
                            default: throw new NotSupportedException($"Unknown property for widget: {key}");
                        }
                    }
                }
            }

            return widget;
        }

        void ParseStyle(YamlMappingNode node)
        {
            var    (selNode, body) = node.Children.First();
            string selector        = selNode.ToString();
            Style  style           = new Style(selector == "$base" ? "" : selector);

            foreach (var (key, _value) in (YamlMappingNode)body)
            {
                string value = _value.ToString();

                switch (key.ToString())
                {
                    case "background-color": style.BackgroundColor = ParseColor(value);  break;
                    case "text-color":       style.TextColor       = ParseColor(value);  break;
                    case "outline-color":    style.OutlineColor    = ParseColor(value);  break;
                    case "text-size":        style.TextSize        = int.Parse(value);   break;
                    case "font-family":      style.FontFamily      = value;              break;
                    case "corner-radius":    style.CornerRadius    = int.Parse(value);   break;

                    case string x: throw new NotSupportedException($"Unknown style property {x}");
                }
            }

            if (selector == "$base")
            {
                Context.BaseStyle = style;
            }
            else
            {
                Context.Add(style);
            }
        }

        Color ParseColor(string value)
        {
            switch (value.ToLower())
            {
                case "transparent": return Color.Transparent;
                case "white":       return Color.White;
                case "yellow":      return Color.Yellow;
                case "purple":      return Color.Purple;
                case "teal":        return Color.Teal;
                case "red":         return Color.Red;
                case "green":       return Color.Green;
                case "blue":        return Color.Blue;
                case "black":       return Color.Black;

                case string x when x.StartsWith("rgb"):
                    byte[] rgb = value.Split('(', 2)[1].Split(')', 2)[0].Split(',').Select(y => byte.Parse(y.Trim())).ToArray();
                    return new Color(rgb[0], rgb[1], rgb[2]);

                default: throw new NotSupportedException($"Unknown color {value}");
            }
        }

        Layout ParseLayout(string value)
        {
            using (StringReader input = new StringReader(value))
            {
                YamlStream yaml = new YamlStream();
                yaml.Load(input);

                YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
                Layout          layout  = new Layout();

                Alignment alignment;

                foreach ((YamlNode nodekey, YamlNode nodeVal) in mapping)
                {
                    IDictionary<YamlNode, YamlNode> body = ((YamlMappingNode)nodekey).Children;

                    value = body.Keys.ElementAt(1).ToString();

                    switch (body.Keys.ElementAt(0).ToString())
                    {
                        case "width":   layout.Width   = int.Parse(value); break;
                        case "height":  layout.Height  = int.Parse(value); break;
                        case "spacing": layout.Spacing = int.Parse(value); break;
                        case "vertical-alignment":
                            value                       = value.First().ToString().ToUpper() + value.Substring(1);
                            alignment                   = layout.Alignment;
                            alignment.VerticalAlignment = Enum.Parse<VerticalAlignment>(value);
                            layout.Alignment            = alignment;
                            break;

                        case "horizontal-alignment":
                            value                         = value.First().ToString().ToUpper() + value.Substring(1);
                            alignment                     = layout.Alignment;
                            alignment.HorizontalAlignment = Enum.Parse<HorizontalAlignment>(value);
                            layout.Alignment              = alignment;
                            break;

                        case "margin":
                            if (int.TryParse(value, out int margin))
                            {
                                layout.Margin = new Margin(margin);
                            }
                            else
                            {
                                string[] margins = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                                layout.Margin = new Margin()
                                {
                                    Up    = int.Parse(margins[0]),
                                    Right = int.Parse(margins[1]),
                                    Down  = int.Parse(margins[2]),
                                    Left  = int.Parse(margins[3]),
                                };
                            }
                            break;

                        case "padding":
                            if (int.TryParse(value, out int padding))
                            {
                                layout.Padding = new Padding(padding);
                            }
                            else
                            {
                                string[] paddings = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                                layout.Padding = new Padding()
                                {
                                    Up    = int.Parse(paddings[0]),
                                    Right = int.Parse(paddings[1]),
                                    Down  = int.Parse(paddings[2]),
                                    Left  = int.Parse(paddings[3]),
                                };
                            }
                            break;
                    }
                }

                return layout;
            }
        }
    }
}
