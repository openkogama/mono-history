using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MovableVisualization : MonoBehaviour, IUpdatecontrollerSubscriber
{
	private class Package
	{
		public readonly Vector3 position;

		public readonly Quaternion rotation;

		public readonly float time;

		public Package(Vector3 position, Quaternion rotation)
		{
			time = Time.fixedTime;
			this.position = position;
			this.rotation = rotation;
		}
	}

	private MVCubeModelBase cmb;

	private GameObject cmbClone;

	private Package next;

	private Package current;

	private Queue<Package> packages = new Queue<Package>();

	private bool isVisible;

	private bool canBeVisible;

	private bool isDirty;

	private static float updateDirtyInterval = 1f;

	private float prevUpdateDirtyTime = Time.realtimeSinceStartup - updateDirtyInterval;

	public bool Visible
	{
		get
		{
			return isVisible && canBeVisible;
		}
		set
		{
			canBeVisible = value;
			SetMeshRenderers(value, cmbClone);
		}
	}

	public void Init(MVCubeModelBase cmb)
	{
		this.cmb = cmb;
		MVCubeModelBase mVCubeModelBase = this.cmb;
		mVCubeModelBase.Changed = (Action<CubeModelChangedEventArgs>)Delegate.Combine(mVCubeModelBase.Changed, new Action<CubeModelChangedEventArgs>(cmb_Changed));
		cmbClone = CreateMeshClone(cmb);
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.POST_UPDATEBUCKET_20);
		transform.position = cmb.WorldPosition;
		transform.rotation = cmb.WorldRotation;
	}

	private void cmb_Changed(CubeModelChangedEventArgs e)
	{
		isDirty = true;
	}

	private static GameObject CreateMeshClone(MVCubeModelBase cmb)
	{
		GameObject gameObject = new GameObject(cmb.GameObject.name + " clone");
		gameObject.transform.parent = cmb.Transform.parent;
		gameObject.transform.localPosition = cmb.Transform.localPosition;
		gameObject.transform.localRotation = cmb.Transform.localRotation;
		gameObject.transform.localScale = cmb.Transform.localScale;
		foreach (KeyValuePair<IntVector, ChunkInstances.ChunkInstanceVariables> item in (IEnumerable)cmb.ChunkInstances)
		{
			GameObject gameObject2 = item.Value.gameObject;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2);
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = gameObject2.transform.localPosition;
			gameObject3.transform.localRotation = gameObject2.transform.localRotation;
			gameObject3.transform.localScale = gameObject2.transform.localScale;
		}
		RemoveAllComponentsInChildrenExclude(new Type[2]
		{
			typeof(MeshFilter),
			typeof(MeshRenderer)
		}, gameObject);
		return gameObject;
	}

	private static void RemoveAllComponentsInChildrenExclude(Type[] exclude, GameObject gameObject)
	{
		Component[] componentsInChildren = gameObject.GetComponentsInChildren<Component>();
		Component[] array = componentsInChildren;
		foreach (Component component in array)
		{
			if (component.GetType() == typeof(Transform))
			{
				continue;
			}
			bool flag = true;
			foreach (Type type in exclude)
			{
				if (type == component.GetType())
				{
					flag = false;
				}
			}
			if (flag)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}

	public void ChangeLOD(bool newVisible)
	{
		if (!newVisible && isVisible)
		{
			SetMeshRenderers(newVisible, cmbClone);
		}
		if (newVisible && !isVisible && canBeVisible)
		{
			SetMeshRenderers(enable: true, cmbClone);
		}
	}

	private void SetMeshRenderers(bool enable, GameObject gameObject)
	{
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = enable;
		}
		isVisible = enable;
	}

	private void Update()
	{
		HandleDirty();
		float num = Time.time - Time.fixedDeltaTime;
		if (current == null && packages.Count > 0)
		{
			current = packages.Dequeue();
		}
		if (current != null && next == null && packages.Count > 0)
		{
			next = packages.Dequeue();
		}
		if (current != null && next != null)
		{
			while (next.time <= num && packages.Count > 0)
			{
				current = next;
				next = packages.Dequeue();
			}
			float num2 = 0f;
			num2 = (num - current.time) / Time.fixedDeltaTime;
			cmbClone.transform.position = Vector3.Lerp(current.position, next.position, num2);
			cmbClone.transform.rotation = Quaternion.Slerp(current.rotation, next.rotation, num2);
		}
	}

	private void HandleDirty()
	{
		if (isDirty && Time.realtimeSinceStartup - prevUpdateDirtyTime > updateDirtyInterval)
		{
			UnityEngine.Object.Destroy(cmbClone);
			cmbClone = CreateMeshClone(cmb);
			prevUpdateDirtyTime = Time.realtimeSinceStartup;
			if (isVisible && canBeVisible)
			{
				SetMeshRenderers(enable: true, cmbClone.gameObject);
			}
			isDirty = false;
		}
	}

	public void Reset()
	{
		packages.Clear();
		current = null;
		next = null;
	}

	private void OnDestroy()
	{
		UpdateController.RemoveFixedUpdateObject(this);
	}

	public void UpdateControllerUpdate()
	{
	}

	public void UpdateControllerFixedUpdate()
	{
		packages.Enqueue(new Package(cmb.Transform.position, cmb.Transform.rotation));
	}
}
