using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public abstract class UINode
    {
        public UITransform Rect { get; protected set; }
        public Vector2 DesiredSize { get; private set; }

        public bool Visible = true;
        public bool Interactable = true;

        protected readonly List<UINode> _children = new List<UINode>();

        public Vector2? Size;                 // Explicit width/height
        public Vector2 MinSize = Vector2.Zero;
        public Vector2 MaxSize = new(float.PositiveInfinity);

        public void Add(UINode child)
        {
            _children.Add(child);
        }

        public virtual Vector2 Measure(Vector2 availableSize)
        {
            // Default: take the max of children
            Vector2 size = Vector2.Zero;

            foreach (var c in _children)
            {
                Vector2 childSize = c.Measure(availableSize);
                size.X = MathF.Max(size.X, childSize.X);
                size.Y = MathF.Max(size.Y, childSize.Y);
            }
            return size;
        }

        public virtual void Arrange(UITransform finalRect)
        {
            Rect = finalRect;

            foreach (var c in _children)
            {
                c.Arrange(finalRect);
            }
        }
    }
}
