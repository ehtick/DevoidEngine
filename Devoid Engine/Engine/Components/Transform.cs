using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class Transform : Component
    {
        public override string Type => nameof(Transform);

        // ===============================
        // Hierarchy
        // ===============================

        private Transform parent;
        private readonly List<Transform> children = new();

        public Transform Parent => parent;
        public IReadOnlyList<Transform> Children => children;

        public bool Interpolated
        {
            get; private set;
        } = true;

        // ===============================
        // Local Transform
        // ===============================

        private Vector3 localPosition = Vector3.Zero;
        private Quaternion localRotation = Quaternion.Identity;
        private Vector3 localScale = Vector3.One;

        // ===============================

        private Vector3 prevLocalPosition;
        private Quaternion prevLocalRotation;
        private Vector3 prevLocalScale;
        private Matrix4x4 prevWorldMatrix;

        private Vector3 prevWorldScale;
        private Quaternion prevWorldRotation;
        private Vector3 prevWorldPosition;
        // ===============================

        // yes i like these designs.

        // ===============================
        // Cached World
        // ===============================

        private Matrix4x4 worldMatrix = Matrix4x4.Identity;
        private bool dirty = true;

        public bool hasMoved = false;

        private Matrix4x4 interpolatedWorldMatrix;
        private int interpolatedFrame = -1; // cache guard

        // ===============================
        // Local Properties
        // ===============================

        public Vector3 LocalPosition
        {
            get => localPosition;
            set
            {
                if (localPosition != value)
                {
                    localPosition = value;
                    MarkDirty();
                }
            }
        }

        public Quaternion LocalRotation
        {
            get => localRotation;
            set
            {
                if (localRotation != value)
                {
                    localRotation = value;
                    MarkDirty();
                }
            }
        }

        public Vector3 LocalScale
        {
            get => localScale;
            set
            {
                if (localScale != value)
                {
                    localScale = value;
                    MarkDirty();
                }
            }
        }

        // ===============================
        // World Matrix
        // ===============================

        public Matrix4x4 LocalMatrix =>
            Matrix4x4.CreateScale(localScale) *
            Matrix4x4.CreateFromQuaternion(localRotation) *
            Matrix4x4.CreateTranslation(localPosition);

        public Matrix4x4 WorldMatrix
        {
            get
            {
                if (dirty)
                    RecalculateWorldMatrix();

                return worldMatrix;
            }
        }

        private void RecalculateWorldMatrix()
        {
            if (parent != null)
                worldMatrix = LocalMatrix * parent.WorldMatrix;
            else
                worldMatrix = LocalMatrix;

            dirty = false;
        }

        private void MarkDirty()
        {
            dirty = true;
            hasMoved = true;
            RecalculateWorldMatrix();

            foreach (var child in children)
                child.MarkDirty();
        }

        // ===============================
        // World Space Properties
        // ===============================

        public Vector3 Position
        {
            get => WorldMatrix.Translation;
            set
            {
                if (parent != null)
                {
                    Matrix4x4.Invert(parent.WorldMatrix, out var invParent);
                    Vector3 local = Vector3.Transform(value, invParent);
                    localPosition = local;
                }
                else
                {
                    localPosition = value;
                }

                MarkDirty();
            }
        }

        public Quaternion Rotation
        {
            get
            {
                if (parent == null)
                    return localRotation;

                return parent.Rotation * localRotation;
            }
            set
            {
                if (parent != null)
                {
                    Quaternion invParent = Quaternion.Inverse(parent.Rotation);
                    localRotation = invParent * value;
                }
                else
                {
                    localRotation = value;
                }

                MarkDirty();
            }
        }

        public Vector3 Scale
        {
            get
            {
                if (parent == null)
                    return localScale;

                return parent.Scale * localScale;
            }
            set
            {
                if (parent != null)
                    localScale = value / parent.Scale;
                else
                    localScale = value;

                MarkDirty();
            }
        }

        public Vector3 EulerAngles
        {
            get => TransformMath.QuaternionToEuler(Rotation);
            set => Rotation = TransformMath.EulerToQuaternion(value);
        }

        public Vector3 Forward
        {
            get => Vector3.Normalize(
                Vector3.Transform(Vector3.UnitZ, Rotation)
            );
        }

        public Vector3 Right
        {
            get => Vector3.Normalize(
                Vector3.Normalize(Vector3.Cross(Forward, Up))
            );
        }

        public Vector3 Up
        {
            get => Vector3.Normalize(
                Vector3.Transform(Vector3.UnitY, Rotation)
            );
        }

        // ===============================
        // Parenting
        // ===============================

        public void SetParent(Transform newParent, bool keepWorld = false)
        {
            if (parent == newParent)
                return;

            Matrix4x4 oldWorld = WorldMatrix;

            parent?.children.Remove(this);

            parent = newParent;
            parent?.children.Add(this);

            if (keepWorld)
            {
                if (parent != null)
                {
                    Matrix4x4.Invert(parent.WorldMatrix, out var invParent);
                    Matrix4x4 local = oldWorld * invParent;
                    Decompose(local);
                }
                else
                {
                    Decompose(oldWorld);
                }
            }

            MarkDirty();
        }

        public void CapturePrevious()
        {
            prevLocalPosition = localPosition;
            prevLocalRotation = localRotation;
            prevLocalScale = localScale;

            prevWorldMatrix = WorldMatrix;

            Matrix4x4.Decompose(prevWorldMatrix,
                out prevWorldScale,
                out prevWorldRotation,
                out prevWorldPosition);
        }

        public Matrix4x4 GetGlobalTransformInterpolated(int frameIndex)
        {

            float alpha = EngineSingleton.Instance.InterpolationAlpha;

            if (interpolatedFrame == frameIndex)
                return interpolatedWorldMatrix;

            Matrix4x4 local;

            if (Interpolated)
            {
                Vector3 pos = Vector3.Lerp(prevLocalPosition, localPosition, alpha);
                Quaternion rot = Quaternion.Slerp(prevLocalRotation, localRotation, alpha);
                Vector3 scale = Vector3.Lerp(prevLocalScale, localScale, alpha);

                local = Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(rot) * Matrix4x4.CreateTranslation(pos);
            }
            else
            {
                local = LocalMatrix;
            }
            Matrix4x4 result;

            // i hate life
            if (parent != null)
            {
                Matrix4x4 parentGlobal;

                if (parent.interpolatedFrame == frameIndex)
                {
                    parentGlobal = parent.interpolatedWorldMatrix;
                }
                else
                {
                    parentGlobal = parent.GetGlobalTransformInterpolated(frameIndex);
                }

                result = local * parentGlobal;
            }
            else
            {
                result = local;
            }

            interpolatedWorldMatrix = result;
            interpolatedFrame = frameIndex;

            return result;
        }

        public TransformData GetSnapshot()
        {
            // current world
            Matrix4x4.Decompose(
                WorldMatrix,
                out Vector3 currScale,
                out Quaternion currRot,
                out Vector3 currPos
            );

            return new TransformData
            {
                PrevPos = prevWorldPosition,
                PrevRot = prevWorldRotation,
                PrevScale = prevWorldScale,

                CurrPos = currPos,
                CurrRot = currRot,
                CurrScale = currScale
            };
        }

        private void Decompose(Matrix4x4 matrix)
        {
            Matrix4x4.Decompose(
                matrix,
                out Vector3 scale,
                out Quaternion rotation,
                out Vector3 translation
            );

            localPosition = translation;
            localRotation = rotation;
            localScale = scale;
        }
    }
}