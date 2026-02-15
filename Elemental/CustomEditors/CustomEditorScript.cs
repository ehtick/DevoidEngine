using DevoidEngine.Engine.Components;

namespace Elemental.Editor.CustomEditors
{
    public class CustomEditorScript
    {
        public Component component;

        public virtual void OnEnable() { }

        public virtual void OnGUI() { }
    }
}