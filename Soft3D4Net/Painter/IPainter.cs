namespace Soft3D4Net.Painter {
    public interface IPainter {
        void DrawTriangle(FrameBuffer surface, ColorRGB color, VertexBuffer vbx, int triangleIndice);
    }
}
