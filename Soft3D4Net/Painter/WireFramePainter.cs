using Soft3D4Net.Painter;
using System.Numerics;

namespace Soft3D4Net {
    public class WireFramePainter : IPainter {
        public void DrawTriangle(FrameBuffer surface, ColorRGB color, VertexBuffer vbx, int triangleIndice) {
            var t = vbx.Volume.Triangles[triangleIndice];

            var (t0, t1, t2) = (vbx.Vertices[t.I0], vbx.Vertices[t.I1], vbx.Vertices[t.I2]);

            var l0p0 = t0.Proj; var l0p1 = t1.Proj;
            var l1p0 = t1.Proj; var l1p1 = t2.Proj;
            var l2p0 = t0.Proj; var l2p1 = t2.Proj;

            var l0 = LiangBarskyClipping.Clip(ref l0p0, ref l0p1);
            var l1 = LiangBarskyClipping.Clip(ref l1p0, ref l1p1);
            var l2 = LiangBarskyClipping.Clip(ref l2p0, ref l2p1);

            if(l0) surface.DrawLine(surface.ToScreen3(l0p0), surface.ToScreen3(l0p1), color);
            if(l1) surface.DrawLine(surface.ToScreen3(l1p0), surface.ToScreen3(l1p1), color);
            if(l2) surface.DrawLine(surface.ToScreen3(l2p0), surface.ToScreen3(l2p1), color);
        }

        public void DrawLine(FrameBuffer surface, ColorRGB color, Vector4 projectionP0, Vector4 projectionP1) {
            if(!LiangBarskyClipping.Clip(ref projectionP0, ref projectionP1))
                return;

            var p0 = surface.ToScreen3(projectionP0);
            var p1 = surface.ToScreen3(projectionP1);

            surface.DrawLine(p0, p1, color);
        }
    }
}
