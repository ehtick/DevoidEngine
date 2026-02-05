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
                Layout = new LayoutOptions { FlexGrowMain = 0 }
            };

            BoxNode node1 = new BoxNode()
            {
                MinSize = new Vector2(40, 200),
                Layout = new LayoutOptions { FlexGrowMain = 0 }
            };



            FlexNode mainContainer = new FlexNode()
            {
                Direction = FlexDirection.Column,
                Gap = 0f,
                Offset = new Vector2(20, 20),
                Size = new Vector2(150, 300),
                Align = AlignItems.Stretch,
                Justify = JustifyContent.Start
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
