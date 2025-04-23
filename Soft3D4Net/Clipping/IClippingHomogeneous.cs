using System.Numerics;

namespace Soft3D4Net {
    public interface IClippingHomogeneous {
        bool Clip(ref Vector4 begin, ref Vector4 end);
    }
}