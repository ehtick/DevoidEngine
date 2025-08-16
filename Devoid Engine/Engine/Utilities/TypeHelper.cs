using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    static class TypeHelper
    {
        public static System.Numerics.Vector2 ToNumerics2(OpenTK.Mathematics.Vector2 value)
        {
            return Unsafe.As<OpenTK.Mathematics.Vector2, System.Numerics.Vector2>(ref value);
        }


    }
}
