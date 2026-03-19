using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.UI.Text;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class ScreenArrowComponent : Component
    {
        public override string Type => nameof(ScreenArrowComponent);

        public Texture2D ArrowTexture;
        public GameObject TargetObject;

        private ContainerNode container;
        private ContainerNode arrow;
        private LabelNode text;

        private Vector4 baseContainerColor = new Vector4(0, 0, 0, 0.4f);

        FontInternal font;

        private float lifecycleAlpha = 0f;
        private float fadeDuration = 0.25f;
        private bool initialized = false;

        private enum FadeState
        {
            Hidden,
            FadingIn,
            Visible,
            FadingOut
        }

        private FadeState fadeState = FadeState.Hidden;
        private float fadeTimer = 0f;

        public override void OnStart()
        {
            if (!initialized)
                Setup();
        }

        void Setup()
        {
            font = FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 48);

            CanvasComponent canvas = gameObject.AddComponent<CanvasComponent>();

            container = new ContainerNode()
            {
                ParticipatesInLayout = false,
                Justify = JustifyContent.SpaceEvenly,
                Align = AlignItems.Center,
                Padding = new Padding()
                {
                    Top = 10,
                    Bottom = 10,
                    Right = 15,
                    Left = 10
                },
                Gap = 5,
                Color = baseContainerColor,
                Visible = false // ✅ start hidden
            };

            arrow = new ContainerNode()
            {
                Texture = ArrowTexture,
                Size = new Vector2(75, 75),
            };

            text = new LabelNode("Interact", font, 20);

            container.Add(arrow);
            container.Add(text);

            canvas.Canvas.Add(container);
            initialized = true;
        }

        public void StartIndicator()
        {
            if (!initialized)
                Setup();
            fadeState = FadeState.FadingIn;
            fadeTimer = 0f;
            container.Visible = true;
        }

        public void StopIndicator()
        {
            fadeState = FadeState.FadingOut;
            fadeTimer = 0f;
        }

        public override void OnUpdate(float dt)
        {
            if (TargetObject == null) return;

            var camera = gameObject.Scene.GetMainCamera().Camera;

            Vector2 screen = Screen.Size;
            Vector2 center = screen * 0.5f;

            Vector3 screenPoint = camera.WorldToScreen(
                TargetObject.transform.Position,
                screen.X,
                screen.Y
            );

            Vector2 target = new Vector2(screenPoint.X, screenPoint.Y);
            float w = screenPoint.Z;

            // --- Depth fade ---
            float fadeStart = 4.0f;
            float fadeEnd = 0.0f;

            float depthAlpha = Math.Clamp(
                (w - fadeEnd) / (fadeStart - fadeEnd),
                0f, 1f
            );

            depthAlpha = depthAlpha * depthAlpha * (3 - 2 * depthAlpha);

            // --- Lifecycle fade ---
            switch (fadeState)
            {
                case FadeState.FadingIn:
                    fadeTimer += dt;
                    lifecycleAlpha = Math.Clamp(fadeTimer / fadeDuration, 0f, 1f);
                    lifecycleAlpha = lifecycleAlpha * lifecycleAlpha * (3 - 2 * lifecycleAlpha);

                    if (lifecycleAlpha >= 1f)
                    {
                        lifecycleAlpha = 1f;
                        fadeState = FadeState.Visible;
                    }
                    break;

                case FadeState.FadingOut:
                    fadeTimer += dt;
                    lifecycleAlpha = 1f - Math.Clamp(fadeTimer / fadeDuration, 0f, 1f);
                    lifecycleAlpha = lifecycleAlpha * lifecycleAlpha * (3 - 2 * lifecycleAlpha);

                    if (lifecycleAlpha <= 0f)
                    {
                        lifecycleAlpha = 0f;
                        fadeState = FadeState.Hidden;
                        container.Visible = false;
                        return;
                    }
                    break;

                case FadeState.Visible:
                    lifecycleAlpha = 1f;
                    break;

                case FadeState.Hidden:
                    return;
            }

            float finalAlpha = depthAlpha * lifecycleAlpha;

            container.Color = new Vector4(
                baseContainerColor.X,
                baseContainerColor.Y,
                baseContainerColor.Z,
                baseContainerColor.W * finalAlpha
            );

            arrow.Color = new Vector4(1, 1, 1, finalAlpha);
            text.Color = new Vector4(1, 1, 1, finalAlpha);

            // --- Positioning ---
            Vector2 delta = target - center;

            if (delta.LengthSquared() < 0.0001f)
                return;

            Vector2 dir = Vector2.Normalize(delta);

            Vector2 size = container.Rect.size;
            Vector2 halfSize = size * 0.5f;

            float offsetY = 80f;

            Vector2 objectPos = new Vector2(
                target.X,
                target.Y + offsetY
            );

            Vector2 half = screen * 0.5f;

            float padding = 20f;

            float a = half.X - padding - halfSize.X;
            float b = half.Y - padding - halfSize.Y;

            float denom =
                (dir.X * dir.X) / (a * a) +
                (dir.Y * dir.Y) / (b * b);

            float t = 1.0f / MathF.Sqrt(denom);

            Vector2 ellipsePos = center + dir * t;

            float dist = delta.Length();
            float ellipseRadius = (ellipsePos - center).Length();

            float normalized = dist / ellipseRadius;

            float blendStart = 0.8f;
            float blendEnd = 1.05f;

            float blend = Math.Clamp(
                (normalized - blendStart) / (blendEnd - blendStart),
                0f, 1f
            );

            blend = blend * blend * (3 - 2 * blend);

            Vector2 finalPos = Vector2.Lerp(objectPos, ellipsePos, blend);

            finalPos = new Vector2(
                Math.Clamp(finalPos.X, halfSize.X, screen.X - halfSize.X),
                Math.Clamp(finalPos.Y, halfSize.Y, screen.Y - halfSize.Y)
            );

            container.Offset = finalPos - halfSize;
        }
    }
}