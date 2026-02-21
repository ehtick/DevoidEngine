using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Physics
{
    public interface ICollisionListener
    {
        void OnCollisionEnter(GameObject other);
        void OnCollisionStay(GameObject other);
        void OnCollisionExit(GameObject other);
    }
}
