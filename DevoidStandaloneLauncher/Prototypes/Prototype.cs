using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidStandaloneLauncher.Prototypes
{
    internal class Prototype
    {
        public virtual void OnInit(Scene main) { }
        public virtual void OnUpdate(float delta) { }
        public virtual void OnRender(float delta) { }

    }
}
