using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
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
            MainScene.OnUpdate(deltaTime);
            Input.Update();
        }

        public override void OnRender(float deltaTime)
        {
            gamePrototype.OnRender(deltaTime);
        }

        public override void OnLateRender()
        {
            //if (SceneManager.MainScene.GetMainCamera() == null) return;
            Texture2D renderOutput = RenderBase.Output;//SceneManager.MainScene.GetMainCamera().Camera.RenderTarget.GetRenderTexture(0);

            RenderAPI.RenderToScreen(renderOutput);
        }

        public override void OnResize(int width, int height)
        {
            Renderer.Resize(width, height);

            Screen.Size = new System.Numerics.Vector2(width, height);
            Renderer.graphicsDevice.SetViewport(0, 0, width, height);
            MainScene.OnResize(width, height);
        }

        public override void OnMouseMove(MouseMoveEvent e)
        {
            Input.OnMouseMove(e);
        }

        public override void OnMouseButton(MouseButtonEvent e)
        {
            Input.OnMouseInput(e);
        }

        public override void OnMouseWheel(MouseWheelEvent e)
        {
            Input.OnMouseWheel(e);
        }

        public override void OnKeyDown(KeyboardEvent e)
        {
            Console.WriteLine(e.Key.ToString());
            Input.OnKeyDown(e.Key);
        }

        public override void OnKeyUp(KeyboardEvent e)
        {
            Input.OnKeyUp(e.Key);
        }

    }
}
