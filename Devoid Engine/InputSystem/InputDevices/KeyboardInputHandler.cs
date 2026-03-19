using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem.InputDevices
{
    internal class KeyboardInputHandler : InputDeviceHandler
    {
        private KeyboardState _state;
        private uint _deviceId = 0;

        public KeyboardInputHandler(KeyboardState state)
        {
            _state = state;
            _deviceId = (uint)InputDeviceType.Keyboard;
        }

        public override void Register(InputBackend backend)
        {
            InputDeviceLayout layout = new InputDeviceLayout();
            layout.Name = "Keyboard0";
            layout.DeviceType = InputDeviceType.Keyboard;
        }

        public override void Update(InputBackend backend)
        {
            if (_state.IsKeyDown(Keys.Escape))
            {
                backend.Emit(new InputEvent()
                {
                    Control = (ushort)Keys.Escape,
                    DeviceId = _deviceId,
                    Value = 1
                });
            }
        }
    }
}
