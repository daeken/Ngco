namespace Ngco
{
    public class Layout
    {
        public int Width   { get; set; }
        public int Height  { get; set; }
        public int Spacing { get; set; }

        public Margin  Margin  { get; set; }
        public Padding Padding { get; set; }

        public Alignment Alignment        { get; set; }
        public Alignment ContentAlignment { get; set; }

        public Layout()
        {
            Margin  = new Margin(0);
            Padding = new Padding(0);

            Alignment = new Alignment()
            {
                VerticalAlignment   = VerticalAlignment.Up,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            ContentAlignment = new Alignment()
            {
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
        }
    }
}