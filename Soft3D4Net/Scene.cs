using Soft3D4Net.World;

namespace Soft3D4Net {

    public class Scene {
        public ICamera Camera { get; set; }
        public IWorld World { get; set; }
        public IProjection Projection { get; set; }
        public FrameBuffer Surface { get; set; }
    }
}