using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    public static class UniformBufferHelper
    {
        public static void WriteInt(Span<byte> buffer, int offset, int value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, sizeof(int)), ref value);
        }
        public static void WriteFloat(Span<byte> buffer, int offset, float value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, sizeof(float)), ref value);
        }

        public static void WriteVector2(Span<byte> buffer, int offset, Vector2 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 8), ref value);
        }

        public static void WriteVector3(Span<byte> buffer, int offset, Vector3 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 12), ref value);
        }

        public static void WriteVector4(Span<byte> buffer, int offset, Vector4 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 16), ref value);
        }

        public static void WriteMatrix4x4(Span<byte> buffer, int offset, Matrix4x4 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 64), ref value);
        }

    }
}
