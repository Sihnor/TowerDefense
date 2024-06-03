using System;
using UnityEngine;

namespace Code.Scripts.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class RangeStep : PropertyAttribute
    {
        public readonly int min;
        public readonly int max;
        public readonly int step;
        
        public RangeStep(int min, int max, int step)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }
    }
}