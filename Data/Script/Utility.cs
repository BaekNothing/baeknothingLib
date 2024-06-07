using System;
using System.Collections.Generic;

namespace BaeknothingLib.Data;

static class Utility
{
    public static Dictionary<string, object> ConvertMapKey_ObjectToString(Dictionary<object, object> map)
    {
        var newMap = new Dictionary<string, object>();
        foreach (var key in map.Keys)
        {
            if (key is string strKey)
            {
                newMap[strKey] = map[key];
            }
        }

        return newMap;
    }

    public static Dictionary<object, object> ConvertMapKey_StringToObject(Dictionary<string, object> map)
    {
        var newMap = new Dictionary<object, object>();
        foreach (var key in map.Keys)
        {
            newMap[key] = map[key];
        }

        return newMap;
    }

    public static Dictionary<object, object> ConvertObjectToDictionary(object obj)
    {
        if (obj is Dictionary<object, object> map)
        {
            return map;
        }

        return [];
    }

    public static bool CheckIsValidMap(Dictionary<object, object> map)
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

    public static T? IsValidComponent<T>(DataComponentBase component) where T : DataComponentBase
    {
        if (component is T tComponent)
        {
            return tComponent;
        }

        return null;
    }
}
