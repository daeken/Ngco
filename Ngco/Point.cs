namespace Ngco
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
        
        public static Point operator +(Point a, Size b) => 
            new Point(
                b.Width  == -1 ? 1000000 : a.X + b.Width, 
                b.Height == -1 ? 1000000 : a.Y + b.Height
            );
        
        public static Point operator -(Point a, Size b) => 
            new Point(
                b.Width  == -1 ? 1000000 : a.X - b.Width, 
                b.Height == -1 ? 1000000 : a.Y - b.Height
            );

        public override string ToString() => $"[ {X} {Y} ]";
    }
}