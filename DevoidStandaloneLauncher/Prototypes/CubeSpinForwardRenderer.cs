using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidStandaloneLauncher.Prototypes
{
    internal class CubeSpinForwardRenderer : Prototype
    {
        Scene scene;
        GameObject cube;
        GameObject cube2;
        GameObject cube3;
        GameObject camera;
        GameObject light;

        public override void OnInit(Scene main)
        {
            this.scene = main;

            // ===============================
            // PLAYER ROOT (Physics Body)
            // ===============================

            GameObject player = scene.addGameObject("Player");
            player.transform.Position = new Vector3(0, 5, 0);

            RigidBodyComponent playerBody = player.AddComponent<RigidBodyComponent>();
            playerBody.Shape = new PhysicsShapeDescription()
            {
                Type = PhysicsShapeType.Capsule,
                Height = 2f,
                Radius = 0.5f
            };

            // Lock tipping
            playerBody.Material = new PhysicsMaterial()
            {
                Friction = 0.8f,
                Restitution = 0f,
                AngularDamping = 10f
            };

            // ===============================
            // CAMERA PIVOT (Pitch Only)
            // ===============================

            GameObject cameraPivot = scene.addGameObject("CameraPivot");
            cameraPivot.transform.LocalPosition = new Vector3(0, 2f, 0);
            cameraPivot.SetParent(player);

            // ===============================
            // CAMERA (Actual Camera)
            // ===============================

            camera = scene.addGameObject("Camera");
            camera.SetParent(cameraPivot);

            CameraComponent3D cameraComponent =
                camera.AddComponent<CameraComponent3D>();

            cameraComponent.IsDefault = true;

            // ===============================
            // LIGHT
            // ===============================

            light = scene.addGameObject("Light");
            LightComponent lightComponent = light.AddComponent<LightComponent>();
            lightComponent.Intensity = 100;
            lightComponent.Color = new Vector4(1, 1, 1, 1);
            light.transform.Position = new Vector3(0, 5, 0);

            // ===============================
            // MESH SETUP
            // ===============================

            Mesh mesh = new Mesh();
            mesh.SetVertices(Primitives.GetCubeVertex());

            // ===============================
            // BOUNCY CUBE
            // ===============================

            cube = scene.addGameObject("Bouncy Cube");
            cube.transform.Position = new Vector3(0, 1, 20);

            MeshRenderer renderer = cube.AddComponent<MeshRenderer>();
            renderer.AddMesh(mesh);

            RigidBodyComponent rigidbody = cube.AddComponent<RigidBodyComponent>();

            // ===============================
            // GROUND
            // ===============================

            cube2 = scene.addGameObject("Ground");
            cube2.transform.Position = new Vector3(0, -2, 0);
            cube2.transform.Scale = new Vector3(20, 1, 20);

            MeshRenderer renderer1 = cube2.AddComponent<MeshRenderer>();
            renderer1.AddMesh(mesh);

            StaticCollider rigidbody1 = cube2.AddComponent<StaticCollider>();
            rigidbody1.Shape = new PhysicsShapeDescription()
            {
                Type = PhysicsShapeType.Box,
                Size = new Vector3(20, 1, 20)
            };

            rigidbody1.Material = new PhysicsMaterial()
            {
                Friction = 1.0f
            };
        }


        public override void OnUpdate(float delta)
        {
            scene.Physics.Raycast(new Ray(Vector3.Zero, new Vector3(10, 0, 0)), 100, out RaycastHit hit);

            if (hit.HitObject != null)
            {
                Console.WriteLine(hit.HitObject.Name);
            }
        }

    }
}
