using DevoidEngine.InputSystem.InputDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem
{
    public static class Input
    {
        public static InputBackend Backend = new();
        public static InputRouter Router = new();
        public static InputState State = new();
        public static InputMap Map = new();


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
