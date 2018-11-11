namespace Ngco {
    public class Layout {
        private int _width;
        private int _height;
        private Margin _margin;
        private Padding _padding;
        private Alignment _alignment;
        private Alignment _contentAlignment;
        private int _spacing;

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public int Spacing { get => _spacing; set => _spacing = value; }

        public Margin Margin { get => _margin; set => _margin = value; }
        public Padding Padding { get => _padding; set => _padding = value; }

        public Alignment Alignment { get => _alignment; set => _alignment = value; }
        public Alignment ContentAlignment { get => _contentAlignment; set => _contentAlignment = value; }

        public Layout() {
            Margin = new Margin(0);
            Padding = new Padding(0);

            Alignment = new Alignment() {
                VerticalAlignment = VerticalAlignment.Up,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            ContentAlignment = new Alignment() {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
        }
    }
}
