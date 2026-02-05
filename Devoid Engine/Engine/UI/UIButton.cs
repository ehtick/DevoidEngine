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
            PanelNode node = new PanelNode()
            {
                Size = new Vector2(100, 100),
            };

            PanelNode node1 = new PanelNode()
            {
                Size = new Vector2(100, 100),
            };

            ContainerNode mainContainer = new ContainerNode()
            {
                MaxSize = new Vector2(300, 300),
                MinSize = new Vector2(200, 200),
                Offset = new Vector2(20, 20),
                Padding = 10
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
