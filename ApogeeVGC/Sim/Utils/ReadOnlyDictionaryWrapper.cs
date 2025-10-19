using System.Collections;
using System.Reflection;

namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// A wrapper around IReadOnlyDictionary that returns copies of objects when accessed.
/// This ensures that mutable fields in the objects cannot be accidentally shared between different users of the Library.
/// </summary>
/// <typeparam name="TKey">The type of the dictionary keys</typeparam>
/// <typeparam name="TValue">The type of the dictionary values, must have a Copy() method</typeparam>
internal class ReadOnlyDictionaryWrapper<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> innerDictionary)
    : IReadOnlyDictionary<TKey, TValue>
    where TValue : class
{
    public TValue this[TKey key]
    {
        get
        {
            TValue value = innerDictionary[key];
            return CopyValue(value);
        }
    }

    public IEnumerable<TKey> Keys => innerDictionary.Keys;

    public IEnumerable<TValue> Values => innerDictionary.Values.Select(CopyValue);

    public int Count => innerDictionary.Count;

    public bool ContainsKey(TKey key) => innerDictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return innerDictionary.Select(kvp =>
            new KeyValuePair<TKey, TValue>(kvp.Key, CopyValue(kvp.Value))).GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (innerDictionary.TryGetValue(key, out TValue? originalValue))
        {
            value = CopyValue(originalValue);
            return true;
        }
        value = null!;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static TValue CopyValue(TValue value)
    {
        // Use reflection to call the Copy method dynamically
        MethodInfo? copyMethod = typeof(TValue).GetMethod("Copy");
        if (copyMethod != null)
        {
            return (TValue)copyMethod.Invoke(value, null)!;
        }

        // If no Copy method exists, throw an exception with helpful information
        throw new InvalidOperationException($"Id {typeof(TValue).Name} does not have a Copy() method. " +
                                          "All types used in Library must implement a Copy() method for" +
                                          "proper isolation.");
    }
}