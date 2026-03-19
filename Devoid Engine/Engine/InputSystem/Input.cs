using DevoidEngine.Engine.Core;

namespace DevoidEngine.Engine.InputSystem
{
    public static class Input
    {
        public static InputBackend Backend = new();
        public static InputRouter Router = new();
        public static InputState State = new();
        public static InputMap Map = new();

        private static Window currentWindow;

        public static void Initialize(Window window)
        {
            currentWindow = window;
            Backend.AddInputDevice(new InputSystem.InputDevices.KeyboardInputHandler(currentWindow.KeyboardState));
            Backend.AddInputDevice(new InputSystem.InputDevices.GamepadInputHandler(currentWindow.JoystickStates));
        }

        public static void Update()
        {
            Backend.UpdateInput();
            Router.Route(Backend.GetEvents(), State);
        }

        public static void EndFrame()
        {
            State.EndFrame();
        }

        public static float GetAction(string action)
            => Map.Evaluate(action, State);

        public static bool GetActionDown(string action)
            => Map.EvaluateDown(action, State);
    }
}