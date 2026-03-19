using DevoidEngine.Engine.Rendering;

namespace DevoidEngine.Engine.Components
{
    public class LightFlickerComponent : Component
    {
        public override string Type => nameof(LightFlickerComponent);

        LightComponent light;

        public float BaseIntensity = 10f;

        // normal flicker
        public float FlickerAmount = 1.5f;
        public float FlickerSpeed = 2.5f;

        // blackout behaviour
        public float BlackoutChance = 0.05f;   // chance per second
        public float MinBlackoutTime = 0.5f;
        public float MaxBlackoutTime = 2f;

        float time;
        float blackoutTimer = 0f;
        bool isBlackout = false;

        Random random = new Random();

        public override void OnStart()
        {
            light = gameObject.GetComponent<LightComponent>();

            if (light == null || light.LightType != LightType.PointLight)
                return;

            BaseIntensity = light.Intensity;
        }

        public override void OnUpdate(float dt)
        {
            if (light == null || light.LightType != LightType.PointLight)
                return;

            time += dt;

            // BLACKOUT STATE
            if (isBlackout)
            {
                blackoutTimer -= dt;

                light.Intensity = 0f;

                if (blackoutTimer <= 0f)
                {
                    isBlackout = false;
                }

                return;
            }

            // chance to enter blackout
            if (random.NextDouble() < BlackoutChance * dt)
            {
                isBlackout = true;
                blackoutTimer = RandomRange(MinBlackoutTime, MaxBlackoutTime);
                return;
            }

            // slow subtle flicker
            float noise =
                MathF.Sin(time * FlickerSpeed) * 0.6f +
                MathF.Sin(time * FlickerSpeed * 0.47f) * 0.4f;

            float intensity = BaseIntensity + noise * FlickerAmount;

            light.Intensity = MathF.Max(0f, intensity);
        }

        float RandomRange(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
    }
}