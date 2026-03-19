using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Animation
{
    public class AnimationChannel<T>
    {
        public string Path;
        public AnimationTrack<T> Track;
    }
}
