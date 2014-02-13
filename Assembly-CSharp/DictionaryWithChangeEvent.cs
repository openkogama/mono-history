using System.Collections;
using System.Collections.Generic;

public class DictionaryWithChangeEvent<TKey, TValue> : IEnumerable, ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
{
	public delegate void OnDictionaryChangeDelegate(IDictionary<TKey, TValue> dictionary);

	private IDictionary<TKey, TValue> dictionary;

	public OnDictionaryChangeDelegate OnDictionaryChange;

	public bool IsReadOnly => dictionary.IsReadOnly;

	public int Count => dictionary.Count;

	public ICollection<TKey> Keys => dictionary.Keys;

	public ICollection<TValue> Values => dictionary.Values;

	public TValue this[TKey key]
	{
		get
		{
			return dictionary[key];
		}
		set
		{
			dictionary[key] = value;
			NotifyDictionaryChange();
		}
	}

	public DictionaryWithChangeEvent()
	{
		dictionary = new Dictionary<TKey, TValue>();
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return dictionary.GetEnumerator();
	}

	public void Add(KeyValuePair<TKey, TValue> pair)
	{
		Add(pair.Key, pair.Value);
	}

	public void Add(TKey key, TValue value)
	{
		dictionary.Add(key, value);
		NotifyDictionaryChange();
	}

	public bool Remove(KeyValuePair<TKey, TValue> pair)
	{
		return Remove(pair.Key);
	}

	public bool Remove(TKey key)
	{
		bool flag = dictionary.Remove(key);
		if (flag)
		{
			NotifyDictionaryChange();
		}
		return flag;
	}

	public void Clear()
	{
		dictionary.Clear();
		NotifyDictionaryChange();
	}

	public bool Contains(KeyValuePair<TKey, TValue> pair)
	{
		return dictionary.Contains(pair);
	}

	public bool ContainsKey(TKey key)
	{
		return dictionary.ContainsKey(key);
	}

	public IEnumerator GetEnumerator()
	{
		return dictionary.GetEnumerator();
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		dictionary.CopyTo(array, arrayIndex);
	}

	private void NotifyDictionaryChange()
	{
		if (OnDictionaryChange != null)
		{
			OnDictionaryChange(this);
		}
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return dictionary.TryGetValue(key, out value);
	}
}
