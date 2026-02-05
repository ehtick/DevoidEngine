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

        protected readonly List<UINode> _children = new();



        // Size intent
        public Vector2 Offset = Vector2.Zero;
        public Vector2? Size;
        public Vector2 MinSize = Vector2.Zero;
        public Vector2 MaxSize = new(float.PositiveInfinity);

        public bool ParticipatesInLayout = true;

        public void Add(UINode child)
        {
            _children.Add(child);
        }

        // ENTRY POINT
        public Vector2 Measure(Vector2 availableSize)
        {
            if (!Visible)
                return Vector2.Zero;

            Vector2 desired = MeasureCore(availableSize);

            if (Size.HasValue)
                desired = Size.Value;

            desired.X = Math.Clamp(desired.X, MinSize.X, MaxSize.X);
            desired.Y = Math.Clamp(desired.Y, MinSize.Y, MaxSize.Y);

            desired.X = Math.Min(desired.X, availableSize.X);
            desired.Y = Math.Min(desired.Y, availableSize.Y);

            DesiredSize = desired;
            return desired;
        }

        public void Arrange(UITransform finalRect)
        {
            Rect = finalRect;

            if (!Visible)
                return;

            ArrangeCore(finalRect);
        }

        // OVERRIDES
        protected abstract Vector2 MeasureCore(Vector2 availableSize);
        protected abstract void ArrangeCore(UITransform finalRect);
    }

}
