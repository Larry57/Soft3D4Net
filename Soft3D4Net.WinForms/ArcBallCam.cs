using Soft3D4Net.Utils;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Soft3D4Net.WinForms {

    // Must adapt moving

    public class ArcBallCam : ICamera {

        Point oldMousePosition;

        Vector3 oldCameraPosition;
        Quaternion oldCameraRotation;

        public Quaternion Rotation { get; set; }
        public Vector3 Position { get; set; }

        readonly float Radius = 0.3f;

        public Matrix4x4 ViewMatrix => Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);

        float yCoeff = 10f;

        public ArcBallCam(Control control) {
            Rotation = Quaternion.Identity;
            Control = control;
        }

        Control control;

        public Control Control {
            get => control;
            set {
                var oldControl = control;

                if(PropertyChangedHelper.ChangeValue(ref control, value)) {

                    if(oldControl != null) {
                        oldControl.MouseDown -= control_MouseDown;
                        oldControl.MouseMove -= control_MouseMove;
                        control.MouseUp -= Control_MouseUp;
                        control.MouseWheel -= Control_MouseWheel;
                    }

                    if(control != null) {
                        control.MouseDown += control_MouseDown;
                        control.MouseMove += control_MouseMove;
                        control.MouseUp += Control_MouseUp;
                        control.MouseWheel += Control_MouseWheel;
                    }
                }
            }
        }

        private void Control_MouseWheel(object sender, MouseEventArgs e) {
            var delta = Math.Sign(e.Delta);
            Debug.WriteLine(delta);
            Position += new Vector3(0, 0, delta / 2.5f);
            control.Invalidate();
        }

        private void Control_MouseUp(object sender, MouseEventArgs e) {
            left = false;
            right = false;
            control.Cursor = Cursors.Default;
        }

        bool right;
        bool left;

        private void control_MouseDown(object sender, MouseEventArgs e) {

            left = Control.MouseButtons.HasFlag(MouseButtons.Left);
            right = Control.MouseButtons.HasFlag(MouseButtons.Right);

            oldMousePosition = e.Location;

            if(left && right) {
                oldCameraPosition = Position;
                control.Cursor = Cursors.SizeNS;
            }
            else if(left) {
                oldCameraRotation = Rotation;
                control.Cursor = Cursors.NoMove2D;
            }
            else if(right) {
                oldCameraPosition = Position;
                control.Cursor = Cursors.SizeAll;
            }
        }

        void control_MouseMove(object sender, MouseEventArgs e) {
            if(left && right) {
                var deltaY = oldMousePosition.Y - e.Location.Y;
                Debug.WriteLine(deltaY);
                Position = oldCameraPosition + new Vector3(0, 0, deltaY / yCoeff);
            }
            else if(left) {
                var oldNpc = control.NormalizePointClient(oldMousePosition);
                var oldVector = MapToSphere(oldNpc);

                var curNpc = control.NormalizePointClient(e.Location);
                var curVector = MapToSphere(curNpc);

                var deltaRotation = CalculateQuaternion(oldVector, curVector);
                Rotation = deltaRotation * oldCameraRotation;
            }
            else if(right) {
                var deltaPosition = new Vector3(new Vector2(e.Location.X, e.Location.Y) - new Vector2(oldMousePosition.X, oldMousePosition.Y), 0);
                Position = oldCameraPosition + (deltaPosition * new Vector3(1, -1, 1)) / 100;
            }

            if(left || right)
                control.Invalidate();
        }

        public Vector3 MapToSphere(Vector2 v) {

            var P = new Vector3(v.X, -v.Y, 0);

            var XY_squared = P.LengthSquared();
            var radius_squared = Radius * Radius;

            if(XY_squared <= .5f * radius_squared)
                P.Z = (float)Math.Sqrt(radius_squared - XY_squared);  // Pythagore
            else
                P.Z = 0.5f * radius_squared / P.Length();  // Hyperboloid

            return Vector3.Normalize(P);
        }

        public Quaternion CalculateQuaternion(Vector3 startV, Vector3 currentV) {

            var cross = Vector3.Cross(startV, currentV);

            if(cross.Length() > MathUtils.Epsilon)
                return new Quaternion(cross, Vector3.Dot(startV, currentV));
            else
                return Quaternion.Identity;
        }
    }
}
