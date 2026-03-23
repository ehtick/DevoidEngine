using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DevoidEngine.Engine.InputSystem.InputDevices
{
    internal class MouseInputHandler : InputDeviceHandler
    {
        private MouseState _state;
        private uint _deviceId = 0;

        private MouseButton[] _buttons;
        private HashSet<MouseButton> _previousButtons = new();

        private Vector2 _previousPosition;
        private Vector2 _previousScroll;
        public MouseInputHandler(MouseState state)
        {
            _state = state;
            _buttons = Enum.GetValues<MouseButton>();
            _previousPosition = state.Position;
        }

        public override void Register(InputBackend backend)
        {
            InputDeviceLayout layout = new InputDeviceLayout
            {
                Name = "Mouse0",
                DeviceType = InputDeviceType.Mouse
            };

            _deviceId = backend.InputDeviceRegistry.RegisterDevice(layout);

            _previousScroll = _state.Scroll;
        }

        public override void Update(InputBackend backend)
        {
            // -------------------------
            // BUTTONS (discrete events)
            // -------------------------
            foreach (MouseButton button in _buttons)
            {
                bool isDown = _state.IsButtonDown((OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)button);
                bool wasDown = _previousButtons.Contains(button);

                if (isDown && !wasDown)
                {
                    backend.Emit(new InputEvent
                    {
                        DeviceId = _deviceId,
                        DeviceType = InputDeviceType.Mouse,
                        Control = (ushort)button,
                        Value = 1f
                    });

                    _previousButtons.Add(button);
                }
                else if (!isDown && wasDown)
                {
                    backend.Emit(new InputEvent
                    {
                        DeviceId = _deviceId,
                        DeviceType = InputDeviceType.Mouse,
                        Control = (ushort)button,
                        Value = 0f
                    });

                    _previousButtons.Remove(button);
                }
            }

            // -------------------------
            // POSITION / DELTA
            // -------------------------
            Vector2 currentPos = _state.Position;
            Vector2 delta = currentPos - _previousPosition;

            backend.Emit(new InputEvent
            {
                DeviceId = _deviceId,
                DeviceType = InputDeviceType.Mouse,
                Control = (ushort)MouseAxis.DeltaX,
                Value = _state.Delta.X
            });

            backend.Emit(new InputEvent
            {
                DeviceId = _deviceId,
                DeviceType = InputDeviceType.Mouse,
                Control = (ushort)MouseAxis.DeltaY,
                Value = _state.Delta.Y
            });

            Vector2 pos = _state.Position;

            backend.Emit(new InputEvent
            {
                DeviceId = _deviceId,
                DeviceType = InputDeviceType.Mouse,
                Control = (ushort)MouseAxis.X,
                Value = pos.X
            });

            backend.Emit(new InputEvent
            {
                DeviceId = _deviceId,
                DeviceType = InputDeviceType.Mouse,
                Control = (ushort)MouseAxis.Y,
                Value = pos.Y
            });


            _previousPosition = currentPos;

            // -------------------------
            // SCROLL
            // -------------------------
            Vector2 scroll = _state.Scroll;
            Vector2 scrollDelta = scroll - _previousScroll;

            if (scrollDelta.X != 0f)
            {
                backend.Emit(new InputEvent
                {
                    DeviceId = _deviceId,
                    DeviceType = InputDeviceType.Mouse,
                    Control = (ushort)MouseAxis.ScrollX,
                    Value = scrollDelta.X
                });
            }

            if (scrollDelta.Y != 0f)
            {
                backend.Emit(new InputEvent
                {
                    DeviceId = _deviceId,
                    DeviceType = InputDeviceType.Mouse,
                    Control = (ushort)MouseAxis.ScrollY,
                    Value = scrollDelta.Y
                });
            }

            _previousScroll = scroll;
        }
    }
}