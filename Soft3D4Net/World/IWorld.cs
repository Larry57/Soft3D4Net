using Soft3D4Net.Volume;
using System.Collections.Generic;

namespace Soft3D4Net.World {
    public interface IWorld {
        List<IVolume> Volumes { get; set; }
        List<ILightSource> LightSources { get; set; }
    }
}