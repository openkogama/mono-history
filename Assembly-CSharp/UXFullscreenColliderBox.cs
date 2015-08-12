using System;
using System.Collections.Generic;
using UnityEngine;

public class UXFullscreenColliderBox : MonoBehaviour
{
	public float depth;

	public Action OnClick;

	private UXScreen screen;

	private GameObject go;

	private BoxCollider boxCollider;

	private UXMouseClickObject mouseClickObject;

	private static UXFullscreenColliderBox instance;

	private HashSet<object> blockingObjects = new HashSet<object>();

	public static UXFullscreenColliderBox Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UXUtils.FindGUIObjectOfType<UXFullscreenColliderBox>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		screen = UXUtils.UXScreen;
		UXScreen uXScreen = screen;
		uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
		if (go == null)
		{
			InitializeCollider();
		}
	}

	private void InitializeCollider()
	{
		go = new GameObject("Collider");
		boxCollider = go.AddComponent<BoxCollider>();
		mouseClickObject = go.AddComponent<UXMouseClickObject>();
		mouseClickObject.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true;
		mouseClickObject.OnMouseUp = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnClick != null)
			{
				OnClick();
			}
		};
		go.layer = LayerMask.NameToLayer("UXElement");
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.SetActive(value: false);
	}

	public void OnResize()
	{
		UpdatePlacement();
	}

	public void AddBlockingObject(object o)
	{
		if (go == null)
		{
			InitializeCollider();
		}
		blockingObjects.Add(o);
		go.SetActive(blockingObjects.Count > 0);
	}

	public void RemoveBlockingObject(object o)
	{
		if (go == null)
		{
			InitializeCollider();
		}
		blockingObjects.Remove(o);
		go.SetActive(blockingObjects.Count > 0);
		if (!go.activeInHierarchy)
		{
			OnClick = null;
		}
	}

	public int number()
	{
		return blockingObjects.Count;
	}

	public void UpdatePlacement()
	{
		transform.position = screen.GetPosition(UXHorizontal.Center, UXVertical.Middle, depth);
		Vector3 vector = screen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - screen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		boxCollider.size = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), 10f);
	}
}
