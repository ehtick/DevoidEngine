using DevoidEngine.Engine.UI.Nodes;
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


            BoxNode node = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = new LayoutOptions { FlexGrowMain = 1 }
            };

            BoxNode node1 = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = new LayoutOptions { FlexGrowMain = 1 }
            };




            FlexNode mainContainer = new FlexNode()
            {
                Direction = FlexDirection.Row,
                Gap = 10f,
                Align = AlignItems.Stretch,
                Justify = JustifyContent.Start,
                Layout = new LayoutOptions
                {
                    FlexGrowMain = 1
                }
            };

            mainContainer.Add(node);
            mainContainer.Add(node1);


            Root = mainContainer;
            UISystem.Canvas.Add(Root);
        }

        public override void Update()
        {
            
        }

    }
}
