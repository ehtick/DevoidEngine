using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    public static class Input
    {
        // ===============================
        // Mouse State
        // ===============================

        public static Vector2 MousePosition { get; private set; } = Vector2.Zero;
        public static Vector2 MouseDelta { get; private set; } = Vector2.Zero;
        public static Vector2 MouseScrollDelta { get; private set; } = Vector2.Zero;

        private static HashSet<MouseButton> mouseDown = new();
        private static HashSet<MouseButton> mousePressedThisFrame = new();
        private static HashSet<MouseButton> mouseReleasedThisFrame = new();

        public static bool IsDragging { get; private set; }

        // ===============================
        // Mouse Events
        // ===============================

        public static void OnMouseInput(MouseButtonEvent e)
        {
            if (e.IsPressed)
            {
                if (!mouseDown.Contains(e.Button))
                {
                    mouseDown.Add(e.Button);
                    mousePressedThisFrame.Add(e.Button);
                }
            }
            else
            {
                if (mouseDown.Contains(e.Button))
                {
                    mouseDown.Remove(e.Button);
                    mouseReleasedThisFrame.Add(e.Button);
                }
            }
        }

        public static void OnMouseMove(MouseMoveEvent e)
        {
            MousePosition = e.position;

            // Accumulate delta for the frame
            MouseDelta += e.delta;

            IsDragging = mouseDown.Count > 0;
        }

        public static void OnMouseWheel(MouseWheelEvent e)
        {
            MouseScrollDelta += e.Offset;
        }

        // ===============================
        // Mouse Queries
        // ===============================

        public static bool GetMouse(MouseButton btn)
            => mouseDown.Contains(btn);

        public static bool GetMouseDown(MouseButton btn)
            => mousePressedThisFrame.Contains(btn);

        public static bool GetMouseUp(MouseButton btn)
            => mouseReleasedThisFrame.Contains(btn);

        // ===============================
        // Keyboard State
        // ===============================

        private static HashSet<Keys> keysDown = new();
        private static HashSet<Keys> keysPressedThisFrame = new();
        private static HashSet<Keys> keysReleasedThisFrame = new();

        public static void OnKeyDown(Keys key)
        {
            if (!keysDown.Contains(key))
            {
                keysDown.Add(key);
                keysPressedThisFrame.Add(key);
            }
        }

        public static void OnKeyUp(Keys key)
        {
            if (keysDown.Contains(key))
            {
                keysDown.Remove(key);
                keysReleasedThisFrame.Add(key);
            }
        }

        // ===============================
        // Keyboard Queries
        // ===============================

        public static bool GetKey(Keys key)
            => keysDown.Contains(key);

        public static bool GetKeyDown(Keys key)
            => keysPressedThisFrame.Contains(key);

        public static bool GetKeyUp(Keys key)
            => keysReleasedThisFrame.Contains(key);

        // ===============================
        // FPS Convenience Abstractions
        // ===============================

        public static Vector2 MoveAxis
        {
            get
            {
                Vector2 axis = Vector2.Zero;

                if (GetKey(Keys.W)) axis.Y += 1;
                if (GetKey(Keys.S)) axis.Y -= 1;
                if (GetKey(Keys.D)) axis.X += 1;
                if (GetKey(Keys.A)) axis.X -= 1;

                if (axis.LengthSquared() > 1f)
                    axis = Vector2.Normalize(axis);

                return axis;
            }
        }

        public static bool JumpPressed =>
            GetKeyDown(Keys.Space);

        // ===============================
        // Frame Reset
        // ===============================

        /// <summary>
        /// Call once at the START of every frame.
        /// </summary>
        public static void Update()
        {
            MouseDelta = Vector2.Zero;
            MouseScrollDelta = Vector2.Zero;

            keysPressedThisFrame.Clear();
            keysReleasedThisFrame.Clear();

            mousePressedThisFrame.Clear();
            mouseReleasedThisFrame.Clear();
        }
    }
}
