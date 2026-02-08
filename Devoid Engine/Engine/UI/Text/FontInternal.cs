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
    public class FontInternal
    {
        string path;
        int pixelSize;

        Face face;

        private const int upscaleResolution = 512;
        private const int spread = upscaleResolution / 2;
        private const int padding = spread + 2;

        public readonly float Ascender;
        public readonly float Descender;
        public readonly float LineHeight;

        Dictionary<uint, Glyph> glyphs;

        public FontInternal(Library ftLibrary, string path, int size)
        {
            this.path = path;
            this.pixelSize = size;

            face = new Face(ftLibrary, path);
            face.SetPixelSizes(0, (uint)size);


            Ascender = face.Size.Metrics.Ascender.ToSingle();
            Descender = face.Size.Metrics.Descender.ToSingle();
            LineHeight = face.Size.Metrics.Height.ToSingle();

            glyphs = new Dictionary<uint, Glyph>();
            Load();
        }

        public void Load()
        {

            face.SelectCharmap(SharpFont.Encoding.Unicode);
            face.SetPixelSizes(upscaleResolution, upscaleResolution);

            uint glyphIndex = 0;
            uint charCode = face.GetFirstChar(out glyphIndex);

            while (glyphIndex != 0)
            {
                if (charCode < 32 || charCode > 126)
                {
                    charCode = face.GetNextChar(charCode, out glyphIndex);
                    continue;
                }

                face.LoadChar(charCode, LoadFlags.Render | LoadFlags.ForceAutohint, LoadTarget.Normal);

                var bmp = face.Glyph.Bitmap;

                if (bmp.Width == 0 || bmp.Rows == 0 || bmp.Buffer == IntPtr.Zero)
                {
                    // Still a valid glyph, just no bitmap (e.g. space)
                    charCode = face.GetNextChar(charCode, out glyphIndex);
                    continue;
                }

                Console.WriteLine("##################### + " + charCode);
                Console.WriteLine(new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop));
                Console.WriteLine(new Vector2(upscaleResolution));
                Console.WriteLine(new Vector2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows));
                Console.WriteLine("##################### + " + charCode);

                Debug.Assert(bmp.PixelMode == PixelMode.Gray);
                Debug.Assert(Math.Abs(bmp.Pitch) == bmp.Width);

                int glyphWidth = bmp.Width;
                int glyphHeight = bmp.Rows;
                int pitch = bmp.Pitch;

                (byte[], int, int) data = SDFGenerator.GenerateSDF(padding, upscaleResolution, pixelSize, spread, glyphWidth, glyphHeight, bmp.BufferData);

                byte[] sdf = data.Item1;
                int paddedW = data.Item2;
                int paddedH = data.Item3;

                SaveGrayscalePng_ImageSharp(
                    sdf,
                    paddedW,
                    paddedH,
                    $"./test_font/{charCode}.png"
                );



                charCode = face.GetNextChar(charCode, out glyphIndex);
            }
        }





        void SaveGrayscalePng_ImageSharp(
            byte[] buffer,
            int width,
            int height,
            string path)
        {
            using Image<Rgba32> image = new Image<Rgba32>(width, height);

            for (int y = 0; y < height; y++)
            {
                Span<Rgba32> row = image.DangerousGetPixelRowMemory(y).Span;

                for (int x = 0; x < width; x++)
                {
                    byte v = buffer[y * width + x];

                    row[x] = new Rgba32(255,255,255,v);
                }
            }

            image.Save(path);
        }


    }
}
