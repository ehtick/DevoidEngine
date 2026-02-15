namespace DevoidGPU
{
    public interface ISampler
    {
        SamplerDescription Description { get; }
        void Bind(int slot = 0);
    }
}
