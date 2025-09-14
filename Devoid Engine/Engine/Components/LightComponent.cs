using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class LightComponent : Component
    {
        public override string Type => nameof(LightComponent);

        private PointLight pointLight;
        private SpotLight spotLight;
        private DirectionalLight directionalLight;

        private bool enabled = true;
        private Vector3 color = Vector3.One;
        private float intensity = 10;
        private float radius = 30;

        private float outerCutoff = 1f;
        private float innerCutoff = 1f;
        private LightType lightType = LightType.PointLight;
        private LightAttenuationType attenuationType = LightAttenuationType.Custom;
        private float linearFactor = 0.7f;
        private float quadraticFactor = 1.8f;

        public LightType LightType
        {
            get => lightType;
            set
            {
                if (lightType != value)
                {
                    OnChangeLightType(lightType, value);
                    lightType = value;
                }
            }
        }

        public bool Enabled
        {
            get => enabled;
            set { enabled = value; OnChangedValue(); }
        }

        public Vector4 Color
        {
            get => new Vector4(color, 1.0f);
            set { color = new Vector3(value.X, value.Y, value.Z); OnChangedValue(); }
        }

        public float Radius
        {
            get => radius;
            set { radius = value; OnChangedValue(); }
        }

        public float Intensity
        {
            get => intensity;
            set { intensity = value; OnChangedValue(); }
        }

        public LightAttenuationType AttenuationFunction
        {
            get => attenuationType;
            set { attenuationType = value; OnChangedValue(); }
        }

        public float LinearFactor
        {
            get => linearFactor;
            set { linearFactor = value; OnChangedValue(); }
        }

        public float QuadraticFactor
        {
            get => quadraticFactor;
            set { quadraticFactor = value; OnChangedValue(); }
        }

        public float InnerCutoff
        {
            get => MathHelper.RadToDeg(innerCutoff);
            set { innerCutoff = MathHelper.DegToRad(value); OnChangedValue(); }
        }

        public float OuterCutoff
        {
            get => MathHelper.RadToDeg(outerCutoff);
            set { outerCutoff = MathHelper.DegToRad(value); OnChangedValue(); }
        }

        public LightComponent()
        {
            // No light creation here; deferred to OnStart
        }

        private void DisposeLights()
        {
            if (pointLight != null)
            {
                LightManager.RemovePointLight(pointLight);
                pointLight = null;
            }

            if (spotLight != null)
            {
                LightManager.RemoveSpotLight(spotLight);
                spotLight = null;
            }

            if (directionalLight != null)
            {
                LightManager.RemoveDirectionalLight();
                directionalLight = null;
            }
        }

        public void OnChangeLightType(LightType previousValue, LightType newValue)
        {
            DisposeLights();
            lightType = newValue;
            OnStart(); // Recreate the new light
        }

        public void OnChangedValue()
        {
            switch (lightType)
            {
                case LightType.PointLight:
                    if (pointLight != null)
                    {
                        pointLight.position = gameObject.transform.Position;
                        pointLight.color = color;
                        pointLight.range = radius;
                        pointLight.intensity = intensity;
                        pointLight.enabled = enabled;
                        pointLight.attenuationType = (int)attenuationType;
                        pointLight.attenuationParameters = new Vector2(linearFactor, quadraticFactor);
                    }
                    break;

                case LightType.SpotLight:
                    if (spotLight != null)
                    {
                        spotLight.position = gameObject.transform.Position;
                        spotLight.color = color;
                        spotLight.range = radius;
                        spotLight.intensity = intensity;
                        spotLight.enabled = enabled;
                        spotLight.innerCutoff = innerCutoff;
                        spotLight.outerCutoff = outerCutoff;
                        spotLight.direction = Vector3.Normalize(gameObject.transform.Rotation);
                    }
                    break;

                case LightType.DirectionalLight:
                    if (directionalLight != null)
                    {
                        directionalLight.direction = gameObject.transform.Rotation;
                        directionalLight.color = color;
                        directionalLight.intensity = intensity;
                        directionalLight.enabled = enabled;
                    }
                    break;
            }
        }

        public override void OnStart()
        {
            DisposeLights(); // Just in case

            Vector3 position = gameObject.transform.Position;

            switch (lightType)
            {
                case LightType.PointLight:
                    pointLight = LightManager.AddPointLight(position, color, intensity, radius);
                    break;

                case LightType.SpotLight:
                    spotLight = LightManager.AddSpotLight(
                        position, color,
                        Vector3.Normalize(gameObject.transform.Rotation),
                        intensity, radius, innerCutoff, outerCutoff);
                    break;

                case LightType.DirectionalLight:
                    directionalLight = LightManager.AddDirectionalLight(
                        gameObject.transform.Rotation, color, intensity);
                    break;
            }

            OnChangedValue(); // Sync properties after light is created
        }

        public override void OnUpdate(float dt)
        {
            if (gameObject.transform.hasMoved)
            {
                OnChangedValue();
            }
        }

        public override void OnDestroy()
        {
            DisposeLights();
        }
    }
}
