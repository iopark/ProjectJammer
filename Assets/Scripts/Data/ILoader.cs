using System.Collections.Generic;

namespace LDW
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }
}