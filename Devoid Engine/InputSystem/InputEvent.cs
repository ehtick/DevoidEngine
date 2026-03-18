using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem
{
    public struct InputEvent
    {
        public uint DeviceId;     // 0 = keyboard, 1 = mouse, 2+ = gamepads
        public ushort Control;   // control index
        public float Value;      // ALWAYS float
    }
}
