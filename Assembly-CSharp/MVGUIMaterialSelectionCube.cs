using System;
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

	private Material originalMaterial;

	private bool isGreyedOut;

	private MVMaterial mvMaterial;

	private Vector3 mouseDownPos;

	private GameObject ChildCube => transform.FindChild("Cube").gameObject;

	public int MaterialId
	{
		get
		{
			return _materialId;
		}
		set
		{
			_materialId = value;
			mvMaterial = MVGameControllerBase.Game.MaterialRepository.GetMaterial((byte)_materialId);
			ChildCube.GetComponent<MeshFilter>().sharedMesh = mvMaterial.mesh;
			ChildCube.GetComponent<Renderer>().material = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
			originalMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
			AddToolTip(mvMaterial.name);
		}
	}

	public static Material GreyedOutMaterial { get; set; }

	public bool IsGreyedOut
	{
		get
		{
			return isGreyedOut;
		}
		set
		{
			isGreyedOut = value;
			if (isGreyedOut)
			{
				ChildCube.GetComponent<Renderer>().material = GreyedOutMaterial;
			}
			else
			{
				ChildCube.GetComponent<Renderer>().material = originalMaterial;
			}
		}
	}

	public void Start()
	{
		UXMouseClickObject component = GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			mouseDownPos = mousePositionWorld;
			return false;
		}));
		component.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(component.OnMouseUp, (UXMouseClickObject.OnMouseUpDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnSelection != null && (mousePositionWorld - mouseDownPos).magnitude < 2f)
			{
				OnSelection(this);
			}
			mouseDownPos = Vector3.zero;
		}));
		UXMouseOverObject component2 = GetComponent<UXMouseOverObject>();
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
		bool isAvailable = mvMaterial.IsAvailable;
		if (!isAvailable)
		{
			IsGreyedOut = true;
		}
		else
		{
			IsGreyedOut = false;
		}
		ChildCube.GetComponent<Renderer>().enabled = visible;
		if (ChildCube.GetComponent<Collider>() != null)
		{
			ChildCube.GetComponent<Collider>().enabled = visible && isAvailable;
		}
		GetComponent<Collider>().enabled = visible && isAvailable;
	}

	public void StartKeyAnimation()
	{
		keyObject.SetVisible(visible: true);
		gameObject.GetComponent<Animation>().Play();
	}

	public void StopKeyAnimation()
	{
		if (gameObject.GetComponent<Animation>().IsPlaying("UnlockAnimation"))
		{
			keyObject.SetVisible(visible: false);
			lockObject.SetVisible(visible: false);
			gameObject.GetComponent<Animation>().Rewind();
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
		uXToolTip.toolTipText = tooltipText;
	}

	public GameObject GetPreviewCube()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(ChildCube);
		gameObject.transform.localPosition = new Vector3(9f, 1.5f, -4f);
		gameObject.transform.localScale = new Vector3(7f, 7f, 7f);
		gameObject.AddComponent<MVGUIMaterialPurchasePreviewCube>();
		return gameObject;
	}
}
