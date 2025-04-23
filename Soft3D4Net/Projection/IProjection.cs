using System;
using System.Numerics;

namespace Soft3D4Net {
    public interface IProjection {
        Matrix4x4 ProjectionMatrix(float w, float h);
    }
}