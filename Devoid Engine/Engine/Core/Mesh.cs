using System;
using System.Numerics;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
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
        public BoundingBox LocalBounds { get; private set; } = BoundingBox.Empty;

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
        //public void Render(IGraphicsDevice graphicsDevice)
        //{
        //    if (!IsRenderable || VertexBuffer == null)
        //        return;

        //    VertexBuffer.Bind();
        //    if (IndexBuffer != null)
        //    {
        //        graphicsDevice.DrawIndexed(IndexBuffer.IndexCount, 0, 0);
        //    } else
        //    {
        //        graphicsDevice.Draw(VertexBuffer.VertexCount, 0);
        //    }

        //}


        public void SetStatic(bool isStatic) => IsStatic = isStatic;

        public void SetVertices(Vertex[] vertexArray)
        {
            vertices = vertexArray;
            ComputeLocalBounds(vertexArray);


            VertexBuffer = Renderer.graphicsDevice.BufferFactory.CreateVertexBuffer(
                IsStatic ? BufferUsage.Default : BufferUsage.Dynamic,
                Vertex.VertexInfo,
                vertexArray.Length
            );

            VertexBuffer.SetData(vertexArray);

            //RenderThreadDispatcher.QueueLatest("MESH_SET_VERTICES", () =>
            //{
            //    Console.WriteLine("Created Vertex Buffer");
            //    VertexBuffer = Renderer.graphicsDevice.BufferFactory.CreateVertexBuffer(
            //        IsStatic ? BufferUsage.Default : BufferUsage.Dynamic,
            //        Vertex.VertexInfo,
            //        vertexArray.Length
            //    );

            //    VertexBuffer.SetData(vertexArray);
            //});
        }

        public void SetIndices(int[] indexArray)
        {
            indices = indexArray;

            // Create the buffer immediately so it isn't null when DrawText is called
            IndexBuffer = Renderer.graphicsDevice.BufferFactory.CreateIndexBuffer(
                indexArray.Length,
                BufferUsage.Default
            );

            IndexBuffer.SetData(indexArray);
        }

        private void ComputeLocalBounds(Vertex[] verts)
        {
            if (verts == null || verts.Length == 0)
            {
                LocalBounds = BoundingBox.Empty;
                return;
            }

            Vector3 min = verts[0].Position;
            Vector3 max = verts[0].Position;

            foreach (var v in verts)
            {
                min = Vector3.Min(min, v.Position);
                max = Vector3.Max(max, v.Position);
            }

            LocalBounds = new BoundingBox(min, max);
        }

        // Getters
        public Vertex[] GetVertices() => vertices;
        public int[] GetIndices() => indices;
    }
}
