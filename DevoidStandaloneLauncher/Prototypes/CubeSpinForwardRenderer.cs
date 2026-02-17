using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;

namespace DevoidStandaloneLauncher.Prototypes
{
    internal class CubeSpinForwardRenderer : Prototype
    {
        Scene scene;
        GameObject cube;
        GameObject camera;
        GameObject light;

        public override void OnInit(Scene main)
        {
            this.scene = main;

            camera = scene.addGameObject("Camera");
            CameraComponent3D cameraComponent = camera.AddComponent<CameraComponent3D>();
            camera.transform.Position = new System.Numerics.Vector3(0, 0, 0);

            cameraComponent.IsDefault = true;

            light = scene.addGameObject("light");
            LightComponent lightComponent = light.AddComponent<LightComponent>();
            lightComponent.Intensity = 100;
            lightComponent.Color = new System.Numerics.Vector4(1, 1, 1, 1);
            light.transform.Position = new System.Numerics.Vector3(0, 2, 0);


            // Build cube mesh and upload.

            Mesh mesh = new Mesh();
            mesh.SetVertices(Primitives.GetCubeVertex());

            cube = scene.addGameObject("Cube");
            cube.transform.Position = new System.Numerics.Vector3(0, 0, -10);
            MeshRenderer renderer = cube.AddComponent<MeshRenderer>();
            RigidBodyComponent rigidbody = cube.AddComponent<RigidBodyComponent>();

            renderer.AddMesh(mesh);
        }

        public override void OnUpdate(float delta)
        {
            //cube.transform.Rotation = new System.Numerics.Vector3(cube.transform.Rotation.X + delta * 10, cube.transform.Rotation.Y + delta, cube.transform.Rotation.Z + delta);
        }

    }
}
