using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem
{
    public class InputDeviceLayout
    {
        public string Name;

        public Dictionary<ushort, string> ControlNames = new();
        public Dictionary<ushort, ControlKind> ControlKinds = new();
    }

    public enum ControlKind
    {
        Button,
        Axis,
        Delta
    }
}
