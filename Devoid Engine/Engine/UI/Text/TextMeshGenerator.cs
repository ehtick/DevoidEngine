using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Text
{
    public static class TextMeshGenerator
    {
        private static List<Vertex> vertices = new List<Vertex>();
        private static List<int> indices = new List<int>();

        public static Mesh Generate
        (
            FontInternal font,
            string text,
            float scale
        )
        {
            vertices.Clear();
            indices.Clear();

            if (string.IsNullOrEmpty(text) || font == null)
            {
                return new Mesh();
            }

            Vector2 cursor = new Vector2();
            float startX = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint charCode = (uint)c;

                if (c == '\n')
                {
                    cursor.X = startX;
                    cursor.Y += font.LineHeight * scale;
                    continue;
                }

                if (!font.Metrics.TryGetValue(charCode, out var metric))
                {
                    continue;
                }

                Console.WriteLine(metric.HorizontalAdvance);

                float xpos = cursor.X + (metric.BearingX * scale);
                float ypos = cursor.Y - metric.BearingY;


                float w = metric.Width * scale;
                float h = metric.Height * scale;

                int indexOffset = vertices.Count;

                vertices.Add(
                    new Vertex
                    (
                        new Vector3(xpos, ypos, 0),
                        Vector3.Zero,
                        new Vector2(metric.U, metric.V)
                    )
                );

                vertices.Add(
                    new Vertex
                    (
                        new Vector3(xpos + w, ypos, 0),
                        Vector3.Zero,
                        new Vector2(metric.S, metric.V)
                    )
                );

                vertices.Add(
                    new Vertex
                    (
                        new Vector3(xpos + w, ypos + h, 0),
                        Vector3.Zero,
                        new Vector2(metric.S, metric.T)
                    )
                );

                vertices.Add(
                    new Vertex
                    (
                        new Vector3(xpos, ypos + h, 0),
                        Vector3.Zero,
                        new Vector2(metric.U, metric.T)
                    )
                );

                indices.Add(indexOffset + 0);
                indices.Add(indexOffset + 1);
                indices.Add(indexOffset + 2);

                indices.Add(indexOffset + 0);
                indices.Add(indexOffset + 2);
                indices.Add(indexOffset + 3);

                cursor.X += metric.HorizontalAdvance * scale;
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices.ToArray());
            mesh.SetIndices(indices.ToArray());
            return mesh;
        }

    }
}
