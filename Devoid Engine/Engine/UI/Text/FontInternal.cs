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
        public byte[] bitmap;
        public int width;
        public int height;
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
                    var glyphData = GenerateSingleGlyph(charCode, scaleFactor, highResSpread, highResPadding, debugPath);
                    if (glyphData.bitmap != null)
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

        private BitmapData GenerateSingleGlyph(uint charCode, float scaleFactor, int spread, int padding, string debugPath)
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

                return new BitmapData
                {
                    bitmap = null,
                    width = 0,
                    height = 0
                };
            }


            int width  = ftBmp.Width;
            int height = ftBmp.Rows;
            int pitch = ftBmp.Pitch;
            byte[] rawBuffer = ftBmp.BufferData;

            int targetWidth = (int)Math.Ceiling(width * scaleFactor);
            int targetHeight = (int)Math.Ceiling(height * scaleFactor);

            int sdfWidth = targetWidth + (TargetSpread * 2);
            int sdfHeight = targetHeight + (TargetSpread * 2);

            BitmapData sdfBitmapData = SDFGenerator.Generate(
                rawBuffer,
                width, height, pitch,
                padding,
                AtlasTileSize, // Target Atlas Size (e.g., 64)
                spread         // High res spread
            );

            byte[] sdfBytes = sdfBitmapData.bitmap;
            int actualW = sdfBitmapData.width;
            int actualH = sdfBitmapData.height;

            byte[] tileData = new byte[AtlasTileSize * AtlasTileSize];
            int offsetX = (AtlasTileSize - actualW) / 2;
            int offsetY = (AtlasTileSize - actualH) / 2;

            for (int y = 0; y < actualH; y++)
            {
                for (int x = 0; x < actualW; x++)
                {
                    int destIndex = (offsetY + y) * AtlasTileSize + (offsetX + x);
                    if (destIndex < tileData.Length)
                        tileData[destIndex] = sdfBytes[y * actualW + x];
                }
            }

            float bearingX = face.Glyph.BitmapLeft * scaleFactor;
            float bearingY = face.Glyph.BitmapTop * scaleFactor;

            float finalBearingX = bearingX - offsetX + TargetSpread;
            // Distance from baseline to TOP of tile quad
            float finalBearingY =
                face.Glyph.BitmapTop * scaleFactor
                + (AtlasTileSize - actualH) * 0.5f;


            Metrics[charCode] = new GlyphMetric
            {
                CharCode = charCode,
                Width = AtlasTileSize,
                Height = AtlasTileSize,
                HorizontalAdvance = face.Glyph.Metrics.HorizontalAdvance.ToSingle() * scaleFactor,
                BearingX = finalBearingX,
                BearingY = finalBearingY
            };

            SaveDebugImage(sdfBytes, sdfWidth, sdfHeight, Path.Combine(debugPath, $"{charCode}.png"));
            return new BitmapData()
            {
                bitmap = tileData,
                width = AtlasTileSize,
                height = AtlasTileSize,
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
