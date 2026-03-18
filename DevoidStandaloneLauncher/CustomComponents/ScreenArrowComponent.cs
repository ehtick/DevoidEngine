using Assimp;
using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Components
{
    public class ScreenArrowComponent : Component
    {
        public override string Type => nameof(ScreenArrowComponent);

        public Texture2D ArrowTexture;
        public GameObject TargetObject;

        ContainerNode arrow;
        Vector2 arrowPositionOnScreen = new Vector2(10, 10);

        public override void OnStart()
        {
            CanvasComponent canvas = gameObject.AddComponent<CanvasComponent>();
            canvas.Canvas.Justify = JustifyContent.Center;
            canvas.Canvas.Align = AlignItems.Center;

            arrow = new ContainerNode()
            {
                ParticipatesInLayout = true,
                Texture = ArrowTexture,
                Offset = arrowPositionOnScreen,
                Size = new System.Numerics.Vector2(100, 100),
            };

            canvas.Canvas.Add(arrow);
        }


        float timer = 0;
        public override void OnUpdate(float dt)
        {
            if (TargetObject == null) return;
            var camera = gameObject.Scene.GetMainCamera().Camera;
            var result = camera.WorldToScreen(TargetObject.transform.Position, Screen.Size.X, Screen.Size.Y);

            Vector2 dir = result - arrowPositionOnScreen;
            dir = Vector2.Normalize(dir);
            float angle = (float)Math.Atan2(dir.Y, dir.X);
            arrow.Rotation = angle;
        }
    }
}
