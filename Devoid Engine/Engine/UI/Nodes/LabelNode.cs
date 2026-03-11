using DevoidEngine.Engine.Components;
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
                _meshDirty = true;
            }
            //set
            //{
            //    _text = value;

            //    UpdateMesh(LayoutOptions.MaxWidth);

            //    if (string.IsNullOrEmpty(_text))
            //    {
            //        Size = new Vector2(
            //            0,
            //            TextMeshGenerator.Measure(
            //                Font,
            //                " ",
            //                Font.GetScaleForFontSize(Scale),
            //                LayoutOptions
            //            ).Y
            //        );
            //    }
            //    else
            //    {
            //        Size = TextMeshGenerator.Measure(
            //            Font,
            //            _text,
            //            Font.GetScaleForFontSize(Scale),
            //            LayoutOptions
            //        );
            //    }
            //}
        }

        public FontInternal Font;
        public float Scale = 1f;

        private Vector2 _measuredTextSize;
        private Mesh _mesh;
        private string _text;
        bool _meshDirty = true;

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

            float widthConstraint = availableSize.X;

            if ((_meshDirty || _lastWidthConstraint != widthConstraint) && !string.IsNullOrEmpty(_text))
            {
                _meshDirty = false;
                _lastWidthConstraint = widthConstraint;
                UpdateMesh(widthConstraint);
            }

            var opts = LayoutOptions;
            opts.MaxWidth = widthConstraint;

            Vector2 textSize = TextMeshGenerator.Measure(
                Font,
                _text,
                Font.GetScaleForFontSize(Scale),
                opts
            );

            textSize.Y = Math.Max((Font.Ascender - Font.Descender) * Font.GetScaleForFontSize(Scale), textSize.Y);
            return textSize;
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            if (Font == null || string.IsNullOrEmpty(Text))
                return;

            //float widthConstraint = LayoutOptions.MaxWidth;

            float widthConstraint = finalRect.size.X;
            if ((_meshDirty || _lastWidthConstraint != widthConstraint) && !string.IsNullOrEmpty(_text))
            {
                _meshDirty = false;
                _lastWidthConstraint = widthConstraint;
                UpdateMesh(widthConstraint);
            }
        }

        protected override void RenderCore(List<RenderItem> renderList, Matrix4x4 canvasModel)
        {
            if (Font == null || string.IsNullOrEmpty(Text) || _mesh == null)
                return;

            Vector2 pos = Rect.position;

            pos.X = MathF.Round(pos.X);
            pos.Y = MathF.Round(pos.Y);

            //Matrix4x4 model =
            //    Matrix4x4.CreateTranslation(pos.X, pos.Y, 0);

            Matrix4x4 local = UISystem.BuildTranslationModel(new UITransform(pos, Rect.size));
            Matrix4x4 final = local * canvasModel;

            renderList.Add(new RenderItem()
            {
                Mesh = _mesh,
                Material = Material,
                Model = final
            });
        }

        public Vector2 GetCursorPosition(int characterIndex, float widthConstraint)
        {
            if (Font == null || string.IsNullOrEmpty(Text) || characterIndex <= 0)
                return Vector2.Zero;

            int safeIndex = Math.Clamp(characterIndex, 0, Text.Length);
            string substring = Text.Substring(0, safeIndex);

            var opts = LayoutOptions;
            opts.MaxWidth = widthConstraint;

            // USE THE NEW METHOD HERE, NOT Measure()!
            return TextMeshGenerator.GetCursorPosition(
                Font,
                substring,
                Font.GetScaleForFontSize(Scale),
                opts
            );
        }

        protected override void InitializeCore()
        {
            Material = UISystem.TextMaterial;
            Material.SetTexture("MAT_fontSDFAtlas", Font.Atlas.GPUTexture);
        }

        protected override void UpdateCore(float deltaTime)
        {

        }
    }
}