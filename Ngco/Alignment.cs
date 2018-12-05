namespace Ngco
{
    public struct Alignment
    {
        public VerticalAlignment   VerticalAlignment   { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }

        public Alignment(VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            VerticalAlignment   = verticalAlignment;
            HorizontalAlignment = horizontalAlignment;
        }
    }
}