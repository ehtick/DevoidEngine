using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Text;
using System.Numerics;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class LabelNode : UINode
    {
        public TextLayoutOptions LayoutOptions = TextLayoutOptions.Default;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                UpdateMesh(LayoutOptions.MaxWidth);

                if (string.IsNullOrEmpty(_text))
                {
                    Size = new Vector2(
                        0,
                        TextMeshGenerator.Measure(
                            Font,
                            " ",
                            Font.GetScaleForFontSize(Scale),
                            LayoutOptions
                        ).Y
                    );
                }
                else
                {
                    Size = TextMeshGenerator.Measure(
                        Font,
                        _text,
                        Font.GetScaleForFontSize(Scale),
                        LayoutOptions
                    );
                }
            }
        }

        public FontInternal Font;
        public float Scale = 1f;

        private Vector2 _measuredTextSize;
        private Mesh _mesh;
        private string _text;

        private float _lastWidthConstraint = float.PositiveInfinity;

        public LabelNode(string text, FontInternal font, float scale = 1f)
        {
            Font = font;
            Scale = scale;
            Text = text;

            Layout.FlexGrowMain = 0;
            Layout.FlexGrowCross = 0;
        }

        private void UpdateMesh(float widthConstraint)
        {
            var opts = LayoutOptions;

            if (!float.IsInfinity(widthConstraint))
                opts.MaxWidth = widthConstraint;

            var newMesh = TextMeshGenerator.Generate(
                Font,
                _text,
                Font.GetScaleForFontSize(Scale),
                opts
            );

            var oldMesh = _mesh;
            _mesh = newMesh;

            if (oldMesh != null)
                oldMesh.Dispose();
        }

        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            if (Font == null)
                return Vector2.Zero;

            var opts = LayoutOptions;

            if (!float.IsInfinity(availableSize.X))
                opts.MaxWidth = availableSize.X;

            return TextMeshGenerator.Measure(
                Font,
                _text,
                Font.GetScaleForFontSize(Scale),
                opts
            );
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            if (Font == null || string.IsNullOrEmpty(Text))
                return;

            float widthConstraint = LayoutOptions.MaxWidth;

            if (float.IsInfinity(widthConstraint))
                widthConstraint = finalRect.size.X;
            if (_lastWidthConstraint != widthConstraint)
            {
                _lastWidthConstraint = widthConstraint;
                UpdateMesh(widthConstraint);
            }
        }

        protected override void RenderCore(List<RenderItem> renderList, Matrix4x4 canvasModel)
        {
            if (Font == null || string.IsNullOrEmpty(Text) || _mesh == null)
                return;

            Matrix4x4 local = UISystem.BuildTranslationModel(Rect);
            Matrix4x4 final = local * canvasModel;

            renderList.Add(new RenderItem()
            {
                Mesh = _mesh,
                Material = Material,
                Model = final
            });
        }

        protected override void InitializeCore()
        {
            Material = UISystem.TextMaterial;
            Material.SetTexture("MAT_fontSDFAtlas", Font.Atlas.GPUTexture);
        }
    }
}