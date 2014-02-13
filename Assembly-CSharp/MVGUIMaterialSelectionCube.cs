using System;
using Localize;
using UnityEngine;

public class MVGUIMaterialSelectionCube : UXGUIElement
{
	public delegate void OnMaterialDelegate(MVGUIMaterialSelectionCube materialCube);

	public OnMaterialDelegate OnSelection;

	public OnMaterialDelegate OnMouseOver;

	public UXPlane lockObject;

	public UXPlane keyObject;

	public ParticleSystem unlockEffect;

	private int _materialId = -1;

	private MVMaterial mvMaterial;

	private Vector3 mouseDownPos;

	private GameObject ChildCube => ((Component)((Component)this).transform.FindChild("Cube")).gameObject;

	public int MaterialId
	{
		get
		{
			return _materialId;
		}
		set
		{
			_materialId = value;
			mvMaterial = MVGameController.Instance.Game.MaterialRepository.GetMaterial((byte)_materialId);
			ChildCube.renderer.material = mvMaterial.material;
			AddToolTip(mvMaterial.name);
		}
	}

	public void Start()
	{
		UXMouseClickObject component = ((Component)this).GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mouseDownPos = mousePositionWorld;
			return false;
		}));
		component.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(component.OnMouseUp, (UXMouseClickObject.OnMouseUpDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (OnSelection != null)
			{
				Vector3 val = mousePositionWorld - mouseDownPos;
				if (val.magnitude < 2f)
				{
					OnSelection(this);
				}
			}
			mouseDownPos = Vector3.zero;
		}));
		UXMouseOverObject component2 = ((Component)this).GetComponent<UXMouseOverObject>();
		component2.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component2.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverDelegate)((UXMouseOverObject o) =>
		{
			if (OnMouseOver != null)
			{
				OnMouseOver(this);
			}
		}));
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		lockObject.SetVisible(visible && !mvMaterial.isUnlocked);
		keyObject.SetVisible(visible: false);
		ChildCube.renderer.enabled = visible;
		if ((Object)(object)ChildCube.collider != (Object)null)
		{
			ChildCube.collider.enabled = visible;
		}
		((Component)this).collider.enabled = visible;
	}

	public void StartKeyAnimation()
	{
		keyObject.SetVisible(visible: true);
		((Component)this).gameObject.animation.Play();
	}

	public void StopKeyAnimation()
	{
		if (((Component)this).gameObject.animation.IsPlaying("UnlockAnimation"))
		{
			keyObject.SetVisible(visible: false);
			lockObject.SetVisible(visible: false);
			((Component)this).gameObject.animation.Rewind();
		}
	}

	private void OnKeyUnlock()
	{
		if (Visible)
		{
			keyObject.SetVisible(visible: false);
			unlockEffect.Play();
			lockObject.SetVisible(visible: false);
		}
	}

	private void AddToolTip(string tooltipText)
	{
		ChildCube.AddComponent<BoxCollider>();
		ChildCube.AddComponent<UXMouseOverObject>();
		UXToolTip uXToolTip = ChildCube.AddComponent<UXToolTip>();
		uXToolTip.toolTipTextID = TextSlotIndex.Empty;
		uXToolTip.toolTipText = tooltipText;
	}

	public GameObject GetPreviewCube()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = (GameObject)Object.Instantiate((Object)(object)ChildCube);
		val.transform.localPosition = new Vector3(9f, 1.5f, -4f);
		val.transform.localScale = new Vector3(7f, 7f, 7f);
		val.AddComponent<MVGUIMaterialPurchasePreviewCube>();
		return val;
	}
}
