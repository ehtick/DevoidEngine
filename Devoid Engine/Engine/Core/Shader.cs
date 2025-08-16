using DevoidEngine.Engine.Rendering;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public class Shader
    {
        public IShader vShader;
        public IShader fShader;
        public IShaderProgram shaderProgram;

        public Shader(string vsfsPath)
        {
            string vsPath = vsfsPath + ".vert.hlsl";
            string fsPath = vsfsPath + ".frag.hlsl";

            string vSource, fSource;

            using (StreamReader sr = new StreamReader(vsPath, Encoding.UTF8))
            {
                vSource = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader(fsPath, Encoding.UTF8))
            {
                fSource = sr.ReadToEnd();
            }

            vShader = Renderer.graphicsDevice.ShaderFactory.CreateShader(ShaderType.Vertex, vSource, "VSMain");
            IShader fShader = Renderer.graphicsDevice.ShaderFactory.CreateShader(ShaderType.Fragment, fSource, "PSMain");

            shaderProgram = Renderer.graphicsDevice.ShaderFactory.CreateShaderProgram();
            shaderProgram.AttachShader(vShader);
            shaderProgram.AttachShader(fShader);
        }

        public Shader(string vsPath, string fsPath)
        {
            string vSource, fSource;

            using (StreamReader sr = new StreamReader(vsPath, Encoding.UTF8))
            {
                vSource = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader(fsPath, Encoding.UTF8))
            {
                fSource = sr.ReadToEnd();
            }

            vShader = Renderer.graphicsDevice.ShaderFactory.CreateShader(ShaderType.Vertex, vSource, "VSMain");
            IShader fShader = Renderer.graphicsDevice.ShaderFactory.CreateShader(ShaderType.Fragment, fSource, "PSMain");

            shaderProgram = Renderer.graphicsDevice.ShaderFactory.CreateShaderProgram();
            shaderProgram.AttachShader(vShader);
            shaderProgram.AttachShader(fShader);
            
        }

        public void LinkShaders()
        {

        }

        public void Use()
        {
            shaderProgram.Bind();
        }


    }
}
