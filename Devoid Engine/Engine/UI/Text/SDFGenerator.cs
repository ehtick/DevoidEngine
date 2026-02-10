using System;

namespace DevoidEngine.Engine.UI.Text
{
    public static class SDFGenerator
    {
        // CHANGED: Replaced 'int targetResolution' with 'float scale'
        public static BitmapData Generate(
            byte[] source,
            int srcW, int srcH, int srcPitch,
            int padding,
            float scale,
            int spread)
        {
            // 1. Pad input (High Res)
            int paddedW = srcW + padding * 2;
            int paddedH = srcH + padding * 2;

            float[] grid = new float[paddedW * paddedH];
            const float INF = 1e9f;

            // Initialize grid
            Array.Fill(grid, INF);

            // Copy source to grid
            for (int y = 0; y < srcH; y++)
            {
                for (int x = 0; x < srcW; x++)
                {
                    byte v = source[y * srcPitch + x];
                    if (v > 127)
                        grid[(y + padding) * paddedW + (x + padding)] = 0f;
                }
            }

            // 2. EDT (Euclidean Distance Transform)
            float[] distOut = PerformEDT(grid, paddedW, paddedH);

            for (int i = 0; i < grid.Length; i++)
                grid[i] = distOut[i] == 0 ? INF : 0f;

            float[] distIn = PerformEDT(grid, paddedW, paddedH);

            // 3. Combine Signed Distance
            float[] highRes = new float[paddedW * paddedH];
            for (int i = 0; i < highRes.Length; i++)
            {
                float d = MathF.Sqrt(distIn[i]) - MathF.Sqrt(distOut[i]);
                highRes[i] = Math.Clamp(d, -spread, spread);
            }

            // 4. Downsample (Using Fixed Scale)
            // REMOVED: float scale = (float)targetResolution / Math.Max(paddedW, paddedH);

            int outW = (int)(paddedW * scale);
            int outH = (int)(paddedH * scale);

            // Safety check for very small glyphs (like periods at low res)
            outW = Math.Max(1, outW);
            outH = Math.Max(1, outH);

            byte[] output = new byte[outW * outH];
            float invScale = 1.0f / scale;

            for (int y = 0; y < outH; y++)
            {
                for (int x = 0; x < outW; x++)
                {
                    int sx = (int)(x * invScale);
                    int sy = (int)(y * invScale);

                    sx = Math.Clamp(sx, 0, paddedW - 1);
                    sy = Math.Clamp(sy, 0, paddedH - 1);

                    float sd = highRes[sy * paddedW + sx];

                    // Normalize (-spread to +spread) to (0 to 1)
                    float norm = (sd / spread) * 0.5f + 0.5f;
                    output[y * outW + x] = (byte)(Math.Clamp(norm, 0f, 1f) * 255);
                }
            }

            return new BitmapData
            {
                Bitmap = output,
                Width = outW,
                Height = outH
            };
        }

        // ... (Keep PerformEDT and Solve1D exactly as they were) ...
        private static float[] PerformEDT(float[] grid, int w, int h)
        {
            // (Copy your existing EDT code here, it is correct)
            // For brevity, I am not repeating the unmodified math code.
            // Just paste the existing PerformEDT and Solve1D methods here.
            float[] dist = new float[w * h];
            Array.Copy(grid, dist, grid.Length);
            int maxDim = Math.Max(w, h);
            float[] f = new float[maxDim];
            float[] d = new float[maxDim];
            int[] v = new int[maxDim];
            float[] z = new float[maxDim + 1];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++) f[y] = dist[y * w + x];
                Solve1D(f, d, h, v, z);
                for (int y = 0; y < h; y++) dist[y * w + x] = d[y];
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++) f[x] = dist[y * w + x];
                Solve1D(f, d, w, v, z);
                for (int x = 0; x < w; x++) dist[y * w + x] = d[x];
            }
            return dist;
        }

        private static void Solve1D(float[] f, float[] d, int n, int[] v, float[] z)
        {
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;
            for (int q = 1; q < n; q++)
            {
                float s;
                do
                {
                    int vk = v[k];
                    s = ((f[q] + q * q) - (f[vk] + vk * vk)) / (2 * (q - vk));
                    if (s <= z[k]) k--;
                } while (s <= z[k]);
                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = float.PositiveInfinity;
            }
            k = 0;
            for (int q = 0; q < n; q++)
            {
                while (z[k + 1] < q) k++;
                float dx = q - v[k];
                d[q] = dx * dx + f[v[k]];
            }
        }
    }
}