using SharpDX;
using SharpFont;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Text
{
    public struct GlyphMetric
    {
        public uint CharCode;

        // The position of the glyph in the atlas (UV coordinates usually calculated from this)
        public int AtlasIndex;

        // The size of the actual pixel data inside the tile
        public int Width;
        public int Height;

        // Positioning logic (Values are normalized to the Atlas Tile Size)
        public float HorizontalAdvance;
        public float BearingX;
        public float BearingY;

        public float U;
        public float V;
        public float S;
        public float T;
    }

    public struct BitmapData
    {
        public byte[] Bitmap;
        public int Width;
        public int Height;
    }

    public class FontInternal
    {
        string path;
        int pixelSize;

        Face face;


        private const int TargetSpread = 8;
        private const int AtlasTileSize = 64;
        private const int highResSourceSize = 1024;

        public readonly float Ascender;
        public readonly float Descender;
        public readonly float LineHeight;

        public Dictionary<uint, GlyphMetric> Metrics { get; private set; }

        public GlyphAtlas Atlas { get; private set; }

        public FontInternal(Library ftLibrary, string path, int size)
        {
            this.path = path;
            this.pixelSize = size;

            face = new Face(ftLibrary, path);
            face.SetPixelSizes(0, highResSourceSize);
            face.SelectCharmap(SharpFont.Encoding.Unicode);

            float emScale = 1.0f / face.Size.Metrics.Height.ToSingle();
            Ascender = face.Size.Metrics.Ascender.ToSingle() * emScale;
            Descender = face.Size.Metrics.Descender.ToSingle() * emScale;
            LineHeight = face.Size.Metrics.Height.ToSingle() * emScale;

            Metrics = new Dictionary<uint, GlyphMetric>();

            ProcessGlyphs();
        }

        public void ProcessGlyphs()
        {
            string debugPath = Path.Combine("./DebugFonts/", face.FamilyName);


            uint glyphIndex;
            uint charCode = face.GetFirstChar(out glyphIndex);

            float scaleFactor = (float)AtlasTileSize / highResSourceSize;
            int highResSpread = (int)(TargetSpread / scaleFactor);
            int highResPadding = highResSpread + 2;

            var rawGlyphs = new Dictionary<uint, BitmapData>();

            while (glyphIndex != 0)
            {
                // Filter: ASCII + Basic Latin (Expand this logic for full Unicode support)
                if (charCode >= 32 && charCode <= 126)
                {
                    var glyphData = GenerateSingleGlyph(charCode, scaleFactor, highResSpread, highResPadding);
                    if (glyphData.Bitmap != null)
                    {
                        rawGlyphs[charCode] = glyphData;
                    }

                }

                charCode = face.GetNextChar(charCode, out glyphIndex);
            }

            Atlas = new GlyphAtlas(1024, 1024);
            Atlas.Pack(rawGlyphs);
            Atlas.UploadGPU();

            foreach (var kvp in rawGlyphs)
            {
                uint key = kvp.Key;
                if (Atlas.GlyphRectangles.TryGetValue(key, out var rect))
                {
                    var metric = Metrics[key];
                    metric.AtlasIndex = 0;

                    metric.U = (float)rect.X / Atlas.Width;
                    metric.V = (float)rect.Y / Atlas.Height;

                    // Calculate the size in UV space
                    metric.S = (float)(rect.Z + rect.X) / Atlas.Width;
                    metric.T = (float)(rect.W + rect.Y) / Atlas.Height;

                    Metrics[key] = metric;
                }
            }

            //Atlas.SaveDebug(Path.Combine(debugPath, "debug_atlas.png"));
        }

        private BitmapData GenerateSingleGlyph(uint charCode, float scaleFactor, int spread, int padding)
        {
            face.LoadChar(charCode, LoadFlags.Render | LoadFlags.ForceAutohint, LoadTarget.Normal);
            var ftBmp = face.Glyph.Bitmap;

            if (ftBmp.Width == 0 || ftBmp.Rows == 0)
            {
                Metrics[charCode] = new GlyphMetric
                {
                    CharCode = charCode,
                    Width = 0,
                    Height = 0,
                    HorizontalAdvance = face.Glyph.Metrics.HorizontalAdvance.ToSingle() * scaleFactor,
                    BearingX = 0,
                    BearingY = 0
                };
                return new BitmapData { Bitmap = null, Width = 0, Height = 0 };
            }

            // 1. Generate SDF
            BitmapData sdfBitmapData = SDFGenerator.Generate(
                ftBmp.BufferData,
                ftBmp.Width, ftBmp.Rows, ftBmp.Pitch,
                padding,
                scaleFactor,
                spread
            );

            byte[] sdfBytes = sdfBitmapData.Bitmap;
            int actualW = sdfBitmapData.Width;
            int actualH = sdfBitmapData.Height;

            // 2. Center & Clip
            // Calculate where the Top-Left of the SDF should land on the Tile
            int dstX = (AtlasTileSize - actualW) / 2;
            int dstY = (AtlasTileSize - actualH) / 2;

            byte[] tileData = new byte[AtlasTileSize * AtlasTileSize];

            // Calculate the Intersection Area (Clip Rect)
            // We only iterate pixels that are valid in BOTH Source and Destination
            int startX = Math.Max(0, dstX);
            int startY = Math.Max(0, dstY);
            int endX = Math.Min(AtlasTileSize, dstX + actualW);
            int endY = Math.Min(AtlasTileSize, dstY + actualH);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    // Dest Index: Simple coordinate on the tile
                    int destIndex = y * AtlasTileSize + x;

                    // Source Index: Shift coordinates back to SDF space
                    int srcX = x - dstX;
                    int srcY = y - dstY;
                    int srcIndex = srcY * actualW + srcX;

                    tileData[destIndex] = sdfBytes[srcIndex];
                }
            }

            // 3. Metric Correction
            float originalBearingX = face.Glyph.Metrics.HorizontalBearingX.ToSingle() * scaleFactor;
            float originalBearingY = face.Glyph.Metrics.HorizontalBearingY.ToSingle() * scaleFactor;

            // Use the calculated dstX/dstY for offset correction (even if negative)
            float finalBearingX = originalBearingX - dstX - TargetSpread;
            float finalBearingY = originalBearingY + dstY + TargetSpread;

            Metrics[charCode] = new GlyphMetric
            {
                CharCode = charCode,
                Width = AtlasTileSize,
                Height = AtlasTileSize,
                HorizontalAdvance = (face.Glyph.Metrics.HorizontalAdvance.ToSingle()) * scaleFactor,
                BearingX = finalBearingX,
                BearingY = finalBearingY
            };

            return new BitmapData
            {
                Bitmap = tileData,
                Width = AtlasTileSize,
                Height = AtlasTileSize,
            };
        }


        private void SaveDebugImage(byte[] buffer, int w, int h, string path)
        {
            using Image<Rgba32> img = new Image<Rgba32>(w, h);
            img.ProcessPixelRows(accessor => {
                for (int y = 0; y < h; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < w; x++)
                    {
                        byte val = buffer[y * w + x];
                        row[x] = new Rgba32(255, 255, 255, val);
                    }
                }
            });
            img.Save(path);
        }


    }
}