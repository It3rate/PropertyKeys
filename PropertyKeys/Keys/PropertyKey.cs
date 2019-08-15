using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyKeys
{
    public struct PropertyKey<T>
    {
        public float t;
        public int[] targetIDs;
        public Action<Values> property;
        public ValueKey ValueKey;
        public bool IsRepeating; // stop count repeats every n items
    }
}
