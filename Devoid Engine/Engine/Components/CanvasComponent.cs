using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.UI;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class CanvasComponent : Component, IRenderComponent
    {
        public override string Type => nameof(CanvasComponent);

        CanvasNode Canvas = new CanvasNode()
        {
            Direction = FlexDirection.Row,
            Align = AlignItems.Stretch,
            Justify = JustifyContent.Start
        };

        public void Collect(CameraComponent3D camera, CameraRenderContext viewData)
        {
            Canvas.Render(viewData.renderItemsUI);
        }

        public override void OnStart()
        {
            UISystem.Roots.Add(Canvas);

            //Setup();

            base.OnStart();
        }

        public void Setup()
        {

            FontInternal font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 32);
            //font = FontLibrary.LoadFont("C:/Windows/Fonts/HARLOWSI.ttf", 32);

            FlexboxNode headerContainer = new FlexboxNode()
            {
                Size = new Vector2(50, 50),

                Direction = FlexDirection.Column,
                Align = AlignItems.Center,
                Justify = JustifyContent.Center,
                Layout = new LayoutOptions()
                {
                    FlexGrowMain = 1
                }
            };



            LabelNode label = new LabelNode("Hey!\u0124", font, 20f)
            {
                Layout = new LayoutOptions() { FlexGrowMain = 0 }
            };

            //FlexboxNode innerFlex = new FlexboxNode()
            //{
            //    Direction = FlexDirection.Column,
            //    Align = AlignItems.End,
            //    Justify = JustifyContent.End,
            //    Layout = new LayoutOptions()
            //    {
            //        FlexGrowMain = 0
            //    }
            //};

            TexNode node1 = new TexNode()
            {
                Size = new Vector2(50, 50),
                texture = Helper.loadImageAsTex("Engine/Content/Textures/shrk.png", DevoidGPU.TextureFilter.Nearest),
                Layout = new LayoutOptions()
                {
                    FlexGrowMain = 0
                }
            };

            TexNode node2 = new TexNode()
            {
                Size = new Vector2(50, 50),
                texture = Helper.loadImageAsTex("Engine/Content/Textures/shrk.png", DevoidGPU.TextureFilter.Nearest),
                Layout = new LayoutOptions()
                {
                    FlexGrowMain = 0
                }
            };

            //innerFlex.Add(node1);
            //innerFlex.Add(node2);
            //innerFlex.Add(label);

            headerContainer.Add(label);

            List<BoxNode> nodes = new List<BoxNode>();

            for (int i = 0; i < 3; i++)
            {
                BoxNode nodeEx = new BoxNode()
                {
                    Size = new Vector2(50, 50),
                    Layout = new LayoutOptions()
                    {
                        FlexGrowMain = 0
                    }

                };

                nodes.Add(nodeEx);
            }

            FlexboxNode mainContainer = new FlexboxNode()
            {
                Size = new Vector2(200, 300),

                Offset = new Vector2(10, 10),
                ParticipatesInLayout = false,

                Direction = FlexDirection.Column,
                Align = AlignItems.Stretch,
                Justify = JustifyContent.Center,
                Gap = 10
            };

            mainContainer.Add(headerContainer);

            for (int i = 0; i < nodes.Count; i++)
            {
                mainContainer.Add(nodes[i]);
            }

            Canvas.Add(mainContainer);
        }

        public override void OnUpdate(float dt)
        {

        }

    }
}
