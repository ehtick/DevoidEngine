using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Text;
using System.Numerics;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class LabelNode : UINode
    {
        public string Text
        {
            get 
            {
                return _text;
            }
            set
            {
                _text = value;
                Console.WriteLine(_text);
                Console.WriteLine(Font);
                Console.WriteLine(Scale);
                _mesh = TextMeshGenerator.Generate(Font, _text, Font.GetScaleForFontSize(Scale));
            }
        }
        public FontInternal Font;
        public float Scale = 1f;

        private Vector2 _measuredTextSize;
        private Mesh _mesh;
        private string _text;

        public LabelNode(string text, FontInternal font, float scale = 1f)
        {
            Font = font;
            Scale = scale;
            Text = text;

            // Text should NOT expand by default
            Layout.FlexGrowMain = 0;
            Layout.FlexGrowCross = 0;
        }

        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return Vector2.Zero;

            // Compute intrinsic text size
            _measuredTextSize = TextMeshGenerator.Measure(Font, Text, Scale);

            return _measuredTextSize;
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            if (Font == null || string.IsNullOrEmpty(Text) || _mesh == null)
                return;



            // Render text inside the allocated rect
            UIRenderer.DrawText(
                new UITransform(
                    finalRect.position,
                    finalRect.size
                ),
                _mesh,
                Font.Atlas.GPUTexture
                //Font,
                //Text,
                //finalRect.position,
                //Scale,
                //finalRect.size
            );
        }

        protected override void RenderCore()
        {
            // Not used — rendering happens in ArrangeCore like BoxNode
        }
    }
}
