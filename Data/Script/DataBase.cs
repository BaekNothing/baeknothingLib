using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace BaeknothingLib.Data;

public interface IDataObject
{
    public bool IsInitialized { get; }
    public T? GetComponent<T>() where T : DataComponentBase;

    public Dictionary<string, object> ToMap();
    public void FromMap(Dictionary<object, object> map);
    public string Print(bool printAll);
}

public class DataObject : IDataObject
{
    public bool IsInitialized { get; private set; } = false;
    private readonly Dictionary<string, DataComponentBase> _components = [];

    public DataObject(List<DataComponentBase> components)
    {
        Debug.Assert(components != null, "Components is null");

        components.ForEach(component => _components[component.GetKey()] = component);
        IsInitialized = true;
    }

    public T? GetComponent<T>() where T : DataComponentBase
    {
        Debug.Assert(IsInitialized, "Character is not initialized");

        var key = typeof(T).Name;
        if (_components.TryGetValue(key, out var component))
            return component as T;

        return null;
    }

    public Dictionary<string, object> ToMap()
    {
        Debug.Assert(IsInitialized, "Character is not initialized");

        var map = new Dictionary<string, object>();
        foreach (var component in _components)
        {
            map[component.Key] = component.Value.ToMap();
        }

        return map;
    }

    public void FromMap(Dictionary<object, object> map)
    {
        Debug.Assert(IsInitialized, "DataObject is not initialized");
        Debug.Assert(Utility.CheckIsValidMap(map), "Map is not valid");

        var stringMap = Utility.ConvertMapKey_ObjectToString(map);

        foreach (var key in stringMap.Keys)
        {
            if (_components.TryGetValue(key, out var component) &&
                stringMap[key] is Dictionary<object, object> mapValue)
            {
                component.FromMap(mapValue);
            }
            else
            {
                Debug.Assert(false, $"Component {key} not found or map value is not valid");
            }
        }
    }


    public string Print(bool printAll)
    {
        Debug.Assert(IsInitialized, "Character is not initialized");

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("Character:\n");
        stringBuilder.Append("Keys : ");
        stringBuilder.Append(string.Join(", ", _components.Keys));
        stringBuilder.Append('\n');

        if (printAll)
        {
            stringBuilder.Append("All Data : ");
            foreach (var component in _components)
            {
                stringBuilder.Append(component.Key);
                stringBuilder.Append(" : ");
                stringBuilder.Append(component.Value.Print());
                stringBuilder.Append('\n');
            }
        }

        return stringBuilder.ToString();
    }
}
