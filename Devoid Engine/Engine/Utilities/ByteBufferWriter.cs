using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DevoidEngine.Engine.Utilities
{
    public static class ByteBufferWriter
    {
        public static void Write<T>(Span<byte> buffer, int offset, T value) where T : struct
        {
            MemoryMarshal.Write(buffer.Slice(offset, Unsafe.SizeOf<T>()), in value);
        }


        public static void WriteInt(Span<byte> buffer, int offset, int value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, sizeof(int)), in value);
        }
        public static void WriteFloat(Span<byte> buffer, int offset, float value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, sizeof(float)), in value);
        }

        public static void WriteVector2(Span<byte> buffer, int offset, Vector2 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 8), in value);
        }

        public static void WriteVector3(Span<byte> buffer, int offset, Vector3 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 12), in value);
        }

        public static void WriteVector4(Span<byte> buffer, int offset, Vector4 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 16), in value);
        }

        public static void WriteMatrix4x4(Span<byte> buffer, int offset, Matrix4x4 value)
        {
            MemoryMarshal.Write(buffer.Slice(offset, 64), in value);
        }

    }
}