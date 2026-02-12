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

            float fontAscender = font.Ascender;

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

            // Initialize dimensions
            float maxLineWidth = 0f;
            //float lineHeight = font.LineHeight * scale;
            float lineHeight = (font.Ascender - font.Descender) * scale;
            float totalHeight = lineHeight; // Start with one line height

            float startX = 0;

            // We only need to track X for width; Y is calculated via line counts
            float cursorX = startX;
            bool isFirstCharacterOfLine = true;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\n')
                {
                    // End of line: Check if this was the widest line so far
                    if (cursorX > maxLineWidth)
                        maxLineWidth = cursorX;

                    // Reset X cursor
                    cursorX = startX;

                    // Add a new line to height
                    totalHeight += lineHeight;

                    isFirstCharacterOfLine = true;
                    continue;
                }

                uint charCode = (uint)c;
                if (!font.Metrics.TryGetValue(charCode, out var metric))
                {
                    continue;
                }

                // --- Layout Logic (Identical to Generate) ---

                // 1. Flush Left Adjustment
                // (Removes the left-side bearing of the first char so it aligns with x=0)
                if (isFirstCharacterOfLine)
                {
                    cursorX -= metric.OriginalBearingX * scale;
                    isFirstCharacterOfLine = false;
                }

                // 2. Advance Cursor
                // (Includes visible width + whitespace/kerning)
                cursorX += metric.HorizontalAdvance * scale;
            }

            // Check the width of the final line (since the loop ends before the newline check)
            if (cursorX > maxLineWidth)
                maxLineWidth = cursorX;

            return new Vector2(maxLineWidth, totalHeight);
        }

    }
}