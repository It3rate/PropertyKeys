using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyKeys
{
    public struct PropertyKey<T>
    {
        public float t;
        public int[] targetIDs;
        Action<T> property;
        ValueKey<T> ValueKey;
    }
}
