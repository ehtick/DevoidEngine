using DevoidEngine.Engine.Core;
using DevoidGPU.DX11;
using Elemental.Editor;

namespace Elemental
{
    public class ElementalEditor
    {
        public void Create()
        {
            ApplicationSpecification applicationSpecification = new ApplicationSpecification()
            {
                Width = (int)(1920 / 1.5f),
                Height = (int)(1080 / 1.5f),
                forceVsync = true,
                Name = "Devoid D3D11",

                graphicsDevice = new DX11GraphicsDevice()
            };


            Application application = new Application();
            application.Create(applicationSpecification);

            EditorLayer editor = new EditorLayer();
            editor.application = application;

            application.AddLayer(editor);
            application.Run();
        }

    }
}
