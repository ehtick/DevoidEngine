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
                Size = new Vector2(100, 100)
            };
            Root = node;

            PanelNode childNode = new PanelNode()
            {
                
                Size = new Vector2(70, 70)
            };
            node.Add(childNode);

            PanelNode childNode1 = new PanelNode()
            {

                Size = new Vector2(50, 50)
            };
            childNode.Add(childNode1);

            UISystem.Canvas.Add(Root);
        }

        public override void Update()
        {
            
        }

    }
}
