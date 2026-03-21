using DevoidEngine.Engine.Content.Scenes;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidStandaloneLauncher.Prototypes;

namespace DevoidStandaloneLauncher
{
    public class PrototypeLoader : Layer
    {
        private float splashTimer = 0;
        private float splashDuration = 0.5f;
        private bool prototypeLoaded = false;

        internal Scene CurrentScene = SplashScene.CreateSplashScene(nameof(MeshLoaderPrototype));
        internal Prototype GamePrototype = new MeshLoaderPrototype();

        public override void OnAttach()
        {
            SceneManager.LoadScene(CurrentScene);
            CurrentScene.Play();
        }

        public void LoadPrototype()
        {
            GamePrototype.loader = this;
            GamePrototype.OnInit();
        }


        public override void OnUpdate(float deltaTime)
        {
            CurrentScene.OnUpdate(deltaTime);
            if (prototypeLoaded)
                GamePrototype.OnUpdate(deltaTime);

            if (splashTimer > splashDuration && !prototypeLoaded)
            {
                LoadPrototype();
                prototypeLoaded = true;
                return;
            }
            splashTimer += deltaTime;
        }

        public override void OnRender(float deltaTime, float alpha)
        {
            CurrentScene.OnRender(deltaTime, alpha);
            if (prototypeLoaded)
            {
                GamePrototype.OnRender(deltaTime, alpha);
            }

        }

        public override void OnLateRender()
        {
            if (prototypeLoaded)
            {
                GamePrototype.OnLateRender();
            }
            Texture2D renderOutput = CurrentScene?.GetMainCamera()?.Camera?.RenderTarget?.GetRenderTexture(0);
            RenderAPI.RenderToScreen(renderOutput);
        }

        public override void OnResize(int width, int height)
        {
            Renderer.Resize(width, height);

            Screen.Size = new System.Numerics.Vector2(width, height);
            Renderer.graphicsDevice.SetViewport(0, 0, width, height);

            CurrentScene?.OnResize(width, height);
            if (prototypeLoaded)
            {
                GamePrototype.Resize(width, height);
            }
        }

        #region INPUT_SETTERS
        public override void OnMouseMove(MouseMoveEvent e)
        {
            Input.OnMouseMove(e);
        }

        public override void OnMouseButton(MouseButtonEvent e)
        {
            Input.OnMouseButton(e);
        }

        public override void OnMouseWheel(MouseWheelEvent e)
        {
            Input.OnMouseWheel(e);
        }

        public override void OnKeyDown(KeyboardEvent e)
        {
            Input.OnKeyDown(e.Key);
        }

        public override void OnKeyUp(KeyboardEvent e)
        {
            Input.OnKeyUp(e.Key);
        }
        #endregion
    }
}