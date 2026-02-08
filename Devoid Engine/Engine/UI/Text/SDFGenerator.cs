using System;

namespace DevoidEngine.Engine.UI.Text
{
    public static class SDFGenerator
    {
        public static BitmapData Generate(
            byte[] source,
            int srcW, int srcH, int srcPitch,
            int padding,
            int targetResolution,
            int spread)
        {
            // 1. Pad input
            int paddedW = srcW + padding * 2;
            int paddedH = srcH + padding * 2;
            float[] grid = new float[paddedW * paddedH];

            // Initialize grid
            const float INF = 1e9f;
            for (int i = 0; i < grid.Length; i++) grid[i] = INF;

            for (int y = 0; y < srcH; y++)
            {
                for (int x = 0; x < srcW; x++)
                {
                    byte val = source[y * srcPitch + x];
                    if (val > 127)
                    {
                        grid[(y + padding) * paddedW + (x + padding)] = 0f;
                    }
                }
            }

            // 2. Compute Signed Distance
            // Outside
            float[] distOut = PerformEDT(grid, paddedW, paddedH);

            // Inside (Invert logic)
            for (int i = 0; i < grid.Length; i++) grid[i] = (distOut[i] == 0) ? INF : 0f;
            float[] distIn = PerformEDT(grid, paddedW, paddedH);

            // 3. Combine & Normalize
            float[] highResSDF = new float[paddedW * paddedH];
            for (int i = 0; i < highResSDF.Length; i++)
            {
                float dIn = MathF.Sqrt(distIn[i]);
                float dOut = MathF.Sqrt(distOut[i]);
                float sd = dIn - dOut;

                highResSDF[i] = Math.Clamp(sd, -spread, spread);
            }

            // 4. Downsample
            float scale = (float)targetResolution / (float)Math.Max(paddedW, paddedH);
            int outW = (int)(paddedW * scale);
            int outH = (int)(paddedH * scale);

            // Safety check for 0 dimensions
            outW = Math.Max(1, outW);
            outH = Math.Max(1, outH);

            byte[] output = new byte[outW * outH];
            float invScale = 1.0f / scale;

            for (int y = 0; y < outH; y++)
            {
                for (int x = 0; x < outW; x++)
                {
                    float srcX = x * invScale;
                    float srcY = y * invScale;

                    int sx = (int)srcX;
                    int sy = (int)srcY;

                    if (sx >= 0 && sx < paddedW && sy >= 0 && sy < paddedH)
                    {
                        float sd = highResSDF[sy * paddedW + sx];
                        float norm = (sd / spread) * 0.5f + 0.5f;
                        output[y * outW + x] = (byte)(Math.Clamp(norm, 0, 1) * 255);
                    }
                }
            }

            return new BitmapData()
            {
                bitmap = output,
                width = outW,
                height = outH
                
            };
        }

        private static float[] PerformEDT(float[] grid, int w, int h)
        {
            float[] dist = new float[w * h];
            Array.Copy(grid, dist, grid.Length);

            int maxDim = Math.Max(w, h);

            // Optimization: Allocate buffers once here, reuse in Solve1D
            float[] dt = new float[maxDim];     // Input buffer for 1D pass
            float[] d_out = new float[maxDim];  // Output buffer for 1D pass
            int[] v = new int[maxDim];
            float[] z = new float[maxDim + 1];

            // Pass 1: Columns
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++) dt[y] = dist[y * w + x];

                // Pass buffers explicitly
                Solve1D(dt, d_out, h, v, z);

                for (int y = 0; y < h; y++) dist[y * w + x] = d_out[y];
            }

            // Pass 2: Rows
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++) dt[x] = dist[y * w + x];

                // Pass buffers explicitly
                Solve1D(dt, d_out, w, v, z);

                for (int x = 0; x < w; x++) dist[y * w + x] = d_out[x];
            }

            return dist;
        }

        // Modified to return void and take an output buffer
        private static void Solve1D(float[] f, float[] d, int n, int[] v, float[] z)
        {
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;

            for (int q = 1; q < n; q++)
            {
                while (true)
                {
                    int vk = v[k];
                    // Calculate intersection of parabolas
                    float s = ((f[q] + q * q) - (f[vk] + vk * vk)) / (2 * (q - vk));

                    if (s <= z[k])
                    {
                        k--;
                        continue;
                    }

                    k++;
                    v[k] = q;
                    z[k] = s;
                    z[k + 1] = float.PositiveInfinity;
                    break;
                }
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