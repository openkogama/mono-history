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
			if ((Object)(object)instance == (Object)null)
			{
				instance = Object.FindObjectOfType(typeof(UXFullscreenColliderBox)) as UXFullscreenColliderBox;
			}
			return instance;
		}
	}

	private void Awake()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		screen = UXUtils.FindObjectOfType<UXScreen>();
		UXScreen uXScreen = screen;
		uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
		go = new GameObject("Collider");
		boxCollider = go.AddComponent<BoxCollider>();
		mouseClickObject = go.AddComponent<UXMouseClickObject>();
		mouseClickObject.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true;
		mouseClickObject.OnClick = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnClick != null)
			{
				OnClick();
			}
		};
		go.layer = LayerMask.NameToLayer("UXElement");
		go.transform.parent = ((Component)this).transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.active = false;
	}

	public void OnResize()
	{
		UpdatePlacement();
	}

	public void AddBlockingObject(object o)
	{
		blockingObjects.Add(o);
		go.active = blockingObjects.Count > 0;
	}

	public void RemoveBlockingObject(object o)
	{
		blockingObjects.Remove(o);
		go.active = blockingObjects.Count > 0;
	}

	public void UpdatePlacement()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = screen.GetPosition(UXHorizontal.Center, UXVertical.Middle, depth);
		Vector3 val = screen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - screen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		boxCollider.size = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), 10f);
	}
}
