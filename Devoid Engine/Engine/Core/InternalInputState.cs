using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public static class InternalInputState
    {
        // Mouse
        public static Vector2 MousePosition { get; internal set; }
        public static Vector2 MouseDelta { get; internal set; }
        public static Vector2 MouseScroll { get; internal set; }
        public static Vector2 PreviousMouseScroll { get; internal set; }
        public static bool[] MouseButtons { get; } = new bool[5]; // Left, Right, Middle, X1, X2

        // Keyboard
        private static HashSet<Keys> keysDown = new();
        private static HashSet<Keys> keysPressedThisFrame = new();
        private static HashSet<Keys> keysReleasedThisFrame = new();

        // Text input
        public static List<char> PressedChars { get; } = new();

        // --- Methods called by backend ---
        public static void SetMousePosition(Vector2 pos) => MousePosition = pos;
        public static void SetMouseDelta(Vector2 delta) => MouseDelta = delta;
        public static void SetMouseScroll(Vector2 scrollDelta)
        {
            MouseScroll = scrollDelta; // this is already a delta
        }

        public static void SetMouseButton(int index, bool down)
        {
            if (index >= 0 && index < MouseButtons.Length)
                MouseButtons[index] = down;
        }

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

        public static void AddInputChar(char c) => PressedChars.Add(c);

        // --- Query API ---
        public static bool GetKey(Keys key) => keysDown.Contains(key);
        public static bool GetKeyDown(Keys key) => keysPressedThisFrame.Contains(key);
        public static bool GetKeyUp(Keys key) => keysReleasedThisFrame.Contains(key);

        public static bool GetMouseButton(int index)
        {
            if (index >= 0 && index < MouseButtons.Length)
                return MouseButtons[index];
            return false;
        }

        public static void UpdateFrame()
        {
            MouseDelta = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            keysPressedThisFrame.Clear();
            keysReleasedThisFrame.Clear();
            PressedChars.Clear();
        }
    }
}
