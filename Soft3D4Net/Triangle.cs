using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soft3D4Net {
    public readonly struct Triangle {

        int[] IX { get; }

        public Triangle(int p0, int p1, int p2) {
            I0 = p0;
            I1 = p1;
            I2 = p2;

            IX = [p0, p1, p2];
        }

        public readonly int I0 { get; }
        public readonly int I1 { get; }
        public readonly int I2 { get; }

        public readonly Vector3 CalculateNormal(Vector3[] vertices) => Vector3.Normalize(Vector3.Cross(vertices[I1] - vertices[I0], vertices[I2] - vertices[I0]));

        public readonly bool Contains(Vector3 vertex, Vector3[] vertices) => vertices[I0] == vertex || vertices[I1] == vertex || vertices[I2] == vertex;

        public readonly bool IsBehindFarPlane(VertexBuffer vbx) {
            return vbx.Vertices[I0].View.Z > 0 && vbx.Vertices[I1].View.Z > 0 && vbx.Vertices[I2].View.Z > 0;
        }


        public readonly bool IsFacingBack(VertexBuffer vbx) {
            var (v0, v1, v2) = (vbx.Vertices[I0].View, vbx.Vertices[I1].View, vbx.Vertices[I2].View);

            // Calculate the centroid without division, since dividing by 3 doesn't affect the sign .
            var centroid = v0 + v1 + v2;
            // Compute the triangle's unnormalized normal
            var normal = Vector3.Cross(v1 - v0, v2 - v0);

            // The sign of the dot product is unchanged by normalization,
            // so no need to normalize centroid or normal.
            return Vector3.Dot(centroid, normal) >= 0;
        }

        public readonly bool isOutsideFrustum(VertexBuffer vbx) {
            var (p0, p1, p2) = (vbx.Vertices[I0].Proj, vbx.Vertices[I1].Proj, vbx.Vertices[I2].Proj);

            if(p0.W < 0 || p1.W < 0 || p2.W < 0)
                return true;

            if(p0.X < -p0.W && p1.X < -p1.W && p2.X < -p2.W)
                return true;

            if(p0.X > p0.W && p1.X > p1.W && p2.X > p2.W)
                return true;

            if(p0.Y < -p0.W && p1.Y < -p1.W && p2.Y < -p2.W)
                return true;

            if(p0.Y > p0.W && p1.Y > p1.W && p2.Y > p2.W)
                return true;

            if(p0.Z > p0.W && p1.Z > p1.W && p2.Z > p2.W)
                return true;

            // This last one is normally not necessary when a IsTriangleBehind check is done
            if(p0.Z < 0 && p1.Z < 0 && p2.Z < 0)
                return true;

            return false;
        }

        public readonly void TransformProjection(VertexBuffer vbx, Matrix4x4 projectionMatrix) {
            foreach(var v in IX)
                if(vbx.Vertices[v].Proj == Vector4.Zero)
                    vbx.Vertices[v] = vbx.Vertices[v].SetProj(Vector4.Transform(vbx.Vertices[v].View, projectionMatrix));
        }


        public readonly void TransformWorld(VertexBuffer vbx) {
            var worldMatrix = vbx.WorldMatrix;
            var normVertices = vbx.Volume.NormVertices;

            foreach(var v in IX) {
                if(vbx.Vertices[v].Norm == Vector3.Zero)
                    vbx.Vertices[v] = vbx.Vertices[v].SetNorm(Vector3.TransformNormal(normVertices[v], worldMatrix));

                if(vbx.Vertices[v].World == Vector3.Zero)
                    vbx.Vertices[v] = vbx.Vertices[v].SetWorld(Vector3.Transform(vbx.Vertices[v].World, worldMatrix));
            }
        }
    }
}