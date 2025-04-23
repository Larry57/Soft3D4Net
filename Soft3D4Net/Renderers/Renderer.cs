using Soft3D4Net.Painter;
using System.Numerics;
using System.Security.Authentication.ExtendedProtection;

namespace Soft3D4Net.Renderers {

    public class Renderer : IRenderer {
        public RendererSettings Settings { get; set; }
        public Stats Stats { get; }

        private WireFramePainter InternalWireFramePainter;

        public Renderer() {
            InternalWireFramePainter = new WireFramePainter();
            Stats = new Stats();
        }

        public void Render(Scene scene, IPainter Painter) {
            var surface = scene.Surface;
            var camera = scene.Camera;
            var projection = scene.Projection;
            var world = scene.World;
            var rendererSettings = Settings;

            Stats.Clear();
            Stats.PaintTime();

            surface.Clear();
            Stats.CalcTime();

            // model => worldMatrix => world => viewMatrix => view => projectionMatrix => projection => toNdc => ndc => toScreen => screen

            var viewMatrix = camera.ViewMatrix;
            var projectionMatrix = projection.ProjectionMatrix(surface.Width, surface.Height);

            // Allocate arrays to store transformed vertices
            using var worldBuffer = new WorldBuffer(world);

            var volumes = world.Volumes;
            var volumeCount = volumes.Count;

            for(var idxVolume = 0; idxVolume < volumeCount; idxVolume++) {

                var vbx = worldBuffer.VertexBuffer[idxVolume];
                var volume = volumes[idxVolume];

                var worldMatrix = volume.WorldMatrix;
                var modelViewMatrix = worldMatrix * viewMatrix;

                vbx.Volume = volume;
                vbx.WorldMatrix = worldMatrix;

                Stats.TotalTriangleCount += volume.Triangles.Length;

                var vertices = volume.Vertices;

                // Transform and store vertices to View
                var vertexCount = vertices.Length;
                for(var idxVertex = 0; idxVertex < vertexCount; idxVertex++) {
                    vbx.Vertices[idxVertex] = vbx.Vertices[idxVertex].SetView(Vector3.Transform(vertices[idxVertex], modelViewMatrix));
                }

                var triangleCount = volume.Triangles.Length;
                for(var idxTriangle = 0; idxTriangle < triangleCount; idxTriangle++) {
                    var t = volume.Triangles[idxTriangle];

                    // Discard if behind far plane
                    if(t.IsBehindFarPlane(vbx)) {
                        Stats.BehindViewTriangleCount++;
                        continue;
                    }

                    // Discard if back facing 
                    if(rendererSettings.BackFaceCulling && t.IsFacingBack(vbx)) {
                        Stats.FacingBackTriangleCount++;
                        continue;
                    }

                    // Project in frustum
                    t.TransformProjection(vbx, projectionMatrix);

                    // Discard if outside view frustum
                    if(t.isOutsideFrustum(vbx)) {
                        Stats.OutOfViewTriangleCount++;
                        continue;
                    }

                    Stats.PaintTime();

                    var color = volume.TriangleColors[idxTriangle];

                    if(rendererSettings.ShowTriangles)
                        InternalWireFramePainter.DrawTriangle(scene.Surface, ColorRGB.Magenta, vbx, idxTriangle);

                    Painter?.DrawTriangle(scene.Surface, color, vbx, idxTriangle);

                    Stats.DrawnTriangleCount++;

                    Stats.CalcTime();
                }
            }

            if(rendererSettings.ShowXZGrid)
                RenderUtils.drawGrid(surface, InternalWireFramePainter, viewMatrix * projectionMatrix, -10, 10);

            if(rendererSettings.ShowAxes)
                RenderUtils.drawAxes(surface, InternalWireFramePainter, viewMatrix * projectionMatrix);
        }
    }
}