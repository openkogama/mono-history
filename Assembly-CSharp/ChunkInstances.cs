using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ChunkInstances : IEnumerator, IEnumerable
{
	public struct ChunkInstanceVariables
	{
		public GameObject gameObject;

		public BoxCollider collider;

		public MeshRenderer renderer;

		public MeshFilter filter;
	}

	private Dictionary<IntVector, ChunkInstanceVariables> chunkInstances = new Dictionary<IntVector, ChunkInstanceVariables>();

	public int Count => chunkInstances.Count;

	public object Current => chunkInstances.GetEnumerator().Current;

	public event EventHandler<ChunkInstancesChanged> Changed;

	IEnumerator IEnumerable.GetEnumerator()
	{
		return chunkInstances.GetEnumerator();
	}

	public void Add(IntVector intVector, ChunkInstanceVariables gameObject)
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

	public bool TryGetValue(IntVector intVector, out ChunkInstanceVariables gameObject)
	{
		return chunkInstances.TryGetValue(intVector, out gameObject);
	}

	public ChunkInstanceVariables GetChunk(IntVector intVector)
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
