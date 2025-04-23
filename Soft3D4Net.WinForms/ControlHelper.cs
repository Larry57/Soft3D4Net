using System.Drawing;
using System.Numerics;

namespace Soft3D4Net.WinForms {
    static class ControlHelper {

        public static Vector2 NormalizePointClient(this Control control, Point position) => new Vector2(position.X * (2f / control.Width) - 1.0f, position.Y * (2f / control.Height) - 1.0f);
    }
}
