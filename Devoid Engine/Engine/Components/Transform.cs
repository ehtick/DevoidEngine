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
        } = false;

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
        }

        public Matrix4x4 GetGlobalTransformInterpolated(int frameIndex)
        {

            float alpha = EngineSingleton.Instance.InterpolationAlpha;

            // 2. Cached result
            if (interpolatedFrame == frameIndex)
                return interpolatedWorldMatrix;

            // 3. Compute local interpolated
            Matrix4x4 local;

            if (Interpolated)
            {
                Vector3 pos = Vector3.Lerp(prevLocalPosition, localPosition, alpha);
                Quaternion rot = Quaternion.Slerp(prevLocalRotation, localRotation, alpha);
                Vector3 scale = Vector3.Lerp(prevLocalScale, localScale, alpha);

                local =
                    Matrix4x4.CreateScale(scale) *
                    Matrix4x4.CreateFromQuaternion(rot) *
                    Matrix4x4.CreateTranslation(pos);
            }
            else
            {
                local = LocalMatrix;
            }

            // 4. Combine with parent (recursive + cached)
            Matrix4x4 result;

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

            // 5. Cache
            interpolatedWorldMatrix = result;
            interpolatedFrame = frameIndex;

            return result;
        }

        public (TransformData, TransformData) GetSnapshot()
        {
            TransformData prevData = new TransformData()
            {
                Position = prevLocalPosition,
                Rotation = prevLocalRotation,
                Scale = prevLocalScale,
            };

            TransformData currData = new TransformData()
            {
                Position = LocalPosition,
                Rotation = LocalRotation,
                Scale = LocalScale,
            };

            return (prevData, currData);
        }

        public override void OnUpdate(float dt)
        {
            CapturePrevious();
        }

        // ===============================
        // Decomposition Helper
        // ===============================

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