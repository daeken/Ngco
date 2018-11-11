namespace Ngco {
    public struct Margin {
        private int _left;
        private int _up;
        private int _right;
        private int _down;

        public int Left { get => _left; set => _left = value; }
        public int Up { get => _up; set => _up = value; }
        public int Right { get => _right; set => _right = value; }
        public int Down { get => _down; set => _down = value; }

        public Margin(int margin) {
            _left = margin;
            _up = margin;
            _right = margin;
            _down = margin;
        }

        public Margin(int left, int up, int right, int down) {
            _left = left;
            _up = up;
            _right = right;
            _down = down;
        }
    }
}
