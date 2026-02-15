namespace DevoidGPU
{
    public interface IShaderStorageBuffer<T> where T : struct
    {
        BufferUsage Usage { get; }
        void SetData(T[] data);
        public void UpdatePartial(T[] data, int startIndex, int elementCount, int elementOffset = 0);
        void Bind(int slot, ShaderStage stages);
        void UnBind(int slot, ShaderStage stages);
        void BindMutable(int slot);
        void UnBindMutable(int slot);
    }
}
