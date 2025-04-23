using Soft3D4Net.Volume;
using System.Collections.Generic;

namespace Soft3D4Net.World {
    public class SimpleWorld : IWorld {
        public List<IVolume> Volumes { get; set; }
        
        public List<ILightSource> LightSources { get; set; }        

        public SimpleWorld() {
            Volumes = new List<IVolume>();
            LightSources = new List<ILightSource>();
        }
    }
}
