using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public struct TransformSnapshot
    {
        public uint Id; // 0 is invalid
        public uint parentId; // 0 is invalid

        public Vector3 PrevLocalPosition;
        public Quaternion PrevLocalRotation;
        public Vector3 PrevLocalScale;

        public Vector3 CurrLocalPosition;
        public Quaternion CurrLocalRotation;
        public Vector3 CurrLocalScale;
    }
}
