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
        BoxNode foreground;

        FontInternal font;

        public Action<string>? OnSubmit;


        float caretTimer;
        bool caretVisible = true;

        const float fontSize = 16f;

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

            foreground = new BoxNode()
            {
                Color = new Vector4(1, 0, 0, 1),
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
            //Add(foreground);
        }

        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            label.Text = Text;

            float availableWidth = availableSize.X;
            float textMaxWidth = availableWidth - (Padding.X * 2);
            if (textMaxWidth < 0) textMaxWidth = 0;

            // 1. Tell the label to measure itself using the constrained width
            label.Measure(new Vector2(textMaxWidth, float.PositiveInfinity));

            // 2. The label now knows its own height (including your empty-text line-height logic!)
            float requiredHeight = label.DesiredSize.Y + (Padding.Y * 2);

            return new Vector2(availableWidth, requiredHeight);
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            float textMaxWidth = finalRect.size.X - (Padding.X * 2);
            if (textMaxWidth < 0) textMaxWidth = 0;

            // 1. We might need to re-measure the label if the Arrange width is 
            // strictly different from the Measure width.
            label.Measure(new Vector2(textMaxWidth, float.PositiveInfinity));

            float requiredHeight = label.DesiredSize.Y + (Padding.Y * 2);

            // 2. Arrange Background (Width of container, Height of Label + Padding)
            background.Color = BackgroundColor;
            background.Arrange(new UITransform(
                finalRect.position,
                new Vector2(finalRect.size.X, requiredHeight)
            ));

            // 3. Arrange Label
            Vector2 contentPos = finalRect.position + Padding;
            label.Arrange(new UITransform(
                contentPos,
                // Give it the full textMaxWidth, NOT the shrink-wrapped label.DesiredSize.X!
                new Vector2(textMaxWidth, label.DesiredSize.Y)
            ));

            // 4. Arrange Caret (Using our new cleanly encapsulated method!)
            Vector2 caretOffset = label.GetCursorPosition(CaretIndex, textMaxWidth);

            caret.Color = CaretColor;
            caret.Visible = caretVisible;
            caret.Arrange(new UITransform(
                contentPos + new Vector2(caretOffset.X, caretOffset.Y),
                caret.Size ?? new Vector2(2, (font.Ascender - font.Descender) * font.GetScaleForFontSize(fontSize))
            ));
        }

        protected override void RenderCore(List<RenderItem> renderList, Matrix4x4 canvasModel)
        {
        }

        protected override void UpdateCore(float deltaTime)
        {
            caretTimer += deltaTime;

            if (caretTimer > 0.8f)
            {
                caretVisible = !caretVisible;
                caretTimer = 0;
            }
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
                case Keys.Enter:
                    OnSubmit?.Invoke(Text);
                    Text = "";
                    CaretIndex = 0;
                    break;
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
    }
}