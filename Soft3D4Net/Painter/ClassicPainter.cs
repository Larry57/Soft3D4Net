using Soft3D4Net.Utils;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soft3D4Net.Painter {

    public class ClassicPainter : IPainter {
        
        public void DrawTriangle(FrameBuffer surface, ColorRGB color, VertexBuffer vbx, int triangleIndice) {
            PainterUtils.SortTrianglePoints(vbx, surface, triangleIndice, out var v0, out var v1, out var v2);

            var p0 = v0.ScreenProj; var p1 = v1.ScreenProj; var p2 = v2.ScreenProj;

            var yStart = (int)Math.Max(p0.Y, 0);
            var yEnd = (int)Math.Min(p2.Y, surface.Height - 1);

            // Out if clipped
            if(yStart > yEnd) return;

            var yMiddle = Math.Clamp((int)p1.Y, yStart, yEnd);

            if(PainterUtils.Cross2D(p0, p1, p2) > 0) {
                // P0
                //   P1
                // P2
                PaintHalfTriangle(surface, yStart, yMiddle - 1, color, p0, p2, p0, p1);
                PaintHalfTriangle(surface, yMiddle, yEnd, color, p0, p2, p1, p2);
            }
            else {
                //   P0
                // P1
                //   P2
                PaintHalfTriangle(surface, yStart, yMiddle - 1, color, p0, p1, p0, p2);
                PaintHalfTriangle(surface, yMiddle, yEnd, color, p1, p2, p0, p2);
            }
        }


        static void PaintHalfTriangle(FrameBuffer surface, int yStart, int yEnd, ColorRGB color, Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pd) {
            var mg1 = pa.Y == pb.Y ? 1f : 1 / (pb.Y - pa.Y);
            var mg2 = pd.Y == pc.Y ? 1f : 1 / (pd.Y - pc.Y);

            for(var y = yStart; y <= yEnd; y++) {
                var gradient1 = Math.Clamp((y - pa.Y) * mg1, 0f, 1f);
                var gradient2 = Math.Clamp((y - pc.Y) * mg2, 0f, 1f);

                var sx = float.Lerp(pa.X, pb.X, gradient1);
                var ex = float.Lerp(pc.X, pd.X, gradient2);

                if(sx >= ex) continue;

                var sz = float.Lerp(pa.Z, pb.Z, gradient1);
                var ez = float.Lerp(pc.Z, pd.Z, gradient2);

                ProcessScanLine(surface, y, sx, ex, sz, ez, color);
            }
        }


        static void ProcessScanLine(FrameBuffer surface, float y, float sx, float ex, float sz, float ez, ColorRGB color) {
            var minX = Math.Max(sx, 0);
            var maxX = Math.Min(ex, surface.Width);

            var mx = 1 / (ex - sx);

            for(var x = minX; x < maxX; x++) {
                var gradient = (x - sx) * mx;

                var z = float.Lerp(sz, ez, gradient);

                surface.PutPixel((int)x, (int)y, (int)z, color);
            }
        }
    }
}
