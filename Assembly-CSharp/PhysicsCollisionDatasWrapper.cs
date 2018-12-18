using System.Collections.Generic;
using UnityEngine;

public class PhysicsCollisionDatasWrapper
{
	private int length;

	private readonly List<PhysicsCollisionData> physicsCollisionDatas = new List<PhysicsCollisionData>(100);

	public int Length => length;

	public PhysicsCollisionData this[int key] => physicsCollisionDatas[key];

	public PhysicsCollisionDatasWrapper()
	{
		for (int i = 0; i < physicsCollisionDatas.Capacity; i++)
		{
			physicsCollisionDatas.Add(new PhysicsCollisionData());
		}
	}

	public void Clear()
	{
		for (int i = 0; i < length; i++)
		{
			physicsCollisionDatas[i].collider = null;
			physicsCollisionDatas[i].transform = null;
		}
		length = 0;
	}

	public void Add(RaycastHit hit)
	{
		if (length >= 100)
		{
			Debug.LogWarning("PhysicsCollisionData length exceeded");
			return;
		}
		physicsCollisionDatas[length].Set(hit);
		length++;
	}

	public void Add(Collider collider, Vector3 origin)
	{
		if (length >= 100)
		{
			Debug.LogWarning("PhysicsCollisionData length exceeded");
			return;
		}
		physicsCollisionDatas[length].Set(collider, origin);
		length++;
	}
}
