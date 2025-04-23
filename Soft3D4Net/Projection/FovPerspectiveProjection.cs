using System;
using System.Numerics;

namespace Soft3D4Net {

    public class FovPerspectiveProjection (float FOV, float ZNear, float ZFar): IProjection {

        public Matrix4x4 ProjectionMatrix(float width, float height) => Matrix4x4.CreatePerspectiveFieldOfView(FOV, width / height, ZNear, ZFar);
    }
}
