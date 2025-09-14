using DevoidEngine.Engine.Rendering;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    public class UniformBuffer<T> where T : struct
    {
        IUniformBuffer uniformBuffer;

        public UniformBuffer(BufferUsage bufferUsage = BufferUsage.Dynamic)
        {
            uniformBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer<T>(bufferUsage);
        }

        public void SetData(ref T data)
        {
            uniformBuffer.SetData(ref data);
        }

        public void Bind(int slot = 0, ShaderStage stage = ShaderStage.Fragment)
        {
            uniformBuffer.Bind(slot, stage);
        }

    }
}
