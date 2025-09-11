using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
