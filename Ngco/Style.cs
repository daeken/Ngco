using System.Collections.Generic;
using System.Data;

namespace Ngco
{
    public class Style
    {
        public readonly List<Style> Parents = new List<Style>();
        public readonly Selector Selector;

        public Style(string selector = "") =>
            Selector = new Selector(selector);

        /// <summary>
        /// Background Color
        /// </summary>

        Color backgroundColor;

        Color _BackgroundColor
        {
            get
            {
                if (backgroundColor != null) return backgroundColor;

                foreach (Style style in Parents)
                {
                    Color val = style._BackgroundColor;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public Color BackgroundColor
        {
            get => _BackgroundColor ?? (Context.Instance.BaseStyle.backgroundColor ?? throw new NoNullAllowedException());
            set => backgroundColor = value;
        }

        /// <summary>
        /// Outline Color
        /// </summary>

        Color outlineColor;

        Color _OutlineColor
        {
            get
            {
                if (outlineColor != null) return outlineColor;

                foreach (Style style in Parents)
                {
                    Color val = style._OutlineColor;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public Color OutlineColor
        {
            get => _OutlineColor ?? (Context.Instance.BaseStyle.outlineColor ?? throw new NoNullAllowedException());
            set => outlineColor = value;
        }
        
        /// <summary>
        /// Text Color
        /// </summary>

        Color textColor;

        Color _TextColor
        {
            get
            {
                if (textColor != null) return textColor;

                foreach (Style style in Parents)
                {
                    Color val = style._TextColor;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public Color TextColor
        {
            get => _TextColor ?? (Context.Instance.BaseStyle.textColor ?? throw new NoNullAllowedException());
            set => textColor = value;
        }

        /// <summary>
        /// Text Size
        /// </summary>

        int? textSize;

        int? _TextSize
        {
            get
            {
                if (textSize != null) return textSize;

                foreach (Style style in Parents)
                {
                    int? val = style._TextSize;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public int TextSize
        {
            get => _TextSize ?? (Context.Instance.BaseStyle.textSize ?? throw new NoNullAllowedException());
            set => textSize = value;
        }

        /// <summary>
        /// Font Family
        /// </summary>

        string fontFamily;

        string _FontFamily
        {
            get
            {
                if (fontFamily != null) return fontFamily;

                foreach (Style style in Parents)
                {
                    string val = style.FontFamily;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public string FontFamily
        {
            get => _FontFamily ?? (Context.Instance.BaseStyle.fontFamily ?? throw new NoNullAllowedException());
            set => fontFamily = value;
        }

        /// <summary>
        /// Focusable
        /// </summary>

        bool? focusable;

        bool? _Focusable
        {
            get
            {
                if (focusable != null) return focusable;

                foreach (Style style in Parents)
                {
                    bool? val = style._Focusable;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public bool Focusable
        {
            get => _Focusable ?? (Context.Instance.BaseStyle.focusable ?? throw new NoNullAllowedException());
            set => focusable = value;
        }

        /// <summary>
        /// Enabled
        /// </summary>

        bool? enabled;

        bool? _Enabled
        {
            get
            {
                if (enabled != null) return enabled;

                foreach (Style style in Parents)
                {
                    bool? val = style._Enabled;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public bool Enabled
        {
            get => _Enabled ?? (Context.Instance.BaseStyle.enabled ?? throw new NoNullAllowedException());
            set => enabled = value;
        }

        /// <summary>
        /// Corner Radius
        /// </summary>

        int? cornerRadius;

        int? _CornerRadius
        {
            get
            {
                if (cornerRadius != null) return cornerRadius;

                foreach (Style style in Parents)
                {
                    int? val = style._CornerRadius;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public int CornerRadius
        {
            get => _CornerRadius ?? (Context.Instance.BaseStyle.cornerRadius ?? throw new NoNullAllowedException());
            set => cornerRadius = value;
        }

        /// <summary>
        /// Multiline
        /// </summary>

        bool? multiline;

        bool? _Multiline
        {
            get
            {
                if (multiline != null) return multiline;

                foreach (Style style in Parents)
                {
                    bool? val = style._Multiline;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public bool Multiline
        {
            get => _Multiline ?? (Context.Instance.BaseStyle.multiline ?? throw new NoNullAllowedException());
            set => multiline = value;
        }

        /// <summary>
        /// Layout
        /// </summary>

        Layout layout;

        Layout _Layout
        {
            get
            {
                if (layout != null) return layout;

                foreach (Style style in Parents)
                {
                    Layout val = style._Layout;

                    if (val != null) return val;
                }

                return null;
            }
        }

        public Layout Layout
        {
            get => _Layout ?? (Context.Instance.BaseStyle.layout ?? throw new NoNullAllowedException());
            set => layout = value;
        }
    }
}