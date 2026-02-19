using System.Numerics;
using System.Threading;

namespace DevoidEngine.Engine.Core
{
    public static class Input
    {
        // ===============================
        // RAW THREAD-SAFE ACCUMULATORS
        // (Written by Render Thread)
        // ===============================

        private static float _rawMouseDeltaX;
        private static float _rawMouseDeltaY;

        private static float _rawScrollX;
        private static float _rawScrollY;

        private static readonly object _lock = new();

        private static readonly HashSet<Keys> _rawKeysDown = new();
        private static readonly HashSet<MouseButton> _rawMouseDown = new();

        // ===============================
        // GAME-FACING STATE
        // (Read by Update Thread)
        // ===============================

        public static Vector2 MousePosition { get; private set; }
        public static Vector2 MouseDelta { get; private set; }
        public static Vector2 MouseScrollDelta { get; private set; }

        private static readonly HashSet<Keys> keysDown = new();
        private static readonly HashSet<Keys> keysPressedThisFrame = new();
        private static readonly HashSet<Keys> keysReleasedThisFrame = new();

        private static readonly HashSet<MouseButton> mouseDown = new();
        private static readonly HashSet<MouseButton> mousePressedThisFrame = new();
        private static readonly HashSet<MouseButton> mouseReleasedThisFrame = new();

        // ===============================
        // RENDER THREAD CALLS THESE
        // ===============================

        private static void AtomicAdd(ref float target, float value)
        {
            float initial, computed;
            do
            {
                initial = target;
                computed = initial + value;
            }
            while (Interlocked.CompareExchange(ref target, computed, initial) != initial);
        }


        public static void OnMouseMove(MouseMoveEvent e)
        {
            MousePosition = e.position;

            AtomicAdd(ref _rawMouseDeltaX, e.delta.X);
            AtomicAdd(ref _rawMouseDeltaY, e.delta.Y);

        }

        public static void OnMouseWheel(MouseWheelEvent e)
        {

            AtomicAdd(ref _rawScrollX, e.Offset.X);
            AtomicAdd(ref _rawScrollY, e.Offset.Y);

        }

        public static void OnMouseButton(MouseButtonEvent e)
        {
            lock (_lock)
            {
                if (e.IsPressed)
                {
                    _rawMouseDown.Add(e.Button);
                }
                else
                {
                    _rawMouseDown.Remove(e.Button);
                }
            }
        }

        public static void OnKeyDown(Keys key)
        {
            lock (_lock)
            {
                _rawKeysDown.Add(key);
            }
        }

        public static void OnKeyUp(Keys key)
        {
            lock (_lock)
            {
                _rawKeysDown.Remove(key);
            }
        }

        // ===============================
        // UPDATE THREAD CALLS THIS
        // ===============================

        /// <summary>
        /// Call ONCE at start of update frame.
        /// </summary>
        public static void Update()
        {


            // Pull atomic deltas
            float dx = Interlocked.Exchange(ref _rawMouseDeltaX, 0f);
            float dy = Interlocked.Exchange(ref _rawMouseDeltaY, 0f);
            MouseDelta = new Vector2(dx, dy);

            float sx = Interlocked.Exchange(ref _rawScrollX, 0f);
            float sy = Interlocked.Exchange(ref _rawScrollY, 0f);
            MouseScrollDelta = new Vector2(sx, sy);

            // Update key states
            lock (_lock)
            {
                keysPressedThisFrame.Clear();
                keysReleasedThisFrame.Clear();

                // Check newly pressed
                foreach (var key in _rawKeysDown)
                {
                    if (!keysDown.Contains(key))
                        keysPressedThisFrame.Add(key);
                }

                // Check released
                foreach (var key in keysDown)
                {
                    if (!_rawKeysDown.Contains(key))
                        keysReleasedThisFrame.Add(key);
                }

                keysDown.Clear();
                foreach (var key in _rawKeysDown)
                    keysDown.Add(key);

                // Same for mouse
                mousePressedThisFrame.Clear();
                mouseReleasedThisFrame.Clear();

                foreach (var btn in _rawMouseDown)
                {
                    if (!mouseDown.Contains(btn))
                        mousePressedThisFrame.Add(btn);
                }

                foreach (var btn in mouseDown)
                {
                    if (!_rawMouseDown.Contains(btn))
                        mouseReleasedThisFrame.Add(btn);
                }

                mouseDown.Clear();
                foreach (var btn in _rawMouseDown)
                    mouseDown.Add(btn);
            }
        }

        // ===============================
        // GAME QUERIES
        // ===============================

        public static bool GetKey(Keys key) => keysDown.Contains(key);
        public static bool GetKeyDown(Keys key) => keysPressedThisFrame.Contains(key);
        public static bool GetKeyUp(Keys key) => keysReleasedThisFrame.Contains(key);

        public static bool GetMouse(MouseButton btn) => mouseDown.Contains(btn);
        public static bool GetMouseDown(MouseButton btn) => mousePressedThisFrame.Contains(btn);
        public static bool GetMouseUp(MouseButton btn) => mouseReleasedThisFrame.Contains(btn);

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

        public static bool JumpPressed => GetKeyDown(Keys.Space);
    }
}
