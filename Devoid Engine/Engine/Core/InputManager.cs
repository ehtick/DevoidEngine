//using DevoidEngine.Engine.Utilities;
//using System.Numerics;

//namespace DevoidEngine.Engine.Core
//{
//    public class InputBuffer
//    {
//        public Vector2 MousePosition;
//        public Vector2 MouseDelta;
//        public Vector2 MouseScroll;

//        public HashSet<Keys> KeysDown;
//        public HashSet<Keys> KeysPressed;
//        public HashSet<Keys> KeysReleased;

//        public HashSet<MouseButton> MouseDown;
//        public HashSet<MouseButton> MousePressed;
//        public HashSet<MouseButton> MouseReleased;
//    }

//    public static class InputManager
//    {
//        private static readonly AsyncDoubleBuffer<InputBuffer> _buffer;

//        static InputManager()
//        {
//            _buffer = new AsyncDoubleBuffer<InputBuffer>(
//                CreateBuffer(),
//                CreateBuffer()
//            );
//        }

//        private static InputBuffer CreateBuffer()
//        {
//            return new InputBuffer
//            {
//                KeysDown = new(),
//                KeysPressed = new(),
//                KeysReleased = new(),
//                MouseDown = new(),
//                MousePressed = new(),
//                MouseReleased = new()
//            };
//        }

//        // ============ RENDER THREAD WRITES HERE ============

//        public static void OnMouseMove(MouseMoveEvent e)
//        {
//            var back = _buffer.Back;
//            back.MousePosition = e.position;
//            back.MouseDelta += e.delta;
//        }

//        public static void OnMouseWheel(MouseWheelEvent e)
//        {
//            var back = _buffer.Back;
//            back.MouseScroll += e.Offset;
//        }

//        public static void OnMouseButton(MouseButtonEvent e)
//        {
//            var back = _buffer.Back;

//            if (e.IsPressed)
//            {
//                back.MouseDown.Add(e.Button);
//                back.MousePressed.Add(e.Button);
//            }
//            else
//            {
//                back.MouseDown.Remove(e.Button);
//                back.MouseReleased.Add(e.Button);
//            }
//        }

//        public static void OnKeyDown(Keys key)
//        {
//            var back = _buffer.Back;

//            if (!back.KeysDown.Contains(key))
//            {
//                back.KeysDown.Add(key);
//                back.KeysPressed.Add(key);
//            }
//        }

//        public static void OnKeyUp(Keys key)
//        {
//            var back = _buffer.Back;

//            if (back.KeysDown.Contains(key))
//            {
//                back.KeysDown.Remove(key);
//                back.KeysReleased.Add(key);
//            }
//        }

//        // ============ CALLED AT END OF RENDER FRAME ============

//        public static void Publish()
//        {
//            _buffer.Publish();

//            // reset new back buffer per-frame data
//            var back = _buffer.Back;
//            back.MouseDelta = Vector2.Zero;
//            back.MouseScroll = Vector2.Zero;
//            back.KeysPressed.Clear();
//            back.KeysReleased.Clear();
//            back.MousePressed.Clear();
//            back.MouseReleased.Clear();
//        }

//        // ============ CALLED AT START OF UPDATE FRAME ============

//        public static void CommitToInput()
//        {
//            var front = _buffer.Front;

//            Input.MousePosition = front.MousePosition;
//            Input.MouseDelta = front.MouseDelta;
//            Input.MouseScrollDelta = front.MouseScroll;

//            Input.SetKeys(front.KeysDown, front.KeysPressed, front.KeysReleased);
//            Input.SetMouseButtons(front.MouseDown, front.MousePressed, front.MouseReleased);
//        }
//    }
//}
