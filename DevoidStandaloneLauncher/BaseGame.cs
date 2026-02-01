using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using DevoidStandaloneLauncher.Prototypes;

namespace DevoidStandaloneLauncher
{
    internal class BaseGame : Layer
    {
        Scene MainScene = new Scene();
        Prototype gamePrototype = new CubeSpinForwardRenderer();

        public override void OnAttach()
        {
            SceneManager.LoadScene(MainScene);

            gamePrototype.OnInit(MainScene);

            MainScene.Play();
        }

        public override void OnUpdate(float deltaTime)
        {
            gamePrototype.OnUpdate(deltaTime);
        }

        public override void OnRender(float deltaTime)
        {
            gamePrototype.OnRender(deltaTime);
        }

        public override void OnLateRender()
        {
            if (SceneManager.MainScene.GetMainCamera() == null) return;
            Texture2D renderOutput = SceneManager.MainScene.GetMainCamera().Camera.RenderTarget.GetRenderTexture(0);
            
            RenderAPI.RenderToScreen(renderOutput);
        }

        public override void OnResize(int width, int height)
        {
            Console.WriteLine(width);
            Renderer.Resize(width, height);
            Renderer.graphicsDevice.SetViewport(0, 0, width, height);
            MainScene.OnResize(width, height);
        }
    }
}
