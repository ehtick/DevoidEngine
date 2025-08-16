using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DevoidEngine.Engine.Utilities
{
    public static class Primitives
    {
        public static Vertex[] GetQuadVertex()
        {
            return new Vertex[]
            {
                // First triangle
                new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new Vertex(new Vector3( 0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new Vertex(new Vector3( 0.5f,  0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 1)),

                // Second triangle
                new Vertex(new Vector3( 0.5f,  0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 1)),
                new Vertex(new Vector3(-0.5f,  0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 0)),
            };
        }


    }
}
