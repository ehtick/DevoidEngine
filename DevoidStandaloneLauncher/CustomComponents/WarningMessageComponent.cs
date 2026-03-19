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
            GameObject canvasGO = gameObject;// gameObject.Scene.addGameObject("WarningCanvas");

            canvas = canvasGO.AddComponent<CanvasComponent>();
            canvas.RenderMode = CanvasRenderMode.WorldSpace;
            canvas.PixelsPerUnit = 300;

            // Centered container
            ContainerNode container = new ContainerNode()
            {
                Size = new Vector2(600, 200),
                Color = new Vector4(0.5f, 0.18f, 0, 0.2f),

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

            gameObject.Transform.Position = new Vector3(0, (float)Math.Sin(timer) * 5, 0);

            //if (timer >= Duration)
            //{
            //    canvas.gameObject.OnDestroy();
            //    gameObject.OnDestroy(); // remove this component holder
            //}
        }
    }
}