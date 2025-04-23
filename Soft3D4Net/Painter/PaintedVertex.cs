using System.Numerics;

namespace Soft3D4Net
{
    readonly struct PaintedVertex
    {
        public readonly Vector3 Norm { get; init; }
        public readonly Vector3 ScreenProj { get; init;}
        public readonly Vector3 World { get; init; }

        public PaintedVertex(Vector3 norm, Vector3 screenProj, Vector3 world)
        {
            Norm = norm;
            ScreenProj = screenProj;
            World = world;
        }
    }
}