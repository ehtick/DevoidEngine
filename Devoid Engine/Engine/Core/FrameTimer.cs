using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    class FrameTimer
    {
        double accumulator = 0.0;
        double fixedDt;

        public FrameTimer(double fixedDt)
        {
            this.fixedDt = fixedDt;
        }

        public FrameAdvance Advance(double realDt)
        {
            accumulator += realDt;

            int steps = 0;

            // Count how many fixed ticks we need
            while (accumulator >= fixedDt)
            {
                accumulator -= fixedDt;
                steps++;
            }

            double alpha = accumulator / fixedDt;

            return new FrameAdvance
            {
                PhysicsSteps = steps,
                Alpha = alpha,
                ProcessDt = realDt
            };
        }
    }

    struct FrameAdvance
    {
        public int PhysicsSteps;
        public double Alpha;
        public double ProcessDt;
    }
}
