using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidStandaloneLauncher.Prototypes
{
    internal class UITester : Prototype
    {
        Scene scene;
        GameObject canvas;
        GameObject camera;

        Mesh testRender;
        MaterialNew testRenderMat;
        MaterialInstanceNew testRenderMatInstance;

        public override void OnInit(Scene main)
        {
            this.scene = main;

            //UIButton button = new UIButton();
            //button.Setup();

            canvas = scene.addGameObject("Canvas");
            CanvasComponent canvasComponent = canvas.AddComponent<CanvasComponent>();


            camera = scene.addGameObject("Camera");
            CameraComponent3D cameraComponent = camera.AddComponent<CameraComponent3D>();
            camera.transform.Position = new System.Numerics.Vector3(0, 0, 0);

            testRenderMat = new MaterialNew(new Shader("Engine/Content/Shaders/Testing/basic"));
            testRenderMat.SetVector4("color", new Vector4(1, 69, 1, 1));
            testRenderMat.SetVector4("Fail", new Vector4(1));

            testRenderMatInstance = new MaterialInstanceNew(testRenderMat);
            testRenderMatInstance.SetVector4("Fail", new Vector4(1, 420, 1, 1));

            testRender = new Mesh();
            testRender.SetVertices(Primitives.GetQuadVertex());
        }

        public override void OnUpdate(float delta)
        {
            //cube.transform.Rotation = new System.Numerics.Vector3(cube.transform.Rotation.X + delta * 10, cube.transform.Rotation.Y + delta, cube.transform.Rotation.Z + delta);
        }

        public override void OnRender(float delta)
        {
            testRender.VertexBuffer.Bind();
            Renderer.GetInputLayout(testRender, testRenderMat.Shader).Bind();

            testRenderMatInstance.Bind();

            Renderer.graphicsDevice.Draw(testRender.VertexBuffer.VertexCount, 0);
        }

    }
}
