using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DevoidEngine.Engine.Core
{
    public class DebugConsole : Layer
    {
        public CanvasNode rootNode = new CanvasNode()
        {
            
        };
        public RenderState debugRenderState = new RenderState()
        {
            BlendMode = DevoidGPU.BlendMode.AlphaBlend,
            CullMode = DevoidGPU.CullMode.None,
            DepthTest = DevoidGPU.DepthTest.Disabled,
            DepthWrite = false,
            FillMode = DevoidGPU.FillMode.Solid,
            PrimitiveType = DevoidGPU.PrimitiveType.Triangles
        };

        FontInternal font;

        FlexboxNode consolePanel;
        FlexboxNode logArea;
        InputFieldNode inputLabel;

        List<string> logs = new();
        string currentInput = "";

        public override void OnAttach()
        {
            font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 20);

            UISystem.Roots.Add(rootNode);

            consolePanel = new FlexboxNode()
            {
                ParticipatesInLayout = false,
                Offset = new Vector2(100, 30),
                Size = new Vector2(600, 350),
                Direction = FlexDirection.Column,
                //Gap = 4,
                Justify = JustifyContent.Center
            };

            // background
            BoxNode background = new BoxNode()
            {
                Size = new Vector2(600, 350),
                Color = new Vector4(0, 1, 0, 0.2f)
            };

            logArea = new FlexboxNode()
            {
                Direction = FlexDirection.Column,
                Layout = new LayoutOptions()
                {
                    FlexGrowMain = 1
                },
                Gap = 2
            };

            inputLabel = new InputFieldNode(font)
            {
                Text = ">Input pending."
            };

            consolePanel.Add(background);
            consolePanel.Add(inputLabel);

            rootNode.Add(consolePanel);
        }

        public override void OnDetach()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            
        }

        public override void OnLateRender()
        {
            List<RenderItem> renderItems = new List<RenderItem>();
            rootNode.Render(renderItems, Matrix4x4.Identity);

            Framebuffer surface = SceneManager.CurrentScene.GetMainCamera().Camera.RenderTarget;

            surface.Bind();
            RenderBase.SetupCamera(UIRenderer.ScreenData);
            RenderBase.Execute(renderItems, debugRenderState);

        }
    }
}
