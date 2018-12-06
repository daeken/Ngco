namespace Ngco
{
    public struct Margin
    {
        public int Left  { get; set; }
        public int Up    { get; set; }
        public int Right { get; set; }
        public int Down  { get; set; }

        public Margin(int margin)
        {
            Left  = margin;
            Up    = margin;
            Right = margin;
            Down  = margin;
        }

        public Margin(int left, int up, int right, int down)
        {
            Left  = left;
            Up    = up;
            Right = right;
            Down  = down;
        }
    }
}