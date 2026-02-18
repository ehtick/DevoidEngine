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
        GameObject camera;

        public override void OnInit(Scene main)
        {
            this.scene = main;

            // ===============================
            // PLAYER ROOT (Capsule Physics)
            // ===============================

            GameObject player = scene.addGameObject("Player");

            // Ground top = 0.5 (height 1 centered at 0)
            // Capsule half total height = 1.5
            // So center should be 2.0
            player.transform.Position = new Vector3(0, 2.0f, 0);

            var playerBody = player.AddComponent<RigidBodyComponent>();
            playerBody.Shape = new PhysicsShapeDescription()
            {
                Type = PhysicsShapeType.Capsule,
                Height = 2f,
                Radius = 0.5f
            };

            playerBody.Material = new PhysicsMaterial()
            {
                Friction = 0.8f,
                Restitution = 0f,
                AngularDamping = 10f
            };

            // ===============================
            // FPS CONTROLLER
            // ===============================

            var fps = player.AddComponent<FPSController>();
            fps.MoveSpeed = 6f;
            fps.JumpForce = 100f;
            fps.MouseSensitivity = 0.15f;

            // ===============================
            // CAMERA PIVOT (Pitch Only)
            // ===============================

            GameObject cameraPivot = scene.addGameObject("CameraPivot");
            cameraPivot.SetParent(player, false);

            cameraPivot.transform.LocalPosition = new Vector3(0, 1.4f, 0);

            // ===============================
            // CAMERA
            // ===============================

            camera = scene.addGameObject("Camera");
            camera.SetParent(cameraPivot, false);
            camera.transform.LocalPosition = Vector3.Zero;

            var camComponent = camera.AddComponent<CameraComponent3D>();
            camComponent.IsDefault = true;

            // ===============================
            // LIGHT
            // ===============================

            GameObject light = scene.addGameObject("Light");
            var lightComponent = light.AddComponent<LightComponent>();
            lightComponent.Intensity = 100;
            lightComponent.Color = new Vector4(1, 1, 1, 1);
            light.transform.Position = new Vector3(0, 5, 0);

            // ===============================
            // GROUND
            // ===============================

            Mesh mesh = new Mesh();
            mesh.SetVertices(Primitives.GetCubeVertex());

            GameObject ground = scene.addGameObject("Ground");
            ground.transform.Position = new Vector3(0, 0, 0);
            ground.transform.Scale = new Vector3(20, 1, 20);

            var groundRenderer = ground.AddComponent<MeshRenderer>();
            groundRenderer.AddMesh(mesh);

            var groundCollider = ground.AddComponent<StaticCollider>();
            groundCollider.Shape = new PhysicsShapeDescription()
            {
                Type = PhysicsShapeType.Box,
                Size = new Vector3(20, 1, 20)
            };

            groundCollider.Material = new PhysicsMaterial()
            {
                Friction = 1f
            };
        }

        public override void OnUpdate(float delta)
        {
            // Nothing needed here now.
            // FPSController handles input + movement.
        }
    }
}
