using System.Numerics;

namespace Soft3D4Net.Utils {

    // Some code from https://www.davrous.com/2013/06/13/tutorial-series-learning-how-to-write-a-3d-soft-engine-from-scratch-in-c-typescript-or-javascript/

    public static class MathUtils {

        public const float Epsilon = 1E-5f;              

        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        internal static float ComputeNDotL(Vector3 vertexCenter, Vector3 normal, Vector3 lightPosition) =>
            MathF.Max(0, Vector3.Dot(
                Vector3.Normalize(normal),
                Vector3.Normalize(lightPosition - vertexCenter)));
    }
}
