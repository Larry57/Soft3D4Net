using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soft3D4Net.Utils {
    class PainterUtils {
        
        public static void SortTrianglePoints(VertexBuffer vbx, FrameBuffer frameBuffer, int triangleIndices, out PaintedVertex v0, out PaintedVertex v1, out PaintedVertex v2) {
            var t = vbx.Volume.Triangles[triangleIndices];

            var (t0, t1, t2) = (vbx.Vertices[t.I0], vbx.Vertices[t.I1], vbx.Vertices[t.I2]);

            v0 = new PaintedVertex(t0.Norm, frameBuffer.ToScreen3(t0.Proj), t0.World);
            v1 = new PaintedVertex(t1.Norm, frameBuffer.ToScreen3(t1.Proj), t1.World);
            v2 = new PaintedVertex(t2.Norm, frameBuffer.ToScreen3(t2.Proj), t2.World);

            if (v0.ScreenProj.Y > v1.ScreenProj.Y) (v0, v1) = (v1, v0);
            if (v1.ScreenProj.Y > v2.ScreenProj.Y) (v1, v2) = (v2, v1);
            if (v0.ScreenProj.Y > v1.ScreenProj.Y) (v0, v1) = (v1, v0);
        }

        // https://www.geeksforgeeks.org/orientation-3-ordered-points/

        public static float Cross2D(Vector3 p0, Vector3 p1, Vector3 p2) => (p1.X - p0.X) * (p2.Y - p1.Y) - (p1.Y - p0.Y) * (p2.X - p1.X);
    }
}
