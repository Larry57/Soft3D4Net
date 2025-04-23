using Soft3D4Net.Painter;

namespace Soft3D4Net;

public interface IRenderer {
    RendererSettings Settings { get; set; }
    Stats Stats { get; }
    void Render(Scene scene, IPainter painter);
}
