using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ComponentCategoryAttribute : Attribute
    {
        public string CategoryName { get; }
        public string HexColor { get; }

        public ComponentCategoryAttribute(string categoryName, string hexColor)
        {
            CategoryName = categoryName;
            HexColor = hexColor;
        }

        public Vector4 GetColor()
        {
            return ColorConverter.FromHex(HexColor);
        }
    }

}
