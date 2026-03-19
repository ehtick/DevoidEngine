using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class StaticCollider : Component
    {
        public override string Type => nameof(StaticCollider);

        private PhysicsShapeDescription internalShape = new PhysicsShapeDescription
        {
            Type = PhysicsShapeType.Box,
            Size = new Vector3(1, 1, 1)
        };

        public PhysicsShapeDescription Shape
        {
            get => internalShape;
            set
            {
                internalShape = value;
                CreateStatic();
            }
        }

        private PhysicsMaterial internalMaterial = PhysicsMaterial.Default;

        public PhysicsMaterial Material
        {
            get => internalMaterial;
            set
            {
                internalMaterial = value;
                CreateStatic();
            }
        }

        private IPhysicsStatic internalStatic;

        private GameObject _debugObject;

        public bool DebugDraw = false;

        public override void OnStart()
        {
            CreateStatic();
        }

        //private void CreateDebugVisual()
        //{
        //    if (Shape.Type != PhysicsShapeType.Box)
        //        return;

        //    // Create child object
        //    _debugObject = gameObject.Scene.addGameObject("StaticCollider_Debug");

        //    _debugObject.transform.Position = gameObject.transform.Position;
        //    _debugObject.transform.Rotation = gameObject.transform.Rotation;
        //    _debugObject.transform.Scale = Shape.Size;

        //    // Create cube mesh
        //    Mesh cubeMesh = new Mesh();
        //    cubeMesh.SetVertices(Primitives.GetCubeVertex());

        //    // Add MeshRenderer
        //    var meshRenderer = _debugObject.AddComponent<MeshRenderer>();
        //    meshRenderer.AddMesh(cubeMesh);

        //    // Optional: assign debug material if needed
        //    // meshRenderer.SetMaterial(DebugMaterial);
        //}

        private void CreateStatic()
        {
            if (internalStatic != null)
            {
                gameObject.Scene.Physics.RemoveStatic(internalStatic);
                internalStatic = null;
            }

            var desc = new PhysicsStaticDescription
            {
                Position = gameObject.transform.Position,
                Rotation = gameObject.transform.Rotation,
                Shape = internalShape,
                Material = internalMaterial
            };

            internalStatic = gameObject.Scene.Physics.CreateStatic(desc, gameObject);
        }

        public override void OnRender(float dt)
        {
            //Matrix4x4 world = gameObject.transform.WorldMatrix;

            Matrix4x4 model = RenderBase.BuildModel(
                gameObject.transform.Position,
                Shape.Size,
                gameObject.transform.Rotation
            );

            //DebugRenderSystem.DrawCube(model);
            DebugRenderSystem.DrawMesh(debugView, gameObject.transform.WorldMatrix);
        }

        Mesh debugView;
        public void DebugVisualization(Mesh mesh)
        {
            debugView = mesh;
        }

        public override void OnUpdate(float dt)
        {
            if (_debugObject == null)
                return;

            // Keep debug object synced with collider transform
            _debugObject.transform.Position = gameObject.transform.Position;
            _debugObject.transform.Rotation = gameObject.transform.Rotation;
        }

        public override void OnDestroy()
        {
            if (_debugObject != null)
            {
                gameObject.Scene.Destroy(_debugObject);
                _debugObject = null;
            }
        }
    }
}