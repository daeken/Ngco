using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Ngco
{
    public abstract class BaseWidget : IEnumerable<BaseWidget>
    {
        public readonly List<string> Classes = new List<string>();
        public readonly Style Style = new Style();

        public virtual string[] PropertyKeys { get; } = new string[0];

        public virtual bool IsFocusable => false;

        public abstract void Render(RICanvas canvas);
        public abstract void OnMeasure(Size region);
        public abstract void OnLayout(Rect region);

        public virtual void Load(Dictionary<string,string> properties) { }

        public Rect BoundingBox { get; protected set; }

        /// <summary>
        /// Enabled
        /// </summary>

        private bool enabled = true;

        bool _Enabled
        {
            get
            {
                return enabled && Parent._Enabled;
            }
        }

        public bool Enabled
        {
            get => _Enabled;
            set => enabled = value;
        }

        /// <summary>
        /// Focusable
        /// </summary>

        private bool _Focusable;

        public bool Focusable
        {
            get => _Focusable && IsFocusable;
            set => _Focusable = value && IsFocusable;
        }

        public bool Focused
        {
            get => this == Context.Instance.Focused;
            set
            {
                if (!Focusable) return;
                if (value) Context.Instance.Focused = this;
                else if (Focused) Context.Instance.Focused = null;
            }
        }

        private Layout _layout;

        public Layout Layout
        {
            get => _layout?? Layout.Default;
            set => _layout = value;
        }

        internal bool StylesDirty = true;
        private  bool _MouseOver;
        private  bool _MouseCurrentlyClicked;

        public bool MouseOver
        {
            get => _MouseOver;
            set
            {
                StylesDirty = StylesDirty || _MouseOver != value;
                _MouseOver  = value;
            }
        }

        public bool MouseCurrentlyClicked
        {
            get => _MouseCurrentlyClicked;
            set
            {
                StylesDirty            = StylesDirty || _MouseCurrentlyClicked != value;
                _MouseCurrentlyClicked = value;
            }
        }

        public string     Id     { get; set; }
        public BaseWidget Parent { get; set; }

        public virtual bool MouseDown(MouseButton button, Point location)
        {
            if (button == MouseButton.Left && !BoundingBox.Contains(location))
            {
                MouseCurrentlyClicked = false;

                return false;
            }

            foreach (var child in this)
            {
                child.MouseDown(button, location);
            }

            if (button == MouseButton.Left) MouseCurrentlyClicked = true;

            return true;
        }

        public virtual void MouseUp(MouseButton button, Point location)
        {
            if (button == MouseButton.Left) MouseCurrentlyClicked = false;

            if (BoundingBox.Contains(location))
            {
                foreach (var child in this)
                {
                    child.MouseUp(button, location);
                }
            }
        }

        public virtual bool MouseMove(MouseButton buttons, Point location)
        {
            if (!BoundingBox.Contains(location))
            {
                UpdateAll(x => x.MouseOver = false);

                return false;
            }

            MouseOver = true;

            foreach (var child in this)
            {
                child.MouseMove(buttons, location);
            }

            return true;
        }

        public virtual bool KeyDown(Key key) => false;
        public virtual bool KeyUp(Key key) => false;
        public virtual bool KeyPress(char key) => false;

        public void UpdateAll(Action<BaseWidget> callback)
        {
            callback(this);
            this.ForEach(x => x.UpdateAll(callback));
        }

        public BaseWidget SetId(string id)
        {
            Id = id;

            return this;
        }

        public BaseWidget AddClass(string name)
        {
            Classes.AddRange(name.Split(' ').Where(x => x.Length != 0));
            
            StylesDirty = true;

            return this;
        }

        public void SetPosition(Point position)
        {
            BoundingBox = new Rect(position, BoundingBox.Size);
        }

        public void SetSize(Size size)
        {
            BoundingBox = new Rect(BoundingBox.TopLeft, size);
        }

        public void UpdateStyles()
        {
            if (!StylesDirty) return;

            StylesDirty = false;
            Style.Parents.Clear();

            foreach (Style style in Context.Instance.Styles)
            {
                if (style.Selector.Match(this)) Style.Parents.Add(style);
            }

            BaseWidget parent = Parent;

            while (parent != null)
            {
                Style.Parents.AddRange(parent.Style.Parents);
                parent = parent.Parent;
            }

            foreach (var child in this)
            {
                child.StylesDirty = true;
                child.UpdateStyles();
            }
        }

        public void ApplyLayoutSize()
        {
            if (Layout.Width  != 0) SetSize(new Size(Layout.Width, BoundingBox.Size.Height));
            if (Layout.Height != 0) SetSize(new Size(BoundingBox.Size.Width, Layout.Height));
        }

        public virtual IEnumerator<BaseWidget> GetEnumerator() => Enumerable.Empty<BaseWidget>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
