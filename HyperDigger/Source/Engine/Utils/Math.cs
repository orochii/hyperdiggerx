using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    public static class HyperMath
    {
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (current < target)
            {
                float value = current + maxDelta;
                if (value > target) return target;
                return value;
            }
            else if (current > target)
            {
                float value = current - maxDelta;
                if (value < target) return target;
                return value;
            }
            return current;
        }
    }
}
