using System.Numerics;

namespace DevoidEngine.Engine.Utilities
{
    public class BoundingBox
    {
        public static BoundingBox Empty => new BoundingBox(Vector3.Zero, Vector3.Zero);

        public Vector3 min;
        public Vector3 max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public BoundingBox Transform(Matrix4x4 worldMatrix)
        {
            // Compute center and half-extents
            Vector3 center = (min + max) * 0.5f;
            Vector3 extents = (max - min) * 0.5f;

            // Transform center
            Vector3 newCenter = Vector3.Transform(center, worldMatrix);

            // Extract absolute rotation/scale from the matrix
            Vector3 newExtents = new Vector3(
                Math.Abs(worldMatrix.M11) * extents.X + Math.Abs(worldMatrix.M21) * extents.Y + Math.Abs(worldMatrix.M31) * extents.Z,
                Math.Abs(worldMatrix.M12) * extents.X + Math.Abs(worldMatrix.M22) * extents.Y + Math.Abs(worldMatrix.M32) * extents.Z,
                Math.Abs(worldMatrix.M13) * extents.X + Math.Abs(worldMatrix.M23) * extents.Y + Math.Abs(worldMatrix.M33) * extents.Z
            );

            return new BoundingBox(newCenter - newExtents, newCenter + newExtents);
        }

    }
}
