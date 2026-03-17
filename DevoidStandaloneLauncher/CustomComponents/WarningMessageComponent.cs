using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class WarningMessageComponent : Component
    {
        public override string Type => nameof(WarningMessageComponent);

        public float Duration = 4f;

        CanvasComponent canvas;
        LabelNode label;
        FontInternal font;

        float timer = 0f;

        public override void OnStart()
        {
            font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 48);

            // Create canvas object
            GameObject canvasGO = gameObject.Scene.addGameObject("WarningCanvas");

            canvas = canvasGO.AddComponent<CanvasComponent>();
            canvas.RenderMode = CanvasRenderMode.ScreenSpace;
            canvas.PixelsPerUnit = 300;

            // Centered container
            FlexboxNode container = new FlexboxNode()
            {
                Size = new Vector2(600, 200),

                Direction = FlexDirection.Column,
                Align = AlignItems.Center,
                Justify = JustifyContent.Center,

                Offset = new Vector2(Screen.Size.X / 2, 0),
                ParticipatesInLayout = false
            };

            label = new LabelNode("Lights can occasionally break", font, 21f);

            container.Add(label);
            canvas.Canvas.Add(container);
        }

        public override void OnUpdate(float dt)
        {
            timer += dt;

            if (timer >= Duration)
            {
                canvas.gameObject.OnDestroy();
                gameObject.OnDestroy(); // remove this component holder
            }
        }
    }
}