//// Represents a generic object with string keys and any values
//public interface IAnyObject : IDictionary<string, object>
//{
//    public bool TryGetInt(string key, [MaybeNullWhen(false)] out int @int);
//    public bool TryGetBool(string key, [MaybeNullWhen(false)] out bool @bool);
//    public bool TryGetString(string key, [MaybeNullWhen(false)] out string @string);
//    public bool TryGetId(string key, [MaybeNullWhen(false)] out Id id);
//    public bool TryGetEnum<TEnum>(string key, [MaybeNullWhen(false)] out TEnum enumValue) where TEnum : Enum;
//    public bool TryGetStruct<TStruct>(string key, [MaybeNullWhen(false)] out TStruct structValue) where TStruct : struct;
//    public bool TryGetClass<TClass>(string key, [MaybeNullWhen(false)] out TClass? classValue) where TClass : class;
//    public bool TryGetObject(string key, [MaybeNullWhen(false)] out object? obj);
//    public bool TryGetList<T>(string key, [MaybeNullWhen(false)] out List<T> list) where T : class;
//    public bool TryGetDictionary<TKey, TValue>(string key, [MaybeNullWhen(false)] out Dictionary<TKey, TValue> dict) where TKey : notnull;
//    public bool TryGetNullable<T>(string key, out T? value) where T : struct;
//    public bool TryGetFunction<T>(string key, [MaybeNullWhen(false)] out Func<T> function);
//    public bool TryGetAction<T>(string key, [MaybeNullWhen(false)] out Action<T> action);
//}


//public class DefaultTextData : IAnyObject
//{
//    private readonly Dictionary<string, object> _data = new();

//    public object this[string key]
//    {
//        get => _data[key];
//        set => _data[key] = value;
//    }

//    public ICollection<string> Keys => _data.Keys;
//    public ICollection<object> Values => _data.Values;
//    public int Count => _data.Count;
//    public bool IsReadOnly => false;

//    public void Add(string key, object value) => _data.Add(key, value);
//    public void Add(KeyValuePair<string, object> item) => _data.Add(item.Key, item.Value);
//    public void Clear() => _data.Clear();
//    public bool Contains(KeyValuePair<string, object> item) => _data.Contains(item);
//    public bool ContainsKey(string key) => _data.ContainsKey(key);
//    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((IDictionary<string, object>)_data).CopyTo(array, arrayIndex);
//    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _data.GetEnumerator();
//    public bool Remove(string key) => _data.Remove(key);
//    public bool Remove(KeyValuePair<string, object> item) => _data.Remove(item.Key);

//    public bool TryGetBool(string key, [MaybeNullWhen(false)] out bool @bool)
//    {
//        if (_data.TryGetValue(key, out var value) && value is bool b)
//        {
//            @bool = b;
//            return true;
//        }
//        @bool = default;
//        return false;
//    }

//    public bool TryGetEffectType(string key, [MaybeNullWhen(false)] out EffectType effectType)
//    {
//        if (_data.TryGetValue(key, out var value) && value is EffectType et)
//        {
//            effectType = et;
//            return true;
//        }
//        effectType = default;
//        return false;
//    }

//    public bool TryGetId(string key, [MaybeNullWhen(false)] out Id id)
//    {
//        if (_data.TryGetValue(key, out var value) && value is Id i)
//        {
//            id = i;
//            return true;
//        }
//        id = default;
//        return false;
//    }

//    public bool TryGetInt(string key, [MaybeNullWhen(false)] out int @int)
//    {
//        if (_data.TryGetValue(key, out var value) && value is int i)
//        {
//            @int = i;
//            return true;
//        }
//        @int = default;
//        return false;
//    }

//    public bool TryGetString(string key, [MaybeNullWhen(false)] out string @string)
//    {
//        if (_data.TryGetValue(key, out var value) && value is string s)
//        {
//            @string = s;
//            return true;
//        }
//        @string = default;
//        return false;
//    }

//    public bool TryGetEnum<TEnum>(string key, [MaybeNullWhen(false)] out TEnum @enum) where TEnum : Enum
//    {
//        if (_data.TryGetValue(key, out var value) && value is TEnum e)
//        {
//            @enum = e;
//            return true;
//        }
//        @enum = default;
//        return false;
//    }

//    // for IDictionary
//    public bool TryGetValue(string key, out object value) => _data.TryGetValue(key, out value);
//    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _data.GetEnumerator();

//    public bool TryGetStruct<TStruct>(string key, [MaybeNullWhen(false)] out TStruct structValue) where TStruct : struct
//    {
//        if (_data.TryGetValue(key, out var value) && value is TStruct s)
//        {
//            structValue = s;
//            return true;
//        }
//        structValue = default;
//        return false;
//    }

//    public bool TryGetClass<TClass>(string key, [MaybeNullWhen(false)] out TClass? classValue) where TClass : class
//    {
//        if (_data.TryGetValue(key, out var value) && value is TClass c)
//        {
//            classValue = c;
//            return true;
//        }
//        classValue = default;
//        return false;
//    }

//    public bool TryGetObject(string key, [MaybeNullWhen(false)] out object? obj)
//    {
//        if (_data.TryGetValue(key, out var value))
//        {
//            obj = value;
//            return true;
//        }
//        obj = default;
//        return false;
//    }

//    public bool TryGetList<T>(string key, [MaybeNullWhen(false)] out List<T> list) where T : class
//    {
//        if (_data.TryGetValue(key, out var value) && value is List<T> l)
//        {
//            list = l;
//            return true;
//        }
//        list = default;
//        return false;
//    }

//    public bool TryGetDictionary<TKey, TValue>(string key, [MaybeNullWhen(false)] out Dictionary<TKey, TValue> dict) where TKey : notnull
//    {
//        if (_data.TryGetValue(key, out var value) && value is Dictionary<TKey, TValue> d)
//        {
//            dict = d;
//            return true;
//        }
//        dict = default;
//        return false;
//    }

//    public bool TryGetNullable<T>(string key, [MaybeNullWhen(false)] out T? value) where T : struct
//    {
//        if (_data.TryGetValue(key, out var obj) && obj is T v)
//        {
//            value = v;
//            return true;
//        }
//        value = default;
//        return false;
//    }

//    public bool TryGetFunction<T>(string key, [MaybeNullWhen(false)] out Func<T> function)
//    {
//        if (_data.TryGetValue(key, out var value) && value is Func<T> func)
//        {
//            function = func;
//            return true;
//        }
//        function = default;
//        return false;
//    }

//    public bool TryGetAction<T>(string key, [MaybeNullWhen(false)] out Action<T> action)
//    {
//        if (_data.TryGetValue(key, out var value) && value is Action<T> act)
//        {
//            action = act;
//            return true;
//        }
//        action = default;
//        return false;
//    }
//}

//public class AnyObjectEmpty : DefaultTextData, IAnyObject
//{
//}


//public interface IPokemonSet { }
//public interface IPRNGSeed { }
//public interface ITypeInfo { }




//public class ModdedEffectData : IEffectData
//{
//    public bool Inherit { get; set; } = true;
//    public string? Name { get; set; } = string.Empty;
//    public string? Desc { get; set; } = string.Empty;
//    public int? Duration { get; set; } = null;
//    public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; } = null;
//    public string? EffectTypeString { get; set; } = null;
//    public bool? Infiltrates { get; set; } = null;
//    public Nonstandard? IsNonstandard { get; set; } = null;
//    public string? ShortDesc { get; set; } = string.Empty;
//}