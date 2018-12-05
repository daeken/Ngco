using System;

namespace Ngco
{
    public interface IRenderer
    {
        int   Width  { get; set; }
        int   Height { get; set; }
        float Scale  { get; set; }

        void Render(Action<RICanvas> inside);
    }
}