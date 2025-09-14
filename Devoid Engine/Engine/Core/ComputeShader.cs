using System.Numerics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DevoidGPU;
using DevoidEngine.Engine.Rendering;

namespace DevoidEngine.Engine.Core
{
    public class ComputeShader : IDisposable
    {

        IComputeShader cShader;

        public ComputeShader(string path, Vector3? WorkGroupSize = null)
        {
            string Cpath = path;

            string ComputeShaderSource;

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                ComputeShaderSource = reader.ReadToEnd();
            }

            cShader = Renderer.graphicsDevice.ShaderFactory.CreateComputeShader(ComputeShaderSource, "CSMain");
        }

        public void Use()
        {
            cShader.Use();
        }

        public void Dispatch(int workGroupX, int WorkGroupY, int workGroupZ)
        {
            cShader.Dispatch(workGroupX, WorkGroupY, workGroupZ);
        }

        public void Wait()
        {
            cShader.Wait();
        }

        ~ComputeShader()
        {
            cShader.Dispose();
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }

    }
}