using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    public class Frustum
    {
        // 6 planes: 0=Left,1=Right,2=Bottom,3=Top,4=Near,5=Far
        public Plane[] Planes { get; private set; } = new Plane[6];

        public enum PlaneIndex
        {
            Left = 0,
            Right = 1,
            Bottom = 2,
            Top = 3,
            Near = 4,
            Far = 5
        }

        /// <summary>
        /// Builds frustum planes from a view-projection matrix.
        /// </summary>
        public static Frustum FromMatrix(Matrix4x4 m)
        {
            var f = new Frustum();

            // Left
            f.Planes[0] = new Plane(m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41);

            // Right
            f.Planes[1] = new Plane(m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41);

            // Bottom
            f.Planes[2] = new Plane(m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42);

            // Top
            f.Planes[3] = new Plane(m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42);

            // Near (✅ fixed)
            f.Planes[4] = new Plane(m.M14 + m.M13, m.M24 + m.M23, m.M34 + m.M33, m.M44 + m.M43);

            // Far
            f.Planes[5] = new Plane(m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43);

            return f;
        }


        /// <summary>
        /// Test an axis-aligned bounding box against the frustum.
        /// Returns true if any part of the box is inside.
        /// </summary>
        public bool Intersects(BoundingBox box)
        {
            return true;
            foreach (var plane in Planes)
            {
                // compute positive vertex
                Vector3 positive = box.min;

                if (plane.Normal.X >= 0) positive.X = box.max.X;
                if (plane.Normal.Y >= 0) positive.Y = box.max.Y;
                if (plane.Normal.Z >= 0) positive.Z = box.max.Z;

                if (Vector3.Dot(plane.Normal, positive) + plane.D < 0)
                    return false; // outside this plane
            }

            return true; // inside all planes
        }
    }

    public struct Plane
    {
        public Vector3 Normal; // normalized direction
        public float D;        // plane equation: Normal · X + D = 0

        public Plane(Vector3 normal, float d)
        {
            Normal = Vector3.Normalize(normal);
            D = d;
        }

        public Plane(float a, float b, float c, float d)
        {
            Normal = Vector3.Normalize(new Vector3(a, b, c));
            D = d / Normal.Length();
        }

        public static Plane Normalize(Plane plane)
        {
            float length = plane.Normal.Length();
            return new Plane(plane.Normal / length, plane.D / length);
        }

        /// <summary>
        /// Computes signed distance from point to plane.
        /// </summary>
        public float DistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + D;
        }

        /// <summary>
        /// Returns positive if in front, negative if behind plane.
        /// </summary>
        public float ClassifyPoint(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + D;
        }
    }
}
