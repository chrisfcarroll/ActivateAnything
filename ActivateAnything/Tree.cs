using System.Collections.Generic;

namespace ActivateAnything
{
    public class Tree<T> : Dictionary<T, Tree<T>>
    {
    }
}
