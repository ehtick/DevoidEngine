namespace Elemental.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    class CustomEditor : Attribute
    {
        public Type target;

        public CustomEditor(Type component)
        {
            this.target = component;
        }

    }
}
