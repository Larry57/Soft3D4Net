using System.Diagnostics;

namespace Soft3D4Net {

    public class Stats {
        internal Stats() {
        }

        public int TotalTriangleCount { get; internal set; }
        public int DrawnTriangleCount { get; internal set; }
        public int FacingBackTriangleCount { get; internal set; }
        public int OutOfViewTriangleCount { get; internal set; }
        public int BehindViewTriangleCount { get; internal set; } 
        public int DrawnPixelCount { get; internal set; }
        public int BehindZPixelCount { get; internal set; }

        public long CalculationTimeMs { get => calcSw.ElapsedMilliseconds; }
        public long PainterTimeMs { get => paintSw.ElapsedMilliseconds; }

        readonly Stopwatch calcSw = new Stopwatch();
        readonly Stopwatch paintSw = new Stopwatch();

        public void PaintTime() {
            calcSw.Stop();
            paintSw.Start();
        }

        public void CalcTime() {
            paintSw.Stop();
            calcSw.Start();
        }

        public void StopTime() {
            paintSw.Stop();
            calcSw.Stop();
        }

        public void Clear() {
            paintSw.Reset();
            calcSw.Reset();
            TotalTriangleCount = 0;
            DrawnTriangleCount = 0;
            FacingBackTriangleCount = 0;
            OutOfViewTriangleCount = 0;
            BehindViewTriangleCount = 0;
            DrawnPixelCount = 0;
            BehindZPixelCount = 0;
        }
    }
}
