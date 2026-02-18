using DevoidEngine.Engine.Core;
using DevoidGPU.DX11;


namespace DevoidStandaloneLauncher
{
    internal class Program
    {
        static ApplicationSpecification applicationOptions = new ApplicationSpecification()
        {
            Height = 200,
            Width = 200,
            graphicsDevice = new DX11GraphicsDevice(),
            forceVsync = false,

            Name = "Test Instance - Devoid"
        };


        static Application baseApplication;


        static void InitializeEngine()
        {
            baseApplication = new Application();
            baseApplication.Create(applicationOptions);
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Sandbox");
            InitializeEngine();

            baseApplication.MainWindow.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

            BaseGame baseGame = new BaseGame();
            baseApplication.AddLayer(baseGame);

            baseApplication.Run();

        }
    }
}
