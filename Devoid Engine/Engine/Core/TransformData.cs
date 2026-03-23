using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public struct TransformData
    {
        public Vector3 PrevPos, CurrPos;
        public Quaternion PrevRot, CurrRot;
        public Vector3 PrevScale, CurrScale;
    }
}
