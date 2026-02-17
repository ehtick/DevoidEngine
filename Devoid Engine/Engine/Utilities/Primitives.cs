using DevoidGPU;
using System.Numerics;

namespace DevoidEngine.Engine.Utilities
{
    public static class Primitives
    {
        public static Vertex[] GetScreenQuadVertex()
        {
            return new Vertex[]
            {
        // First triangle
        new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 1)),
        new Vertex(new Vector3( 1.0f, -1.0f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 1)),
        new Vertex(new Vector3( 1.0f,  1.0f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 0)),

        // Second triangle
        new Vertex(new Vector3( 1.0f,  1.0f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 0)),
        new Vertex(new Vector3(-1.0f,  1.0f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 0)),
        new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            };
        }


        public static Vertex[] GetQuadVertex()
        {
            return new Vertex[]
            {
        // First triangle
        new Vertex(new Vector3(0.0f, 0.0f, 0.0f), Vector3.UnitZ, new Vector2(0, 0)),
        new Vertex(new Vector3(1.0f, 0.0f, 0.0f), Vector3.UnitZ, new Vector2(1, 0)),
        new Vertex(new Vector3(1.0f, 1.0f, 0.0f), Vector3.UnitZ, new Vector2(1, 1)),

        // Second triangle
        new Vertex(new Vector3(1.0f, 1.0f, 0.0f), Vector3.UnitZ, new Vector2(1, 1)),
        new Vertex(new Vector3(0.0f, 1.0f, 0.0f), Vector3.UnitZ, new Vector2(0, 1)),
        new Vertex(new Vector3(0.0f, 0.0f, 0.0f), Vector3.UnitZ, new Vector2(0, 0)),
            };
        }


        public static Vertex[] GetCubeVertex()
        {
            return new Vertex[]
            {
        // Front face (+Z)
        new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
        new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
        new Vertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),

        new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
        new Vertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),
        new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),

        // Back face (-Z)
        new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
        new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
        new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),

        new Vertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
        new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),
        new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),

        // Left face (-X)
        new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
        new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
        new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),

        new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
        new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),
        new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),

        // Right face (+X)
        new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
        new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
        new Vertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),

        new Vertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
        new Vertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),
        new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),

        // Bottom face (-Y)
        new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(1, 0)),
        new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(0, 0)),
        new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0, -1, 0), new Vector2(1, 1)),

        new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0, -1, 0), new Vector2(0, 1)),
        new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0, -1, 0), new Vector2(1, 1)),
        new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(0, 0)),

        // Top face (+Y)
        new Vertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(0, 1, 0), new Vector2(1, 0)),
        new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0, 1, 0), new Vector2(0, 0)),
        new Vertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1, 1)),

        new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(0, 1)),
        new Vertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1, 1)),
        new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0, 1, 0), new Vector2(0, 0)),
            };
        }




    }
}
