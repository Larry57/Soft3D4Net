using Soft3D4Net.Utils;
using System.Linq;
using System.Numerics;

namespace Soft3D4Net.Volume {
    public class BaseVolume : IVolume {
        public BaseVolume(Vector3[] vertices, Triangle[] triangleIndices, Vector3[] vertexNormals = null, ColorRGB[] triangleColors = null) {
            Vertices = vertices;
            Triangles = triangleIndices;

            NormVertices = vertexNormals == null ? this.CalculateVertexNormals().ToArray() : vertexNormals;
            TriangleColors = triangleColors ?? Enumerable.Repeat(ColorRGB.Gray, Triangles.Length).ToArray();

            Scale = Vector3.One;
        }

        public Vector3[] Vertices { get; }

        public ColorRGB[] TriangleColors { get; }

        public Triangle[] Triangles { get; }

        public Vector3 Centroid { get; }

        public Rotation3D Rotation { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Scale { get; set; }

        public Vector3[] NormVertices { get; set; }
    }
}
