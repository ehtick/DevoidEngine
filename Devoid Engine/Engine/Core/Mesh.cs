using System;
using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Core
{
    public class Mesh : IDisposable
    {
        public IVertexBuffer VertexBuffer { get; private set; }
        public IIndexBuffer IndexBuffer { get; private set; }

        private Vertex[] vertices;
        private int[] indices;

        //public Material Material { get; set; }

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }

        // State
        public bool IsRenderable { get; set; } = true;
        public bool IsHighlighted { get; set; } = false;
        public bool IsStatic { get; private set; } = true;

        private bool isDisposed = false;

        // Constructors
        public Mesh() { }

        ~Mesh()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed) return;

            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();

            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        // Rendering handled by Renderer, not Mesh itself.
        public void Render(IGraphicsDevice graphicsDevice)
        {
            if (!IsRenderable || VertexBuffer == null)
                return;

            VertexBuffer.Bind();
            if (IndexBuffer != null)
            {

            } else
            {
                graphicsDevice.Draw(VertexBuffer.VertexCount, 0);
            }

        }


        public void SetStatic(bool isStatic) => IsStatic = isStatic;

        public void SetVertices(Vertex[] vertexArray)
        {
            vertices = vertexArray;

            VertexBuffer = Renderer.graphicsDevice.BufferFactory.CreateVertexBuffer(
                BufferUsage.Default,
                Vertex.VertexInfo,
                vertexArray.Length
            );

            VertexBuffer.SetData(vertexArray);
        }

        public void SetIndices(int[] indexArray)
        {
            //indices = indexArray;

            //IndexBuffer = Renderer.graphicsDevice.BufferManager.CreateIndexBuffer(
            //    indexArray.Length,
            //    BufferUsage.Default
            //);

            //IndexBuffer.SetData(indexArray);
        }

        // Getters
        public Vertex[] GetVertices() => vertices;
        public int[] GetIndices() => indices;
    }
}
