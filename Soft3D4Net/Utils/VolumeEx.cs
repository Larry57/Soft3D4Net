﻿using Soft3D4Net.Volume;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Soft3D4Net.Utils {
    static class VolumeEx {
        public static IEnumerable<Vector3> BuildVector3s(this float[] vertices) {
            for(var i = 0; i < vertices.Length; i += 3)
                yield return new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
        }

        public static IEnumerable<int> GetTriangleIndexesHaving(this Vector3 vertex, IVolume volume) {
            for(var i = 0; i < volume.Triangles.Length; i++)
                if(volume.Triangles[i].Contains(vertex, volume.Vertices))
                    yield return i;
        }

        public static Vector3 CalculateVertexNormal(this Vector3 vertex, IVolume volume) {
            var inTriangles = vertex.GetTriangleIndexesHaving(volume);
            if(!inTriangles.Any())
                return Vector3.Zero;
            var sum = inTriangles.Select(idx => volume.Triangles[idx].CalculateNormal(volume.Vertices)).Distinct().Aggregate((v1, v2) => v1 + v2);
            return Vector3.Normalize(sum);
        }

        public static IEnumerable<Vector3> CalculateVertexNormals(this IVolume volume) {
            foreach(var vertex in volume.Vertices)
                yield return vertex.CalculateVertexNormal(volume);
        }

        public static Triangle[] BuildTriangleIndices(this int[] indices) {
            var triangles = new List<Triangle>();
            for(var i = 0; i < indices.Length; i += 3)
                triangles.Add(new Triangle(indices[i], indices[i + 1], indices[i + 2]));

            return triangles.ToArray();
        }
    }
}
