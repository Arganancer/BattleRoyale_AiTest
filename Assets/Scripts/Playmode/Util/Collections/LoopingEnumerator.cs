using System;
using System.Collections.Generic;

namespace Playmode.Util.Collections
{
    public class LoopingEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        public LoopingEnumerator(IEnumerable<T> enumerable)
        {
            enumerator = enumerable.GetEnumerator();

            if (!enumerator.MoveNext())
                throw new ArgumentException("Can't do an infinite loop on an empty Enumerator.");
        }

        public T Next()
        {
            var current = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                enumerator.Reset();
                enumerator.MoveNext();
            }
            return current;
        }
    }
}