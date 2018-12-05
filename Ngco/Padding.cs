namespace Ngco
{
    public struct Padding
    {
        public int Left  { get; set; }
        public int Up    { get; set; }
        public int Right { get; set; }
        public int Down  { get; set; }

        public Padding(int padding)
        {
            Left  = padding;
            Up    = padding;
            Right = padding;
            Down  = padding;
        }

        public Padding(int left, int up, int right, int down)
        {
            Left  = left;
            Up    = up;
            Right = right;
            Down  = down;
        }
    }
}