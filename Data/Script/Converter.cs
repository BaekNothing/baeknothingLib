using System.Collections;
using System.Reflection;
using System.Linq;

namespace BaeknothingLib.Data;

public static class Converter
{
    [AttributeUsage(AttributeTargets.Field)] public class ConvertTarget : Attribute { };


    public static T DictionaryToClass<T>(Dictionary<string, object> dict) where T : new()
    {
        return (T)DictionaryToClass(dict, typeof(T));
    }

    public static Dictionary<string, object?> ClassToDictionary(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var result = new Dictionary<string, object?>();
        var fields = GetFields(obj.GetType());

        ArgumentNullException.ThrowIfNull(fields);

        foreach (var field in fields)
        {
            ConvertTarget? attribute = field.GetCustomAttribute<ConvertTarget>();
            if (attribute == null)
                continue;

            object? fieldValue = field.GetValue(obj);
            ArgumentNullException.ThrowIfNull(fieldValue);

            if (IsUserDefinedClass(field.FieldType))
                result[field.Name] = ClassToDictionary(fieldValue);
            else
                result[field.Name] = ConvertInvalidTypeToString(fieldValue);
        }

        return result;
    }

    static FieldInfo[]? GetFields(Type type)
    {
        return type.
            GetFields(BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.Instance);
    }

    static bool IsUserDefinedClass(Type type)
    {
        // Check if the type is a user-defined class (not a built-in type)
        return type.IsClass &&
            type.Namespace != null &&
            !type.Namespace.StartsWith("System") &&
            !type.IsGenericType;
    }


    const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    static object? ConvertInvalidTypeToString(object? value)
    {
        if (value != null && value.GetType().IsEnum)
        {
            return $"{value}";
        }
        else if (value is IDictionary dictionary)
        {
            var newDictionary = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in dictionary)
            {
                var dicValue = ConvertInvalidTypeToString(entry.Value);
                if (dicValue != null)
                    newDictionary[$"{entry.Key}"] = dicValue;
            }
            return newDictionary;
        }
        else if (value is IEnumerable enumerable)
        {
            var newEnumerable = new List<object>();
            foreach (var item in enumerable)
            {
                var enuValue = ConvertInvalidTypeToString(item);
                if (enuValue != null)
                    newEnumerable.Add(enuValue);
            }
            return newEnumerable;
        }
        else if (value is DateTime dateTime)
        {
            return dateTime.ToString(dateTimeFormat);
        }
        else
            return value;
    }

    static object DictionaryToClass(Dictionary<string, object> dict, Type type)
    {
        ArgumentNullException.ThrowIfNull(dict);
        ArgumentNullException.ThrowIfNull(type);

        object result = Activator.CreateInstance(type) ??
            throw new InvalidOperationException("Failed to create an instance of the class.");

        foreach (var kvp in dict)
        {
            FieldInfo? field = type.GetField(kvp.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (field != null)
            {
                object value = kvp.Value;

                if (field.FieldType.IsEnum)
                {
                    // If the field is an enum, convert the string back to the enum value
                    object enumValue = Enum.Parse(field.FieldType, $"{value}");
                    field.SetValue(result, enumValue);
                }
                else if (value is Dictionary<string, object> nestedDict && IsUserDefinedClass(field.FieldType))
                {
                    // If the field is a user-defined class, process recursively
                    object? nestedObj = DictionaryToClass(nestedDict, field.FieldType);
                    field.SetValue(result, nestedObj);
                }
                else if (IsGenericDictionary(field.FieldType))
                {
                    // If the field is a generic dictionary, process it
                    object dictObj = ConvertToGenericDictionary(value, field.FieldType);
                    field.SetValue(result, dictObj);
                }
                else
                {
                    field.SetValue(result, Convert.ChangeType(value, field.FieldType));
                }
            }
        }

        return result;
    }

    private static bool IsGenericDictionary(Type type)
    {
        if (!type.IsGenericType) return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(Dictionary<,>);
    }

    private static object ConvertToGenericDictionary(object value, Type fieldType)
    {
        var keyType = fieldType.GetGenericArguments()[0];
        var valueType = fieldType.GetGenericArguments()[1];
        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dictionary = Activator.CreateInstance(dictionaryType) as IDictionary ??
            throw new InvalidOperationException("Failed to create an instance of the dictionary.");

        if (value is IDictionary<string, object> genericDict)
        {
            foreach (var kvp in genericDict)
            {
                var key = keyType.IsEnum ? Enum.Parse(keyType, kvp.Key) : Convert.ChangeType(kvp.Key, keyType);
                var val = kvp.Value;

                if (valueType.IsEnum)
                {
                    val = Enum.Parse(valueType, $"{val}");
                }
                else if (val is Dictionary<string, object> nestedDict && IsUserDefinedClass(valueType))
                {
                    val = DictionaryToClass(nestedDict, valueType);
                }

                dictionary.Add(key, val);
            }
        }

        return dictionary;
    }
}
