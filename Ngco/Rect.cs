using SkiaSharp;
using System;

namespace Ngco
{
    public struct Rect
    {
        public Point TopLeft;
        public Size  Size;

        public Rect(int x, int y)
        {
            TopLeft = new Point(x, y);
            Size    = Size.Infinite;
        }

        public Rect(Point topLeft)
        {
            TopLeft = topLeft;
            Size    = Size.Infinite;
        }

        public Rect(int x, int y, int width, int height)
        {
            TopLeft = new Point(x, y);
            Size    = new Size(x + width, y + height);
        }
        
        public Rect(Point topLeft, Size size)
        {
            TopLeft = topLeft;
            Size    = size;
        }

        public Rect ClipTo(Rect region)
        {
            Point tl   = new Point(Math.Max(TopLeft.X, region.TopLeft.X), 
                                   Math.Max(TopLeft.Y, region.TopLeft.Y));

            Size size  = new Size(
                region.Size.Width == -1 ? 
                    Size.Width :
                    Size.Width == -1 ? 
                        region.TopLeft.X + region.Size.Width - tl.X :
                        Math.Min(region.TopLeft.X + region.Size.Width, TopLeft.X + Size.Width) - tl.X,
                    region.Size.Height == -1 ? 
                        Size.Height :
                        Size.Height == -1 ? 
                            region.TopLeft.Y + region.Size.Height - tl.Y :
                            Math.Min(region.TopLeft.Y + region.Size.Height, TopLeft.Y + Size.Height) - tl.Y
            );

            return new Rect(tl, size);
        }
        
        public Rect Inset(Size amount) =>
            new Rect(
                TopLeft + amount, 
                Size - amount * 2
            );
        
        public static implicit operator SKRect(Rect rect) => new SKRect(
            rect.TopLeft.X, rect.TopLeft.Y, 
            rect.Size.Width  == -1 ? 1000000 : rect.TopLeft.X + rect.Size.Width, 
            rect.Size.Height == -1 ? 1000000 : rect.TopLeft.Y + rect.Size.Height
        );

        public Rect Extend(Rect b)
        {
            var tl = new Point(Math.Min(TopLeft.X, b.TopLeft.X), 
                               Math.Min(TopLeft.Y, b.TopLeft.Y));

            var br = new Point(
                Math.Max(TopLeft.X + Size.Width,  b.TopLeft.X + b.Size.Width), 
                Math.Max(TopLeft.Y + Size.Height, b.TopLeft.Y + b.Size.Height)
            );

            return new Rect(tl, new Size(br.X - tl.X, br.Y - tl.Y));
        }
        
        public bool Contains(Point point) =>
            TopLeft.X <= point.X && 
            TopLeft.Y <= point.Y &&
            point.X   <  TopLeft.X + Size.Width && 
            point.Y   <  TopLeft.Y + Size.Height;
    }
}