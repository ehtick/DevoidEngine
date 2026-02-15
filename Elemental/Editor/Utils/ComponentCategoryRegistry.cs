using DevoidEngine.Engine.Attributes;
using DevoidEngine.Engine.Components;
using System.Numerics;
using System.Reflection;

namespace Elemental.Editor.Utils
{
    public class CategoryInfo
    {
        public Vector4 IconColor { get; set; }
        public string DisplayName { get; set; }
        public string? DefaultIcon { get; }

        public CategoryInfo(Vector4 color, string label, string? defaultIcon = null)
        {
            IconColor = color;
            DisplayName = label;
            DefaultIcon = defaultIcon;
        }
    }

    public static class ComponentCategoryRegistry
    {
        // Mapping from component Type to its string category
        private static readonly Dictionary<Type, string> _typeToCategory = new();

        // Mapping from string category name to CategoryInfo
        public static readonly Dictionary<string, CategoryInfo> CategoryInfos = new()
        {
            ["Transform"] = new(Colors.ForestGreen, "Transform/Layout", BootstrapIconFont.ArrowsMove),
            ["Rendering"] = new(Colors.DodgerBlue, "Rendering", BootstrapIconFont.Film),
            ["Physics"] = new(Colors.OrangeRed, "Physics", BootstrapIconFont.Magnet),
            ["UI"] = new(Colors.MediumSeaGreen, "UI", BootstrapIconFont.UiChecks),
            ["Animation"] = new(Colors.MediumPurple, "Animation", BootstrapIconFont.CollectionPlay),
            ["Camera"] = new(Colors.CadetBlue, "Camera", BootstrapIconFont.CameraVideo),
            ["Environment"] = new(Colors.SkyBlue, "Environment", BootstrapIconFont.Globe),
            ["Other"] = new(Colors.Gray, "Other", BootstrapIconFont.Gear)
        };

        // Pre-register known types to known categories
        static ComponentCategoryRegistry()
        {
            RegisterCategory<Transform>("Transform");
            //RegisterCategory<RectTransform>("Transform");
            //RegisterCategory<AnchorComponent>("Transform");
            //RegisterCategory<CanvasComponent>("Transform");

            RegisterCategory<MeshRenderer>("Rendering");
            RegisterCategory<LightComponent>("Rendering");

            //RegisterCategory<ColliderComponent3D>("Physics");
            //RegisterCategory<RigidBody3D>("Physics");
            //RegisterCategory<RigidBody2D>("Physics");

            //RegisterCategory<Animator2D>("Animation");

            RegisterCategory<CameraComponent3D>("Camera");

            //RegisterCategory<WorldEnvironment>("Environment");
        }

        public static void RegisterCategory<T>(string categoryName)
        {
            _typeToCategory[typeof(T)] = categoryName;
        }

        public static string GetCategoryFor(Type type)
        {

            var attr = type.GetCustomAttribute<ComponentCategoryAttribute>();
            if (attr != null)
            {
                string name = attr.CategoryName;

                if (!CategoryInfos.ContainsKey(name))
                {
                    CategoryInfos[name] = new CategoryInfo(attr.GetColor(), name);
                }

                _typeToCategory[type] = name;
                return name;
            }
            else
            {
                if (_typeToCategory.TryGetValue(type, out var category))
                    return category;
            }

            _typeToCategory[type] = "Other";
            return "Other";
        }

        public static CategoryInfo GetCategoryInfo(Type type)
        {
            var category = GetCategoryFor(type);
            if (CategoryInfos.TryGetValue(category, out var info))
                return info;

            return CategoryInfos["Other"];
        }

        public static IEnumerable<string> GetAllCategoryNames() => CategoryInfos.Keys;
    }
}
