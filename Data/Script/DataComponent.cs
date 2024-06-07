using System;
using System.Collections.Generic;

namespace BaeknothingLib.Data;

public abstract class DataComponentBase
{
    public DateTime LastUpdate { get; private set; }
    public bool IsSaved { get; private set; }

    public void FromMap(Dictionary<object, object> map)
    {
        if (!CheckMapIsValid(map))
            return;

        FromMapInternal(map);
        LastUpdate = DateTime.Now;
        IsSaved = false;
    }

    static bool CheckMapIsValid(Dictionary<object, object> map)
    {
        if (map == null)
            return false;

        foreach (var key in map.Keys)
        {
            if (key is not string)
            {
                return false;
            }
        }
        return true;
    }

    public abstract string GetKey();
    public abstract Dictionary<string, object> ToMap();
    abstract protected void FromMapInternal(Dictionary<object, object> map);
    public abstract string Print();
}
