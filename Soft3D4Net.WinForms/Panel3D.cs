using Soft3D4Net.Painter;
using Soft3D4Net.Renderers;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Text;

namespace Soft3D4Net.WinForms {
    public partial class Panel3D : UserControl {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene { get; set; }

        Stats Stats { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RendererSettings RendererSettings {
            get => Renderer.Settings;
            set => Renderer.Settings = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPainter Painter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRenderer Renderer { get; set; }

        public Panel3D() {
            InitializeComponent();

            Renderer = new Renderer { Settings = new RendererSettings { BackFaceCulling = true } };

            this.Stats = Renderer.Stats;
            Painter = new GouraudPainter();

            this.ResizeRedraw = true;

            StatDisplay = new StringBuilder();

            this.Paint += Panel3D_Paint;
        }

        const string Format = "Volumes:{0}\nTriangles:{1} - Back:{2} - Out:{3} - Behind:{4}\nPixels:{9} drawn:{5} - Z behind:{6}\nCalc time:{7} - Paint time:{8}";

        StringBuilder StatDisplay;

        Size oldSize;
        Bitmap bmp;

        void Panel3D_Paint(object sender, PaintEventArgs e) {

            if(Scene == null)
                return;

            var clip = e.ClipRectangle;

            if(clip.Size != oldSize) {
                Scene.Surface = new FrameBuffer(clip.Width, clip.Height) { Stats = Stats };

                bmp?.Dispose();
                bmp = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height, PixelFormat.Format32bppPArgb);

                oldSize = clip.Size;
            }

            var g = e.Graphics;

            // g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            // g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            // g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            Renderer.Render(Scene, Painter);
            ImageUtils.FillBitmap(bmp, Scene.Surface.Screen);

            g.DrawImage(bmp, Point.Empty);

            StatDisplay.Clear();
            StatDisplay.AppendFormat(Format,
                Scene.World.Volumes.Count,
                Stats.TotalTriangleCount,
                Stats.FacingBackTriangleCount,
                Stats.OutOfViewTriangleCount,
                Stats.BehindViewTriangleCount,
                Stats.DrawnPixelCount,
                Stats.BehindZPixelCount,
                Stats.CalculationTimeMs,
                Stats.PainterTimeMs,
                Stats.DrawnPixelCount + Stats.BehindZPixelCount
            );

            TextRenderer.DrawText(g, StatDisplay.ToString(), this.Font, Point.Empty, Color.BlueViolet, this.BackColor, TextFormatFlags.ExpandTabs);
        }
    }
}
