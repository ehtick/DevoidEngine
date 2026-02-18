using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    public static class Input
    {
        // ============================================================
        // BACK BUFFER (Written by Render Thread Only)
        // ============================================================

        private static Vector2 _backMouseDelta;
        private static Vector2 _backMouseScroll;
        private static readonly object _lock = new();

        private static readonly HashSet<Keys> _backKeysDown = new();
        private static readonly HashSet<MouseButton> _backMouseDown = new();

        // ============================================================
        // FRONT BUFFER (Read by Update Thread Only)
        // ============================================================

        public static Vector2 MouseDelta { get; private set; }
        public static Vector2 MouseScrollDelta { get; private set; }

        private static readonly HashSet<Keys> _keysDown = new();
        private static readonly HashSet<Keys> _keysPressedThisFrame = new();
        private static readonly HashSet<Keys> _keysReleasedThisFrame = new();

        private static readonly HashSet<MouseButton> _mouseDown = new();
        private static readonly HashSet<MouseButton> _mousePressedThisFrame = new();
        private static readonly HashSet<MouseButton> _mouseReleasedThisFrame = new();

        // ============================================================
        // RENDER THREAD EVENTS
        // ============================================================

        public static void OnMouseMove(MouseMoveEvent e)
        {
            // DO NOT ACCUMULATE
            _backMouseDelta = e.delta;
        }

        public static void OnMouseWheel(MouseWheelEvent e)
        {
            _backMouseScroll = e.Offset;
        }

        public static void OnMouseButton(MouseButtonEvent e)
        {
            lock (_lock)
            {
                if (e.IsPressed)
                    _backMouseDown.Add(e.Button);
                else
                    _backMouseDown.Remove(e.Button);
            }
        }

        public static void OnKeyDown(Keys key)
        {
            lock (_lock)
            {
                _backKeysDown.Add(key);
            }
        }

        public static void OnKeyUp(Keys key)
        {
            lock (_lock)
            {
                _backKeysDown.Remove(key);
            }
        }

        // ============================================================
        // CALLED ONCE AT START OF UPDATE FRAME
        // ============================================================

        public static void Update()
        {
            // --- Copy mouse delta snapshot ---
            MouseDelta = _backMouseDelta;
            MouseScrollDelta = _backMouseScroll;

            // Clear back buffer AFTER snapshot
            _backMouseDelta = Vector2.Zero;
            _backMouseScroll = Vector2.Zero;

            lock (_lock)
            {
                // --- Keys ---
                _keysPressedThisFrame.Clear();
                _keysReleasedThisFrame.Clear();

                foreach (var key in _backKeysDown)
                {
                    if (!_keysDown.Contains(key))
                        _keysPressedThisFrame.Add(key);
                }

                foreach (var key in _keysDown)
                {
                    if (!_backKeysDown.Contains(key))
                        _keysReleasedThisFrame.Add(key);
                }

                _keysDown.Clear();
                foreach (var key in _backKeysDown)
                    _keysDown.Add(key);

                // --- Mouse buttons ---
                _mousePressedThisFrame.Clear();
                _mouseReleasedThisFrame.Clear();

                foreach (var btn in _backMouseDown)
                {
                    if (!_mouseDown.Contains(btn))
                        _mousePressedThisFrame.Add(btn);
                }

                foreach (var btn in _mouseDown)
                {
                    if (!_backMouseDown.Contains(btn))
                        _mouseReleasedThisFrame.Add(btn);
                }

                _mouseDown.Clear();
                foreach (var btn in _backMouseDown)
                    _mouseDown.Add(btn);
            }
        }

        // ============================================================
        // GAME QUERIES
        // ============================================================

        public static bool GetKey(Keys key) => _keysDown.Contains(key);
        public static bool GetKeyDown(Keys key) => _keysPressedThisFrame.Contains(key);
        public static bool GetKeyUp(Keys key) => _keysReleasedThisFrame.Contains(key);

        public static bool GetMouse(MouseButton btn) => _mouseDown.Contains(btn);
        public static bool GetMouseDown(MouseButton btn) => _mousePressedThisFrame.Contains(btn);
        public static bool GetMouseUp(MouseButton btn) => _mouseReleasedThisFrame.Contains(btn);

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
