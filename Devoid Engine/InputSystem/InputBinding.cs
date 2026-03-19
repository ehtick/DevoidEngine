using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem
{
    public class InputBinding
    {
        public InputDeviceType DeviceType;
        public ushort Control;

        public float Scale = 1f;
        public List<IInputProcessor> Processors = new();
    }
}
