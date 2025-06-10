using System;
using System.Collections.Generic;

namespace Sly
{
    [Serializable]
    public class Wrapper<T>
    {
        public List<T> items;

    }
}