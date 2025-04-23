using Soft3D4Net.Volume;
using Soft3D4Net.World;
using System;
using System.Buffers;
using System.Numerics;

namespace Soft3D4Net {

    public class WorldBuffer : IDisposable {
        private static ArrayPool<VertexBuffer> vertexBuffer3bag = ArrayPool<VertexBuffer>.Shared;

        public VertexBuffer[] VertexBuffer { get; }

        int size { get; }

        public WorldBuffer(IWorld w) {
            var volumes = w.Volumes;
            size = volumes.Count;

            VertexBuffer = vertexBuffer3bag.Rent(size);

            for(var i = 0; i < size; i++) {
                VertexBuffer[i] = new VertexBuffer(volumes[i].Vertices.Length);
            }
        }

        public void Dispose() {
            var nv = VertexBuffer.Length;
            for(var i = 0; i < nv; i++) {
                VertexBuffer[i]?.Dispose();
            }
            vertexBuffer3bag.Return(VertexBuffer, true);
        }
    }

    public readonly struct Vertices {

        public readonly Vector3 View { get; init; }

        public Vertices SetView(Vector3 value) => new() { Norm = this.Norm, Proj = this.Proj, World = this.World, View = value };

        public readonly Vector3 World { get; init; }

        public Vertices SetWorld(Vector3 value) => new() { Norm = this.Norm, Proj = this.Proj, World = value, View = this.View };

        public readonly Vector3 Norm { get; init; }

        public Vertices SetNorm(Vector3 value) => new() { Norm = value, Proj = this.Proj, World = this.World, View = this.View };

        public readonly Vector4 Proj { get; init; }

        public Vertices SetProj(Vector4 value) => new() { Norm = this.Norm, Proj = value, World = this.World, View = this.View };
    }

    public class VertexBuffer : IDisposable {

        private static ArrayPool<Vertices> verticeBag = ArrayPool<Vertices>.Create();

        public IVolume Volume { get; set; }

        public Vertices[] Vertices { get; }

        int size { get; }

        public Matrix4x4 WorldMatrix { get; set; }

        public VertexBuffer(int vertexCount) {
            this.size = vertexCount;
            Vertices = verticeBag.Rent(vertexCount);
        }
        public void Dispose() => verticeBag.Return(Vertices, true);
    }
}