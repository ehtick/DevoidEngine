using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Utilities
{
    public unsafe class BufferObject<T> where T : struct
    {


        IShaderStorageBuffer<T> shaderStorageBuffer;

        public BufferObject(int elementCount, BufferUsage usage, bool allowRW = false)
        {
            shaderStorageBuffer = Renderer.graphicsDevice.BufferFactory.CreateShaderStorageBuffer<T>(elementCount, usage, allowRW);

        }

        public void SetData(in T[] data, int start, int count, int offset = 0)
        {
            shaderStorageBuffer.UpdatePartial(data, start, count, offset);
        }

        public void SetData(in T[] data, int count, int offset = 0)
        {
            shaderStorageBuffer.UpdatePartial(data, 0, count, offset);
        }

        public void Bind(int slot = 0, ShaderStage stages = ShaderStage.Fragment)
        {
            shaderStorageBuffer.Bind(slot, stages);
        }

        public void BindMutable(int slot = 0)
        {
            shaderStorageBuffer.BindMutable(slot);
        }

        public void UnBindMutable(int slot = 0)
        {
            shaderStorageBuffer.UnBindMutable(slot);
        }

        public void UnBind(int slot = 0, ShaderStage stages = ShaderStage.All)
        {
            shaderStorageBuffer.UnBind(slot, stages);
        }
    }


}
