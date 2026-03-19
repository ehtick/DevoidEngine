using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Animation
{
    public struct ChannelBinding<T>
    {
        public AnimationTrack<T> Track;
        public Action<T> Setter;
    }
}
