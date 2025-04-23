using System.Numerics;

namespace Soft3D4Net.Volume {
    public interface IVolume {
        Rotation3D Rotation { get; set; }
        Vector3 Position { get; set; }
        Vector3 Scale { get; }

        ColorRGB[] TriangleColors { get; }
        Triangle[] Triangles { get; }
        Vector3[] Vertices { get; }
        Vector3[] NormVertices { get; }
        // Matrix4x4 WorldMatrix { get; }

        public Matrix4x4 WorldMatrix =>
            Matrix4x4.CreateFromYawPitchRoll(Rotation.YYaw, Rotation.XPitch, Rotation.ZRoll) *
            Matrix4x4.CreateTranslation(Position) *
            Matrix4x4.CreateScale(Scale);
    }
}
