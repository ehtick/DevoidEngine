using System.Runtime.CompilerServices;

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
