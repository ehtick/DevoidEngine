using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using System.Numerics;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class BoxNode : FlexboxNode
    {
        private Texture2D _texture;

        public Texture2D Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                UpdateMaterial();
            }
        }

        public Vector4 Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdateMaterial();
            }
        }

        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                UpdateMaterial();
            }
        }

        public float BorderThickness = 0f;
        public Vector4 BorderColor = new Vector4(0, 0, 0, 1);

        private Vector4 _color = new Vector4(1, 1, 1, 1);
        private float _opacity = 1f;

        protected override void InitializeCore()
        {
            Material = UISystem.UIMaterial;
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (Material == null)
                return;

            Material.SetInt("useTexture", _texture != null ? 1 : 0);
            Material.SetTexture("MAT_Texture", _texture);

            Vector4 final = _color;
            final.W *= _opacity;

            Material.SetVector4("COLOR", final);
        }

        //protected override Vector2 MeasureCore(Vector2 availableSize)
        //{
        //    return Size ?? Vector2.Zero;
        //}

        //protected override void ArrangeCore(UITransform finalRect)
        //{
        //    Rect = finalRect;
        //}

        public override void Render(List<RenderItem> renderList, Matrix4x4 canvasModel)
        {
            renderList.Add(new RenderItem()
            {
                Mesh = UISystem.QuadMesh,
                Material = Material,
                Model = UISystem.BuildModel(Rect)
            });

            UIScissorStack.Push(Rect.position.X, Rect.position.Y, Rect.size.X, Rect.size.Y);
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Render(renderList, canvasModel);
            }
            UIScissorStack.Pop();
        }

        protected override void UpdateCore(float deltaTime)
        {

        }
    }
}