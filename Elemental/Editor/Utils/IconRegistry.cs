using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Elemental.Editor.Utils
{
    public class FontIconEntry
    {
        public Vector4 iconColor;
        public string icon;

        public override string ToString() => icon;
    }

    public static class IconRegistry
    {
        private static readonly Dictionary<Type, List<FontIconEntry>> _fontIcons = new();
        //private static readonly Dictionary<Type, Texture> _textureIcons = new();
        //private static Texture _defaultTextureIcon;

        private static readonly FontIconEntry _defaultFontIcon = new()
        {
            iconColor = Colors.AliceBlue,
            icon = BootstrapIconFont.Bug
        };

        static IconRegistry()
        {
            // GameObject icons
            Register<GameObject>(new FontIconEntry { iconColor = Colors.AliceBlue, icon = BootstrapIconFont.Box });
            Register<GameObject>(new FontIconEntry { iconColor = Colors.AliceBlue, icon = BootstrapIconFont.Boxes });

            // --- Transform/Layout Components ---
            Register<Transform>(Category("Transform", BootstrapIconFont.Intersect));
            //Register<RectTransform>(Category("Transform", BootstrapIconFont.BoxArrowRight));
            //Register<AnchorComponent>(Category("Transform", BootstrapIconFont.BoundingBox));
            //Register<CanvasComponent>(Category("Transform", BootstrapIconFont.AspectRatio));

            // --- Rendering ---
            //Register<MeshRenderer>(Category("Rendering", BootstrapIconFont.BoxFill));
            //Register<LightComponent>(Category("Rendering", BootstrapIconFont.Lightbulb));
            //Register<WorldEnvironment>(Category("Rendering", BootstrapIconFont.Sliders));

            // --- Physics ---
            //Register<ColliderComponent3D>(Category("Physics", BootstrapIconFont.App));
            //Register<RigidBody3D>(Category("Physics", BootstrapIconFont.Back));
            //Register<RigidBody2D>(Category("Physics", BootstrapIconFont.Front));

            // --- Animation ---
            //Register<Animator2D>(Category("Animation", BootstrapIconFont.PersonArmsUp));

            // --- Camera ---
            Register<CameraComponent3D>(Category("Camera", BootstrapIconFont.CameraVideo));
        }

        private static FontIconEntry Category(string categoryName, string? iconOverride = null)
        {
            if (!ComponentCategoryRegistry.CategoryInfos.TryGetValue(categoryName, out var info))
            {
                info = ComponentCategoryRegistry.CategoryInfos["Other"];
            }

            return new FontIconEntry
            {
                iconColor = info.IconColor,
                icon = iconOverride ?? info.DefaultIcon ?? BootstrapIconFont.QuestionCircle
            };
        }

        private static void Register<T>(FontIconEntry icon)
        {
            var type = typeof(T);
            if (!_fontIcons.TryGetValue(type, out var list))
                _fontIcons[type] = list = new List<FontIconEntry>();

            list.Add(icon);
        }

        //private static void Register<T>(Texture icon)
        //{
        //    _textureIcons[typeof(T)] = icon;
        //}

        //public static Texture GetTextureIcon(Type type)
        //{
        //    return _textureIcons.TryGetValue(type, out var tex) ? tex : _defaultTextureIcon;
        //}

        public static FontIconEntry GetFontIcon(Type type, int state = 0)
        {
            if (_fontIcons.TryGetValue(type, out var icons) && state < icons.Count)
                return icons[state];

            // Try to infer from attribute and category registry
            var categoryName = ComponentCategoryRegistry.GetCategoryFor(type);
            var info = ComponentCategoryRegistry.GetCategoryInfo(type);

            return new FontIconEntry
            {
                iconColor = info.IconColor,
                icon = info.DefaultIcon ?? BootstrapIconFont.QuestionCircle
            };
        }
    }
}
