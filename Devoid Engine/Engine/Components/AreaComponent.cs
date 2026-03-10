using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Rendering;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class AreaComponent : Component, ICollisionListener
    {
        public override string Type => nameof(AreaComponent);


        // ===============================
        // Settings
        // ===============================

        public PhysicsShapeDescription Shape
        {
            get => internalShape;
            set
            {
                internalShape = value;
                CreateBody();
            }
        }

        public PhysicsMaterial Material = PhysicsMaterial.Default;

        public bool AllowSleep
        {
            get => allowSleep;
            set
            {
                allowSleep = value;
                CreateBody();
            }
        }

        // ===============================
        // Internal Physics Handle
        // ===============================

        private IPhysicsBody internalBody;

        private PhysicsShapeDescription internalShape = new PhysicsShapeDescription
        {
            Type = PhysicsShapeType.Box,
            Size = new Vector3(1, 1, 1)
        };

        private bool allowSleep = false;

        // ===============================
        // Events
        // ===============================

        public event Action<GameObject> OnEnter;
        public event Action<GameObject> OnExit;

        // ===============================
        // Lifecycle
        // ===============================

        public override void OnStart()
        {
            CreateBody();
        }

        private void CreateBody()
        {
            if (internalBody != null)
            {
                gameObject.Scene.Physics.RemoveBody(internalBody);
            }

            if (internalShape.Type == PhysicsShapeType.Box && internalShape.Size == Vector3.Zero)
            {
                internalShape.Size = new Vector3(1, 1, 1);
            }

            var desc = new PhysicsBodyDescription
            {
                Position = gameObject.transform.Position,
                Rotation = gameObject.transform.Rotation,
                Mass = 0f,
                IsKinematic = true,
                IsTrigger = true,
                Shape = internalShape,
                Material = Material,
                AllowSleep = allowSleep
            };

            internalBody = gameObject.Scene.Physics.CreateBody(desc, gameObject);
        }

        public override void OnUpdate(float dt)
        {
            if (internalBody == null)
                return;

            internalBody.Position = gameObject.transform.Position;
            internalBody.Rotation = gameObject.transform.Rotation;
        }

        public override void OnRender(float dt)
        {
            Matrix4x4 model = RenderBase.BuildModel(
                gameObject.transform.Position,
                internalShape.Size,
                gameObject.transform.Rotation
            );

            DebugRenderSystem.DrawCube(model);
        }

        public override void OnDestroy()
        {
            internalBody?.Remove();
            internalBody = null;
        }

        // ===============================
        // Collision callbacks
        // ===============================

        public void OnCollisionEnter(GameObject other)
        {
            OnEnter?.Invoke(other);
        }

        public void OnCollisionExit(GameObject other)
        {
            OnExit?.Invoke(other);
        }

        public void OnCollisionStay(GameObject other) { }
    }
}