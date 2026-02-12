using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{

    public static class Renderer
    {
        public static IGraphicsDevice graphicsDevice;

        public static int Width;
        public static int Height;

        public static void Initialize(IGraphicsDevice gd, int width, int height)
        {
            Width = width;
            Height = height;

            graphicsDevice = gd;
            graphicsDevice.SetViewport(0, 0, width, height);


            Renderer3D.Initialize(width, height);
            UIRenderer.Initialize(width, height);
        }

        public static IInputLayout GetInputLayout(Mesh mesh, Shader shader)
        {
            var key = (mesh.VertexBuffer.Layout, shader.vShader);
            if (!InputLayoutManager.inputLayoutCache.TryGetValue(key, out var layout))
            {
                layout = graphicsDevice.CreateInputLayout(mesh.VertexBuffer.Layout, shader.vShader);
                InputLayoutManager.inputLayoutCache[key] = layout;
            }
            return layout;
        }

        public static void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            Renderer3D.Resize(width, height);
            UIRenderer.Resize(width, height);
        }

    }
}
