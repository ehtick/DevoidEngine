using System;
using DevoidEngine.Engine.Core;
using Elemental.Editor;
using DevoidGPU.DX11;

namespace Elemental
{
    public class ElementalEditor
    {
        public void Create()
        {
            ApplicationSpecification applicationSpecification = new ApplicationSpecification()
            {
                Height = 500,
                Width = 500,
                forceVsync = true,
                Name = "Devoid D3D11",

                graphicsDevice = new DX11GraphicsDevice()
            };


            Application application = new Application();
            application.Create(applicationSpecification);
            application.AddLayer(new EditorLayer());
            application.Run();
        }

    }
}
