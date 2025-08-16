using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    public struct KeyboardEvent
    {
        public Keys Key;

        public int scanCode;
        public string modifiers;

        public bool Caps;
    }

    public struct MouseMoveEvent
    {
        public Vector2 position;
        public Vector2 delta;
    }

    public struct MouseButtonEvent
    {
        public MouseButton Button;
        public InputAction Action;
        public KeyModifiers Modifiers;
        public bool IsPressed => Action != InputAction.Release;
    }

    public struct MouseWheelEvent
    {
        public Vector2 Offset; // this is equivalent to delta
    }

    public struct TextInputEvent
    {
        public int Unicode;
    }

    internal class InputUtils
    {
    }
}
