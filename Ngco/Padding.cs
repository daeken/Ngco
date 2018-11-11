namespace Ngco {
    public struct Padding {
        private int _left;
        private int _up;
        private int _right;
        private int _down;

        public int Left { get => _left; set => _left = value; }
        public int Up { get => _up; set => _up = value; }
        public int Right { get => _right; set => _right = value; }
        public int Down { get => _down; set => _down = value; }

        public Padding(int padding) {
            _left = padding;
            _up = padding;
            _right = padding;
            _down = padding;
        }

        public Padding(int left, int up, int right, int down) {
            _left = left;
            _up = up;
            _right = right;
            _down = down;
        }
    }
}
