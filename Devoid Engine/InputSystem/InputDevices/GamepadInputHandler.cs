using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem.InputDevices
{
    internal class GamepadInputHandler : InputDeviceHandler
    {
        private IReadOnlyList<JoystickState> _states;
        private Func<IReadOnlyList<JoystickState>> getter;

        public GamepadInputHandler(Func<IReadOnlyList<JoystickState>> getter)
        {
            this.getter = getter;
            _states = getter();
        }

        public override void Register(InputBackend backend)
        {

        }

        public override void Update(InputBackend backend)
        {
            var states = _states;

            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];

                if (state == null)
                    continue;

                if (state.AxisCount == 0 && state.ButtonCount == 0)
                    continue;

                Console.WriteLine($"Joystick {i} connected: {state.Name}");
                for (int j = 0; j < state.ButtonCount; j++)
                {

                    Console.WriteLine(state.IsButtonDown(j));
                }

                for (int j = 0; j < state.AxisCount; j++)
                {
                    Console.WriteLine(state.GetAxis(j).ToString());
                }
            }
        }
    }
}
