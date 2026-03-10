using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Text;
using System.Numerics;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class InputFieldNode : UINode
    {
        public string Text = "";
        public int CaretIndex = 0;

        public Vector2 Padding = new Vector2(6, 4);

        public Vector4 BackgroundColor = new Vector4(0, 0, 0, 0.9f);
        public Vector4 CaretColor = new Vector4(1, 1, 1, 1);

        LabelNode label;
        BoxNode caret;
        BoxNode background;

        FontInternal font;

        float caretTimer;
        bool caretVisible = true;

        const float fontSize = 18f;

        public InputFieldNode(FontInternal font)
        {
            this.font = font;
            this.BlockInput = true;
        }

        protected override void InitializeCore()
        {
            background = new BoxNode()
            {
                Color = BackgroundColor,
                ParticipatesInLayout = false
            };

            label = new LabelNode("", font, fontSize)
            {
                ParticipatesInLayout = false
            };

            caret = new BoxNode()
            {
                Size = new Vector2(
                    2,
                    (font.Ascender - font.Descender) * font.GetScaleForFontSize(fontSize)
                ),
                Color = CaretColor,
                ParticipatesInLayout = false
            };

            CaretIndex = Text.Length;

            Add(background);
            Add(label);
            Add(caret);
        }

        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            label.Text = Text;

            Vector2 textSize = label.Measure(
                new Vector2(
                    availableSize.X - Padding.X * 2,
                    float.PositiveInfinity
                )
            );

            return new Vector2(
                availableSize.X,
                textSize.Y + Padding.Y * 2
            );
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            background.Color = BackgroundColor;

            background.Arrange(new UITransform(
                finalRect.position,
                finalRect.size
            ));

            Vector2 contentPos = finalRect.position + Padding;

            Vector2 labelSize = new Vector2(
                finalRect.size.X - Padding.X * 2,
                label.DesiredSize.Y
            );

            label.Arrange(new UITransform(
                contentPos,
                labelSize
            ));

            // Measure caret position based on substring
            string left = CaretIndex > 0
                ? Text.Substring(0, CaretIndex)
                : "";

            float textWidth = finalRect.size.X - Padding.X * 2;

            var opts = TextLayoutOptions.Default;
            opts.MaxWidth = textWidth;
            opts.Overflow = TextOverflow.Clip;

            Vector2 caretOffset =
                TextMeshGenerator.Measure(
                    font,
                    left,
                    font.GetScaleForFontSize(fontSize),
                    opts
                );

            caret.Color = CaretColor;
            caret.Visible = caretVisible;

            caret.Arrange(new UITransform(
                contentPos + new Vector2(caretOffset.X, 0),
                caret.Size ?? new Vector2(2, finalRect.size.Y - Padding.Y * 2)
            ));
        }

        protected override void RenderCore(List<RenderItem> renderList, Matrix4x4 canvasModel)
        {

        }

        public override void OnMouseDown(Vector2 position)
        {
            UISystem.SetFocus(this);
        }

        public override void OnTextInput(char c)
        {
            Text = Text.Insert(CaretIndex, c.ToString());
            CaretIndex++;
        }

        public override void OnKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Backspace:
                    if (CaretIndex > 0)
                    {
                        Text = Text.Remove(CaretIndex - 1, 1);
                        CaretIndex--;
                    }
                    break;

                case Keys.Delete:
                    if (CaretIndex < Text.Length)
                    {
                        Text = Text.Remove(CaretIndex, 1);
                    }
                    break;

                case Keys.Left:
                    if (CaretIndex > 0)
                        CaretIndex--;
                    break;

                case Keys.Right:
                    if (CaretIndex < Text.Length)
                        CaretIndex++;
                    break;

                case Keys.Home:
                    CaretIndex = 0;
                    break;

                case Keys.End:
                    CaretIndex = Text.Length;
                    break;
            }
        }

        public override void OnFocus()
        {
            caretVisible = true;
            caretTimer = 0;
        }

        public override void OnBlur()
        {
            caretVisible = false;
        }

        public void Update(float deltaTime)
        {
            caretTimer += deltaTime;

            if (caretTimer > 0.5f)
            {
                caretVisible = !caretVisible;
                caretTimer = 0;
            }
        }
    }
}