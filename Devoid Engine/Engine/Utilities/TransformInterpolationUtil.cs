using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    public static class TransformInterpolationUtil
    {
        public static TransformData Interpolate(
            in TransformData prev,
            in TransformData curr,
            float alpha)
        {
            TransformData result;

            // Position / Scale
            result.Position = Vector3.Lerp(prev.Position, curr.Position, alpha);
            result.Scale = Vector3.Lerp(prev.Scale, curr.Scale, alpha);

            // Rotation
            Quaternion toRot = curr.Rotation;

            float dot = Quaternion.Dot(prev.Rotation, toRot);

            if (dot < 0f)
            {
                toRot = -toRot;
                dot = -dot;
            }

            result.Rotation = (dot > 0.9995f)
                ? Quaternion.Normalize(Quaternion.Lerp(prev.Rotation, toRot, alpha))
                : QuaternionSlerp(prev.Rotation, toRot, alpha);

            return result;
        }
        static Quaternion QuaternionSlerp(Quaternion a, Quaternion b, float t)
        {
            float cos = Quaternion.Dot(a, b);

            if (cos < 0f)
            {
                b = -b;
                cos = -cos;
            }

            if (1f - cos > 1e-6f)
            {
                float omega = MathF.Acos(cos);
                float sin = MathF.Sin(omega);

                float s0 = MathF.Sin((1 - t) * omega) / sin;
                float s1 = MathF.Sin(t * omega) / sin;

                return (a * s0) + (b * s1);
            }

            return Quaternion.Normalize(a * (1 - t) + b * t);
        }

        static void DecomposeBasis(in Matrix4x4 m, out Quaternion rot, out Vector3 scale)
        {
            Vector3 x = GetColumn(m, 0);
            Vector3 y = GetColumn(m, 1);
            Vector3 z = GetColumn(m, 2);

            scale = new Vector3(x.Length(), y.Length(), z.Length());

            if (scale.X != 0) x /= scale.X;
            if (scale.Y != 0) y /= scale.Y;
            if (scale.Z != 0) z /= scale.Z;

            Matrix4x4 rotM = new Matrix4x4(
                x.X, x.Y, x.Z, 0,
                y.X, y.Y, y.Z, 0,
                z.X, z.Y, z.Z, 0,
                0, 0, 0, 1
            );

            rot = Quaternion.CreateFromRotationMatrix(rotM);
        }

        static Matrix4x4 ToMatrix(in TransformData t)
        {
            return Matrix4x4.CreateScale(t.Scale)
                 * Matrix4x4.CreateFromQuaternion(t.Rotation);
        }

        static Vector3 GetColumn(in Matrix4x4 m, int i)
        {
            return i switch
            {
                0 => new Vector3(m.M11, m.M12, m.M13),
                1 => new Vector3(m.M21, m.M22, m.M23),
                2 => new Vector3(m.M31, m.M32, m.M33),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        static void SetColumn(ref Matrix4x4 m, int i, Vector3 v)
        {
            switch (i)
            {
                case 0: m.M11 = v.X; m.M12 = v.Y; m.M13 = v.Z; break;
                case 1: m.M21 = v.X; m.M22 = v.Y; m.M23 = v.Z; break;
                case 2: m.M31 = v.X; m.M32 = v.Y; m.M33 = v.Z; break;
            }
        }

    }
}
