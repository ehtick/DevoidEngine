using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Text
{
    public static class SDFGenerator
    {
        public static (byte[], int, int) GenerateSDF
        (
            int padding,
            int upscaleResolution,
            int targetSize,
            int spread,
            int width,
            int height,
            byte[] source
        )
        {
            int paddedWidth = width + padding * 2;
            int paddedHeight = height + padding * 2;
            byte[] padded = new byte[paddedWidth * paddedHeight];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    padded[(x + padding) + (y + padding) * paddedWidth] = source[x + y * width];
                }
            }


            float[] outside = performEDT2D(padded, paddedWidth, paddedHeight);

            byte[] inverted = new byte[padded.Length];
            for (int i = 0; i < padded.Length; i++)
            {
                inverted[i] = Invert(padded[i]);
            }

            float[] inside = performEDT2D(inverted, paddedWidth, paddedHeight);

            byte[] sdf = new byte[padded.Length];

            for (int i = 0; i < sdf.Length; i++)
            {
                float distIn = MathF.Min(inside[i], spread);
                float distOut = MathF.Min(outside[i], spread);

                float signed = distIn - distOut;

                float normalized = signed / spread;
                normalized = normalized * 0.5f + 0.5f;
                sdf[i] = (byte)Math.Clamp(normalized * 255f, 0f, 255f);
            }

            // 3. Downsample Loop
            int scaleFactor = upscaleResolution / targetSize;
            int finalWidth = paddedWidth / scaleFactor;
            int finalHeight = paddedHeight / scaleFactor;
            byte[] finalSDF = new byte[finalWidth * finalHeight];

            for (int y = 0; y < finalHeight; y++)
            {
                for (int x = 0; x < finalWidth; x++)
                {
                    float sumSigned = 0f;
                    int samples = 0;

                    for (int ky = 0; ky < scaleFactor; ky++)
                    {
                        for (int kx = 0; kx < scaleFactor; kx++)
                        {
                            int srcX = x * scaleFactor + kx;
                            int srcY = y * scaleFactor + ky;

                            if (srcX < paddedWidth && srcY < paddedHeight)
                            {
                                int i = srcX + srcY * paddedWidth;
                                sumSigned += inside[i] - outside[i]; // ✔ RAW
                                samples++;
                            }
                        }
                    }

                    float avgSigned = sumSigned / samples;

                    // ----------------------------------------------------
                    // 4. Normalize ONCE (using original spread)
                    // ----------------------------------------------------
                    float normalized = avgSigned / spread;
                    normalized = normalized * 0.5f + 0.5f;

                    finalSDF[x + y * finalWidth] =
                        (byte)Math.Clamp(normalized * 255f, 0f, 255f);
                }
            }



            return (finalSDF, finalWidth, finalHeight);

        }

        static byte Invert(byte v) => v == 0 ? (byte)255 : (byte)0;


        static float[] performEDT2D(byte[] bitmap, int width, int height)
        {
            const float INF = 1e9f;

            float[] grid = new float[width * height];
            float[] temp = new float[Math.Max(width, height)];
            float[] dist = new float[Math.Max(width, height)];
            int[] v = new int[Math.Max(width, height)];
            float[] z = new float[Math.Max(width, height) + 1];

            for (int i = 0; i < grid.Length; i++)
                grid[i] = bitmap[i] > 127 ? 0f : INF; // ✔ thresholded

            // Vertical pass
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    temp[y] = grid[x + y * width];

                DistanceTransform1D(temp, height, dist, v, z); // ✔ FIXED

                for (int y = 0; y < height; y++)
                    grid[x + y * width] = dist[y];
            }

            // Horizontal pass
            float[] result = new float[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    temp[x] = grid[x + y * width];

                DistanceTransform1D(temp, width, dist, v, z);

                for (int x = 0; x < width; x++)
                    result[x + y * width] = MathF.Sqrt(dist[x]);
            }

            return result;
        }


        static void DistanceTransform1D(
            float[] f,
            int n,
            float[] d,
            int[] v,
            float[] z)
        {
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;

            for (int q = 1; q < n; q++)
            {
                float s;
                while (true)
                {
                    int vk = v[k];

                    // Calculate intersection of parabola q and vk
                    // (f[q] + q*q) - (f[vk] + vk*vk) / (2 * (q - vk))

                    // We use the raw formula here to ensure we calculate s 
                    // against the CURRENT vk in this iteration of the while loop.
                    float numer = (f[q] + q * q) - (f[vk] + vk * vk);
                    float denom = 2 * (q - vk);
                    s = numer / denom;

                    // If the intersection is to the left of the current parabola's
                    // starting definition, the current parabola vk is hidden.
                    if (s <= z[k])
                    {
                        k--;
                        // We continue the loop to check against the NEXT parabola down the stack
                        // and RECALCULATE s.
                    }
                    else
                    {
                        // We found the correct position
                        break;
                    }
                }

                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = float.PositiveInfinity;
            }

            k = 0;
            for (int q = 0; q < n; q++)
            {
                while (z[k + 1] < q) k++;
                int vk = v[k];
                float dx = q - vk;
                d[q] = dx * dx + f[vk];
            }
        }

    }
}
