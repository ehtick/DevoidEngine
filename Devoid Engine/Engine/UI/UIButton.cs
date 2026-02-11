using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI
{
    public class UIButton : UIElement
    {
        public UINode Root;
        public override void Setup()
        {


            //BoxNode node = new BoxNode()
            //{
            //    Size = new Vector2(50, 100),
            //    Layout = new LayoutOptions { FlexGrowMain = 1 }
            //};

            //BoxNode node1 = new BoxNode()
            //{
            //    Size = new Vector2(100, 20),
            //    Layout = new LayoutOptions { FlexGrowMain = 0 }
            //};

            //BoxNode node2 = new BoxNode()
            //{
            //    Size = new Vector2(100, 100),
            //    Layout = new LayoutOptions { FlexGrowMain = 1 }
            //};



            //FlexboxNode mainContainer = new FlexboxNode()
            //{
            //    Offset = new Vector2(50, 50),
            //    ParticipatesInLayout = false,

            //    Direction = FlexDirection.Column,
            //    Gap = 0f,
            //    Align = AlignItems.Stretch,
            //    Justify = JustifyContent.Start,
            //    Layout = new LayoutOptions
            //    {
            //        FlexGrowMain = 1
            //    }
            //};

            //mainContainer.Add(node);
            //mainContainer.Add(node1);
            //mainContainer.Add(node2);

            font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 32);

            BoxNode headerContainer = new BoxNode()
            {
                Size = new Vector2(0, 50), // height fixed, width flexible
                Layout = new LayoutOptions()
                {
                    FlexGrowMain = 0
                },
                Align = AlignItems.Stretch
            };


            LabelNode label = new LabelNode("Hey!", font, 0.5f)
            {
                Size = new Vector2(50, 50),
                Layout = new LayoutOptions() { FlexGrowMain = 0 }
            };

            FlexboxNode innerFlex = new FlexboxNode()
            {
                Direction = FlexDirection.Row,
                Align = AlignItems.Center,
                Justify = JustifyContent.Center,
                Layout = new LayoutOptions()
                {
                    FlexGrowMain = 1
                }
            };

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

            innerFlex.Add(node1);
            //innerFlex.Add(node2);
            //innerFlex.Add(label);

            headerContainer.Add(innerFlex);

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
                Size = new Vector2(200, 200),

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

            Root = mainContainer;
            UISystem.Canvas.Add(Root);

            font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 32);
            //font = FontLibrary.LoadFont("C:/Windows/Fonts/Arial.ttf", 32);
            mesh = TextMeshGenerator.Generate(font, (text), font.GetScaleForFontSize(50));

            UISystem.OnRender += Update;
        }

        Mesh mesh;
        FontInternal font;

        public override void Update()
        {
            //UIRenderer.DrawText(new UITransform(new Vector2(10), new Vector2(1))
            //    , mesh,font.Atlas.GPUTexture
            //);
        }

        string text = @"Hey!";

    }
}
