using DevoidEngine.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemental.Editor.CustomEditors
{
    public class CustomEditorScript
    {
        public Component component;

        public virtual void OnEnable() { }

        public virtual void OnGUI() { }
    }
}