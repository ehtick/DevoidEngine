using System;
using System.Collections.Generic;
using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Core
{
    enum SupportedGraphicsBackends
    {
        DX11,
        OpenGL,
        OpenGLES
    }

    public struct ApplicationSpecification
    {
        public string Name;
        public int Width, Height;
        public bool forceVsync;
        public bool useImGui;
        public bool useImGuiDock;
        public bool useFullscreen;

        public bool useCustomTitlebar;
        public string customTitlebarLogo;
        public bool darkTitlebar;

        public IGraphicsDevice graphicsDevice;
    }

    public class Application
    {
        public WindowManager windowManager;
        public Window MainWindow;

        public IGraphicsDevice graphicsDevice;

        public LayerHandler LayerHandler;
        public ApplicationSpecification AppSpec;

        public void Create(ApplicationSpecification appSpec)
        {
            this.AppSpec = appSpec;

            // Main Window
            WindowSpecification windowSpecification = new WindowSpecification()
            {
                StartVisible = false,
                StartFocused = true,
                WindowSize = new System.Numerics.Vector2(appSpec.Width, appSpec.Height),
                WindowTitle = appSpec.Name
            };

            PresentationParameters presentationParameters = new PresentationParameters()
            {
                BackBufferWidth = appSpec.Width,
                BackBufferHeight = appSpec.Height,
                RefreshRate = 60,
                VSync = appSpec.forceVsync,
                BufferCount = 2,
                ColorFormat = TextureFormat.RGBA8_UNorm,
                Windowed = true
            };

            LayerHandler = new LayerHandler();


            windowManager = new WindowManager();
            MainWindow = new Window(windowSpecification);

            graphicsDevice = appSpec.graphicsDevice;
            graphicsDevice.Initialize(MainWindow.GetWindowPtr(), presentationParameters);
            Renderer.Initialize(graphicsDevice);
            ExampleRenderer.Initialize();

            MainWindow.OnLoad += OnLoad;
            MainWindow.OnUpdateFrame += OnUpdateFrame;
            MainWindow.OnRenderFrame += OnRenderFrame;

            MainWindow.KeyDown += OnKeyDown;
            MainWindow.KeyUp += OnKeyUp;

            MainWindow.Resize += OnResize;
            MainWindow.TextInput += OnTextInput;

            MainWindow.MouseMove += OnMouseMove;
            MainWindow.MouseDown += OnMouseInput;
            MainWindow.MouseEnter += OnMouseEnter;
            MainWindow.MouseLeave += OnMouseLeave;
            MainWindow.MouseUp += OnMouseInput;
            MainWindow.MouseWheel += OnMouseWheel;

            windowManager.RegisterWindow(MainWindow);
        }

        private void OnMouseMove(OpenTK.Windowing.Common.MouseMoveEventArgs obj)
        {
            LayerHandler.OnMouseMoveEvent(obj);
        }

        private void OnMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj)
        {
            LayerHandler.OnMouseWheelEvent(obj);
        }

        private void OnMouseLeave()
        {

        }

        private void OnMouseEnter()
        {

        }

        private void OnMouseInput(OpenTK.Windowing.Common.MouseButtonEventArgs obj)
        {
            LayerHandler.OnMouseButtonEvent(obj);
        }

        private void OnTextInput(OpenTK.Windowing.Common.TextInputEventArgs obj)
        {
            LayerHandler.TextInput(obj.Unicode);
        }

        private void OnResize(OpenTK.Windowing.Common.ResizeEventArgs obj)
        {
            LayerHandler.ResizeLayers(obj.Width, obj.Height);
        }

        private void OnLoad()
        {
            LayerHandler.AttachLayers();
        }

        private void OnKeyDown(OpenTK.Windowing.Common.KeyboardKeyEventArgs obj)
        {
        
        }

        private void OnKeyUp(OpenTK.Windowing.Common.KeyboardKeyEventArgs obj)
        {
        }

        public void Run()
        {
            windowManager.RunAll();
        }

        public void AddLayer(Layer layer)
        {
            LayerHandler.AddLayer(layer);
        }

        private void OnRenderFrame()
        {
            ExampleRenderer.Render();
            LayerHandler.RenderLayers();
            graphicsDevice.MainSurface.Present();
        }

        private void OnUpdateFrame(double deltaTime)
        {
            LayerHandler.UpdateLayers((float)deltaTime);
        }
    }
}
