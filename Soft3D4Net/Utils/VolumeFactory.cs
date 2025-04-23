using Soft3D4Net.Volume;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Xml.Linq;

namespace Soft3D4Net.Utils {
    public class VolumeFactory {

        static XNamespace ns = "http://www.collada.org/2005/11/COLLADASchema";

        public static IVolume[] HackyImportCollada(string fileName) {

            var xdoc = XDocument.Load(fileName);
            var geometries = xdoc.Root.Element(ns + "library_geometries").Elements(ns + "geometry");

            var volumes = new List<IVolume>();

            foreach(var geometry in geometries) {
                var mesh = geometry.Element(ns + "mesh");

                List<Vector3> vertices = [];
                List<Vector3> normals = [];
                List<int> indices = [];

                var polylist = mesh.Element(ns + "polylist");
                if(polylist != null) {
                    getSource(polylist, "VERTEX", out var polylist_vertex_id, out _);

                    var v = mesh.Elements(ns + "vertices").FirstOrDefault(e => e.Attribute("id")?.Value == polylist_vertex_id);
                    getSource(v, "POSITION", out var vertices_position_id, out _);
                    getSource(v, "NORMAL", out var vertices_normal_id, out _);

                    var polylist_vertices = getArraySource<Vector3>(mesh, vertices_position_id);
                    var polylist_normals = getArraySource<Vector3>(mesh, vertices_normal_id);
                    var polylist_indices = ParseArray<int>(polylist.Element(ns + "p")?.Value);

                    vertices = vertices.Concat(polylist_vertices).ToList();
                    normals = normals.Concat(polylist_normals).ToList();
                    indices = indices.Concat(polylist_indices).ToList();
                }

                var triangles = mesh.Element(ns + "triangles");
                if(triangles != null) {
                    var triangles_indices = ParseArray<int>(triangles.Element(ns + "p")?.Value);
                    var maxOffset = triangles.Elements(ns + "input").Max(e => int.Parse(e.Attribute("offset")?.Value ?? "0"));

                    getSource(triangles, "VERTEX", out var triangles_vertex_id, out var triangle_vertex_offset);

                    var v = mesh.Elements(ns + "vertices").FirstOrDefault(e => e.Attribute("id")?.Value == triangles_vertex_id);
                    getSource(v, "POSITION", out var vertices_position_id, out _);

                    var triangles_vertices = getArraySource<Vector3>(mesh, vertices_position_id);

                    string triangles_normal_id = null;
                    getSource(triangles, "NORMAL", out triangles_normal_id, out var triangle_normal_offset);
                    var triangles_normals = triangles_normal_id == null
                        ? []
                        : getArraySource<Vector3>(mesh, triangles_normal_id, triangles_indices, triangle_normal_offset, maxOffset);

                    var indices_vertexes = triangles_indices
                        .Select((index, i) => new { index, i })
                        .GroupBy(x => x.i % (maxOffset + 1), x => x.index)
                        .Where(x => x.Key == triangle_vertex_offset)
                        .SelectMany(g => g.ToArray())
                        .ToList();

                    vertices = vertices.Concat(triangles_vertices).ToList();
                    normals = normals.Concat(triangles_normals).ToList();
                    indices = indices.Concat(indices_vertexes).ToList();
                }

                volumes.Add(new BaseVolume(
                    vertices.ToArray(),
                    indices.ToArray().BuildTriangleIndices(),
                    normals.Count > 0 ? normals.ToArray() : null,
                    null));
            }

            return volumes.ToArray();
        }
        

        static List<T> ParseArray<T>(string value) {
            return value?.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)?.Select(v => (T)Convert.ChangeType(v, typeof(T), CultureInfo.InvariantCulture))?.ToList() ?? [];
        }

        static void getSource(XElement element, string semantic, out string id, out int offset) {
            var e = element?.Elements(element?.GetDefaultNamespace() + "input")?.FirstOrDefault(e => string.Equals(e.Attribute("semantic")?.Value, semantic));
            id = e?.Attribute("source")?.Value?.TrimStart('#');
            offset = int.Parse(e?.Attribute("offset")?.Value ?? "0");
        }

        static List<T> getArraySource<T>(XElement mesh, string id, List<int> indices = null, int offset = -1, int maxOffset = -1) {

            var data = mesh
                .Elements(ns + "source")
                .FirstOrDefault(e => e.Attribute("id").Value == id)
                .Element(ns + "float_array")
                .Value;

            var floats = ParseArray<float>(data);

            var v = new List<Vector3>();

            if(typeof(T) == typeof(Vector3)) {

                if(indices != null && offset != -1 && maxOffset != -1) {
                    for(var i = 0; i < indices.Count; i += maxOffset + 1) {
                        var index = indices[i + offset];
                        v.Add(new Vector3(floats[index * 3], floats[index * 3 + 1], floats[index * 3 + 2]));
                    }
                }
                else {
                    for(var i = 0; i < floats.Count; i += 3)
                        v.Add(new Vector3(floats[i], floats[i + 1], floats[i + 2]));
                }

                return [.. v.OfType<T>()];
            }

            throw new NotImplementedException($"Type {typeof(T)} is not supported.");
        }
    }
}
