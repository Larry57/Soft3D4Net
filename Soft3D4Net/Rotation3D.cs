using System;
using System.Numerics;

namespace Soft3D4Net {

    // Must replace with a Quaternion
    public readonly struct Rotation3D(float x, float y, float z) {

        const float PI_Deg = (float)Math.PI * 1f / 180f;

        public readonly float XPitch { get; } = x;
        public readonly float YYaw { get; } = y;

        public readonly float ZRoll { get; } = z;

        public readonly Rotation3D ToRad() => this * PI_Deg;

        public static bool operator ==(Rotation3D p, Rotation3D other) => other.XPitch == p.XPitch && other.YYaw == p.YYaw && other.ZRoll == p.ZRoll;

        public static bool operator !=(Rotation3D p, Rotation3D other) => !(other == p);

        public static Rotation3D operator +(Rotation3D p, Rotation3D other) => new Rotation3D(other.XPitch + p.XPitch, other.YYaw + p.YYaw, other.ZRoll + p.ZRoll);

        public static Rotation3D operator *(Rotation3D p, float a) => new Rotation3D(p.XPitch * a, p.YYaw * a, p.ZRoll * a);

        public static bool operator ==(Rotation3D p, Vector3 other) => other.X == p.XPitch && other.Y == p.YYaw && other.Z == p.ZRoll;
        public static bool operator !=(Rotation3D p, Vector3 other) => !(p == other);

        public override string ToString() {
            return $"<{XPitch}\t{YYaw}\t{ZRoll}>";
        }

        public override bool Equals(object obj) {
            return obj is Rotation3D d &&
                   XPitch == d.XPitch &&
                   YYaw == d.YYaw &&
                   ZRoll == d.ZRoll;
        }

        public override int GetHashCode() {
            return HashCode.Combine(XPitch, YYaw, ZRoll);
        }
    }
}
