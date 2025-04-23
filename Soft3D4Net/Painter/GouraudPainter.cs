using Soft3D4Net.Painter;
using Soft3D4Net.Utils;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soft3D4Net {

    public class GouraudPainter : IPainter {

        public void DrawTriangle(FrameBuffer surface, ColorRGB color, VertexBuffer vbx, int triangleIndice) {
            vbx.Volume.Triangles[triangleIndice].TransformWorld(vbx);

            PainterUtils.SortTrianglePoints(vbx, surface, triangleIndice, out var v0, out var v1, out var v2);

            var p0 = v0.ScreenProj; var p1 = v1.ScreenProj; var p2 = v2.ScreenProj;

            var yStart = (int)Math.Max(p0.Y, 0);
            var yEnd = (int)Math.Min(p2.Y, surface.Height - 1);

            // Out if clipped
            if(yStart > yEnd) return;

            var yMiddle = Math.Clamp((int)p1.Y, yStart, yEnd);

            // This has to move elsewhere
            var lightPos = new Vector3(0, 10, 10);

            // computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color

            var nl0 = MathUtils.ComputeNDotL(v0.World, v0.Norm, lightPos);
            var nl1 = MathUtils.ComputeNDotL(v1.World, v1.Norm, lightPos);
            var nl2 = MathUtils.ComputeNDotL(v2.World, v2.Norm, lightPos);

            if(PainterUtils.Cross2D(p0, p1, p2) > 0) {
                // P0
                //   P1
                // P2
                PaintHalfTriangle(surface, yStart, (int)yMiddle - 1, color, p0, p2, p0, p1, nl0, nl2, nl0, nl1);
                PaintHalfTriangle(surface, (int)yMiddle, yEnd, color, p0, p2, p1, p2, nl0, nl2, nl1, nl2);
            }
            else {
                //   P0
                // P1 
                //   P2
                PaintHalfTriangle(surface, yStart, (int)yMiddle - 1, color, p0, p1, p0, p2, nl0, nl1, nl0, nl2);
                PaintHalfTriangle(surface, (int)yMiddle, yEnd, color, p1, p2, p0, p2, nl1, nl2, nl0, nl2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PaintHalfTriangle(FrameBuffer surface, int yStart, int yEnd, ColorRGB color, Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pd, float nla, float nlb, float nlc, float nld) {
            var mg1 = pa.Y == pb.Y ? 1f : 1 / (pb.Y - pa.Y);
            var mg2 = pd.Y == pc.Y ? 1f : 1 / (pd.Y - pc.Y);

            for (var y = yStart; y <= yEnd; y++) {
                var gradient1 = (y - pa.Y) * mg1;
                var gradient2 = (y - pc.Y) * mg2;

                gradient1 = gradient1 < 0f ? 0f : (gradient1 > 1f ? 1f : gradient1);
                gradient2 = gradient2 < 0f ? 0f : (gradient2 > 1f ? 1f : gradient2);

                var sx = pa.X + (pb.X - pa.X) * gradient1;
                var ex = pc.X + (pd.X - pc.X) * gradient2;

                if (sx >= ex) continue;

                var sl = nla + (nlb - nla) * gradient1;
                var el = nlc + (nld - nlc) * gradient2;

                var sz = pa.Z + (pb.Z - pa.Z) * gradient1;
                var ez = pc.Z + (pd.Z - pc.Z) * gradient2;

                PaintScanline(surface, y, sx, ex, sz, ez, sl, el, color);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PaintScanline(FrameBuffer surface, float y, float sx, float ex, float sz, float ez, float sl, float el, ColorRGB color) {
            var minX = Math.Max(sx, 0);
            var maxX = Math.Min(ex, surface.Width);

            if (minX >= maxX) return;

            var mx = 1 / (ex - sx);
            var dz = (ez - sz) * mx;
            var dl = (el - sl) * mx;

            var z = sz + (minX - sx) * dz;
            var c = sl + (minX - sx) * dl;

            for (var x = minX; x < maxX; x++) {
                surface.PutPixel((int)x, (int)y, (int)z, c * color);
                z += dz;
                c += dl;
            }
        }
    }
}
