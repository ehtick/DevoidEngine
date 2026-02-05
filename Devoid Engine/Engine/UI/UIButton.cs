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
            int counter = 1;

            BoxNode node = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = LayoutOptions.Default
            };

            BoxNode node1 = new BoxNode()
            {
                MinSize = new Vector2(40, 200),
                Layout = new LayoutOptions() { ExpandV = false }
            };

            HContainer hnode = new HContainer()
            {
                Size = new Vector2(150, 300),
                MinSize = new Vector2(150, 50),
            };

            BoxNode node2 = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = LayoutOptions.Default
            };

            BoxNode node3 = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = LayoutOptions.Default
            };

            BoxNode node4 = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = LayoutOptions.Default
            };

            VContainer vnode = new VContainer()
            {
                Layout = LayoutOptions.Default
            };

            BoxNode node5 = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = LayoutOptions.Default
            };

            BoxNode node6 = new BoxNode()
            {
                Size = new Vector2(100, 100),
                Layout = LayoutOptions.Default
            };

            VContainer mainContainer = new VContainer()
            {
                Offset = new Vector2(20, 20),
                Size = new Vector2(150, 300),
                
                childGap = 0f,
            };
            mainContainer.Add(node);
            mainContainer.Add(node1);

            hnode.Add(node2);
            hnode.Add(node3);
            hnode.Add(node4);
            hnode.Add(vnode);

            vnode.Add(node5);
            vnode.Add(node6);

            mainContainer.Add(hnode);

            Root = mainContainer;

            UISystem.Canvas.Add(Root);
        }

        public override void Update()
        {
            
        }

    }
}
