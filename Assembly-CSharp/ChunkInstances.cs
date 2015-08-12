using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ChunkInstances : IEnumerator, IEnumerable
{
	private Dictionary<IntVector, GameObject> chunkInstances = new Dictionary<IntVector, GameObject>();

	public int Count => chunkInstances.Count;

	public object Current => chunkInstances.GetEnumerator().Current;

	public event EventHandler<ChunkInstancesChanged> Changed;

	IEnumerator IEnumerable.GetEnumerator()
	{
		return chunkInstances.GetEnumerator();
	}

	public void Add(IntVector intVector, GameObject gameObject)
	{
		chunkInstances.Add(intVector, gameObject);
		if (Changed != null)
		{
			Changed(this, new ChunkInstancesChanged(ChunkInstancesChanged.ChangeType.Added, intVector));
		}
	}

	public void Remove(IntVector intVector)
	{
		chunkInstances.Remove(intVector);
		if (Changed != null)
		{
			Changed(this, new ChunkInstancesChanged(ChunkInstancesChanged.ChangeType.Removed, intVector));
		}
	}

	public bool Contains(IntVector intVector)
	{
		return chunkInstances.ContainsKey(intVector);
	}

	public bool TryGetValue(IntVector intVector, out GameObject gameObject)
	{
		return chunkInstances.TryGetValue(intVector, out gameObject);
	}

	public GameObject GetChunk(IntVector intVector)
	{
		return chunkInstances[intVector];
	}

	public void Clear()
	{
		chunkInstances.Clear();
		if (Changed != null)
		{
			Changed(this, new ChunkInstancesChanged(ChunkInstancesChanged.ChangeType.Clear, IntVector.One));
		}
	}

	public bool MoveNext()
	{
		return chunkInstances.GetEnumerator().MoveNext();
	}

	public void Reset()
	{
		((IEnumerator)chunkInstances.GetEnumerator()).Reset();
		chunkInstances.GetEnumerator().MoveNext();
	}
}
