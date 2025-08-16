using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IGraphicsDevice
    {
        IPresentSurface MainSurface { get; }

        // Factories
        IBufferFactory BufferFactory { get; }
        IShaderFactory ShaderFactory { get; }
        ITextureFactory TextureFactory { get; }


        void Initialize(nint hwnd, PresentationParameters parameters);

        void SetViewport(int x, int y, int width, int height);
        
        void SetBlendState();
        void SetScissorState();
        void SetAlphaBlendState();
        void SetRasterizerState();



        void DrawInstanced(int indexCount, int startIndexLocation, int baseVertexLocation);
        void Draw(int vertexCount, int startVertex);

        // Optionally move this to its own factory when similar items are added.
        IInputLayout CreateInputLayout(VertexInfo vertexInfo, IShader vertexShader);

        
    }
}
