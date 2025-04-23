using System;
using System.Numerics;

namespace Soft3D4Net {
    public interface ICamera {
        Matrix4x4 ViewMatrix { get; }
        Vector3 Position { get; set; }
    }
}