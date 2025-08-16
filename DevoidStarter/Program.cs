using DevoidEngine.Engine.Core;
using DevoidGPU.DX11;

namespace DevoidStarter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationSpecification applicationSpecification = new ApplicationSpecification()
            {
                Height = 500,
                Width = 500,
                forceVsync = true,
                Name = "Devoid D3D11",
            
                graphicsDevice = new DX11GraphicsDevice()
            };


            Application application  = new Application();
            application.Create(applicationSpecification);
            application.Run();
        }
    }
}
