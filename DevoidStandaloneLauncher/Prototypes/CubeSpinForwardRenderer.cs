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

        public override void OnInit(Scene main)
        {
            this.scene = main;

            camera = scene.addGameObject("Camera");
            CameraComponent3D cameraComponent = camera.AddComponent<CameraComponent3D>();
            camera.transform.Position = new System.Numerics.Vector3(0, 0, 0);

            cameraComponent.IsDefault = true;

            Mesh mesh = new Mesh();
            mesh.SetVertices(Primitives.GetCubeVertex());

            cube = scene.addGameObject("Cube");
            cube.transform.Position = new System.Numerics.Vector3(10, 0, 0);
            MeshRenderer renderer = cube.AddComponent<MeshRenderer>();

            renderer.AddMesh(mesh);
        }

        public override void OnUpdate(float delta)
        {
            //cube.transform.Rotation = new System.Numerics.Vector3(cube.transform.Rotation.X + delta * 10, cube.transform.Rotation.Y + delta, cube.transform.Rotation.Z + delta);
        }

    }
}
