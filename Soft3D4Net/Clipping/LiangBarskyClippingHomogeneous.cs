using System;
using System.Numerics;

namespace Soft3D4Net {

    // Taken from https://fr.mathworks.com/matlabcentral/fileexchange/51550-3d-and-2d-homogeneous-space-line-clipping-using-liang-barsky-algorithm

    public static class LiangBarskyClipping {

        public static bool Clip(ref Vector4 p0, ref Vector4 p1) {

            if(p0.W < 0 && p1.W < 0)
                return false;

            var t0 = 0f;
            var t1 = 1f;

            var delta = p1 - p0;

            if(!Clip(p0.W - p0.X, -delta.W + delta.X, ref t0, ref t1)) return false;
            if(!Clip(p0.W + p0.X, -delta.W - delta.X, ref t0, ref t1)) return false;
            
            if(!Clip(p0.W - p0.Y, -delta.W + delta.Y, ref t0, ref t1)) return false;
            if(!Clip(p0.W + p0.Y, -delta.W - delta.Y, ref t0, ref t1)) return false;

            if(!Clip(p0.W - p0.Z, -delta.W + delta.Z, ref t0, ref t1)) return false;
            if(!Clip(p0.W + p0.Z, -delta.W - delta.Z, ref t0, ref t1)) return false;

            if(t1 < 1)
                p1 = p0 + t1 * delta;

            if(t0 > 0)
                p0 = p0 + t0 * delta;

            return true;
        }

        static bool Clip(float q, float p, ref float t0, ref float t1) {
            if(Math.Abs(p) < float.Epsilon && q < 0)
                return false;

            var r = q / p;

            if(p < 0) {
                if(r > t1) return false;
                if(r > t0) t0 = r;
            }
            else {
                if(r < t0) return false;
                if(r < t1) t1 = r;
            }

            return true;
        }
    }
}
