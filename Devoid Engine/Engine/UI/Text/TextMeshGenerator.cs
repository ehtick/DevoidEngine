using DevoidEngine.Engine.Core;
using DevoidGPU;
using System.Collections.Generic;
using System.Numerics;

namespace DevoidEngine.Engine.UI.Text
{
    public static class TextMeshGenerator
    {
        private static List<Vertex> vertices = new List<Vertex>();
        private static List<int> indices = new List<int>();

        public static Mesh Generate(FontInternal font, string text, float scale)
        {
            vertices.Clear();
            indices.Clear();

            if (string.IsNullOrEmpty(text) || font == null)
            {
                return new Mesh();
            }

            float fontAscender = font.Ascender + font.Descender;
            Console.WriteLine(fontAscender);
            Console.WriteLine(scale);

            if (fontAscender == 0)
            {
                fontAscender = font.LineHeight * 0.8f;
            }

            float startX = 0;
            float startY = fontAscender * scale;

            Vector2 cursor = new Vector2(startX, startY);
            bool isFirstCharacterOfLine = true;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint charCode = (uint)c;

                if (c == '\n')
                {
                    cursor.X = startX;
                    cursor.Y += font.LineHeight * scale;
                    isFirstCharacterOfLine = true;
                    continue;
                }

                if (!font.Metrics.TryGetValue(charCode, out var metric))
                {
                    continue;
                }

                float scaledBearingY = metric.BearingY * scale;
                float scaledBearingX = metric.BearingX * scale;

                if (isFirstCharacterOfLine)
                {
                    cursor.X -= metric.OriginalBearingX * scale;
                    isFirstCharacterOfLine = false;
                }

                float xpos = cursor.X + scaledBearingX;
                float ypos = cursor.Y - scaledBearingY;

                float w = metric.Width * scale;
                float h = metric.Height * scale;

                int indexOffset = vertices.Count;


                // Top Left
                vertices.Add(new Vertex(
                    new Vector3(xpos, ypos, 0),
                    Vector3.Zero,
                    new Vector2(metric.U, metric.V) // V usually Top
                ));

                // Top Right
                vertices.Add(new Vertex(
                    new Vector3(xpos + w, ypos, 0),
                    Vector3.Zero,
                    new Vector2(metric.S, metric.V)
                ));

                // Bottom Right
                vertices.Add(new Vertex(
                    new Vector3(xpos + w, ypos + h, 0),
                    Vector3.Zero,
                    new Vector2(metric.S, metric.T) // T usually Bottom
                ));

                // Bottom Left
                vertices.Add(new Vertex(
                    new Vector3(xpos, ypos + h, 0),
                    Vector3.Zero,
                    new Vector2(metric.U, metric.T)
                ));

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

        public static Vector2 Measure(FontInternal font, string text, float scale)
        {
            if (string.IsNullOrEmpty(text) || font == null)
            {
                return Vector2.Zero;
            }

            float currentLineWidth = 0;
            float maxLineWidth = 0;

            // Start with height of one line
            float totalHeight = font.LineHeight * scale;

            bool isFirstCharOfLine = true;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // --- 1. Handle Newlines ---
                if (c == '\n')
                {
                    // Lock in the width of the line we just finished
                    if (currentLineWidth > maxLineWidth)
                        maxLineWidth = currentLineWidth;

                    // Reset for next line
                    currentLineWidth = 0;
                    totalHeight += font.LineHeight * scale;
                    isFirstCharOfLine = true;
                    continue;
                }

                uint charCode = (uint)c;
                if (!font.Metrics.TryGetValue(charCode, out var metric))
                {
                    continue;
                }

                // --- 2. Handle "Flush Left" Fix ---
                // Just like Generate(), we subtract the empty bearing from the start.
                // This ensures the box starts exactly where the ink starts.
                if (isFirstCharOfLine)
                {
                    currentLineWidth -= metric.OriginalBearingX * scale;
                    isFirstCharOfLine = false;
                }

                // --- 3. Advance ---
                // Simply add the horizontal advance (no kerning lookup)
                currentLineWidth += metric.HorizontalAdvance * scale;
            }

            // Capture the width of the final line
            if (currentLineWidth > maxLineWidth)
                maxLineWidth = currentLineWidth;

            return new Vector2(maxLineWidth, totalHeight);
        }
    }
}