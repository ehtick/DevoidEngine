using DevoidEngine.Engine.Components;
using Elemental.Editor.Attributes;
using Elemental.Editor.CustomEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Elemental.Editor.Utils
{

    public static class ComponentTypeRegistry
    {
        private static readonly Dictionary<Type, CustomEditorScript> _customEditorMap = new();
        private static readonly List<Type> _componentCache = new();

        public static IReadOnlyDictionary<Type, CustomEditorScript> CustomEditors => _customEditorMap;
        public static IReadOnlyList<Type> ComponentTypes => _componentCache;

        static ComponentTypeRegistry()
        {
            CacheCustomEditors();
            CacheComponentTypes();
        }

        private static void CacheCustomEditors()
        {
            var editorTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(CustomEditorScript).IsAssignableFrom(t));

            foreach (var editorType in editorTypes)
            {
                var attr = editorType.GetCustomAttribute<CustomEditor>();
                if (attr != null && attr.target != null)
                {
                    var editor = (CustomEditorScript)Activator.CreateInstance(editorType);
                    _customEditorMap[attr.target] = editor;
                    editor.OnEnable();
                }
            }
        }

        private static void CacheComponentTypes()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Component)))
                .ToList();

            _componentCache.Clear();
            _componentCache.AddRange(allTypes);
        }
    }

}
