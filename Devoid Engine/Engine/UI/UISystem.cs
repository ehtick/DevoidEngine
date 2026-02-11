using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI
{
    public static class UISystem
    {
        public static UINode Canvas;
        public static event Action? OnRender;

        static UISystem()
        {
            Canvas = new CanvasNode()
            {
                Direction = FlexDirection.Row,
                Align = AlignItems.Stretch,
                Justify = JustifyContent.Start
            };
        }



        public static void PerformUI()
        {
            UIRenderer.BeginRender();
            OnRender.Invoke();
            Canvas.Measure(Screen.Size);
            Canvas.Arrange(new UITransform(Vector2.Zero, Screen.Size));
            UIRenderer.EndRender();
        }
    }
}
