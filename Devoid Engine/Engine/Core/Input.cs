using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    /// <summary>
    /// Game-facing input API.
    /// This class is updated ONLY by InputManager on the update thread.
    /// Game code reads from here.
    /// </summary>
    public static class Input
    {
        // ===============================
        // Mouse State (Read-Only to Game)
        // ===============================

        public static Vector2 MousePosition { get; internal set; } = Vector2.Zero;
        public static Vector2 MouseDelta { get; internal set; } = Vector2.Zero;
        public static Vector2 MouseScrollDelta { get; internal set; } = Vector2.Zero;

        private static readonly HashSet<MouseButton> mouseDown = new();
        private static readonly HashSet<MouseButton> mousePressedThisFrame = new();
        private static readonly HashSet<MouseButton> mouseReleasedThisFrame = new();

        public static bool IsDragging => mouseDown.Count > 0;

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
        // Keyboard State (Read-Only to Game)
        // ===============================

        private static readonly HashSet<Keys> keysDown = new();
        private static readonly HashSet<Keys> keysPressedThisFrame = new();
        private static readonly HashSet<Keys> keysReleasedThisFrame = new();

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
        // FPS Convenience
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
        // Called by InputManager ONLY
        // ===============================

        internal static void SetKeys(
            HashSet<Keys> down,
            HashSet<Keys> pressed,
            HashSet<Keys> released)
        {
            keysDown.Clear();
            foreach (var k in down)
                keysDown.Add(k);

            keysPressedThisFrame.Clear();
            foreach (var k in pressed)
                keysPressedThisFrame.Add(k);

            keysReleasedThisFrame.Clear();
            foreach (var k in released)
                keysReleasedThisFrame.Add(k);
        }

        internal static void SetMouseButtons(
            HashSet<MouseButton> down,
            HashSet<MouseButton> pressed,
            HashSet<MouseButton> released)
        {
            mouseDown.Clear();
            foreach (var b in down)
                mouseDown.Add(b);

            mousePressedThisFrame.Clear();
            foreach (var b in pressed)
                mousePressedThisFrame.Add(b);

            mouseReleasedThisFrame.Clear();
            foreach (var b in released)
                mouseReleasedThisFrame.Add(b);
        }

        // ===============================
        // Frame Reset (Update Thread)
        // ===============================

        /// <summary>
        /// Call once at the END of update frame.
        /// Clears per-frame transient input only.
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
