using System;
using UnityEngine;

public class MVGUISelectionGizmo : MonoBehaviour
{
	public delegate void GizmoClickDelegate();

	public UXMouseClickObject clone;

	public UXMouseClickObject delete;

	public UXMouseClickObject rotate;

	public UXMouseClickObject xzTranslate;

	public UXMouseClickObject yTranslate;

	public UXMouseClickObject open;

	public UXMouseClickObject editButton;

	public UXMouseClickObject logicReset;

	public UXMouseClickObject addToInventory;

	public GizmoClickDelegate OnClone;

	public GizmoClickDelegate OnDelete;

	public GizmoClickDelegate OnRotate;

	public GizmoClickDelegate OnXZtranslate;

	public GizmoClickDelegate OnYtranslate;

	public GizmoClickDelegate OnEdit;

	public GizmoClickDelegate OnOpen;

	public GizmoClickDelegate OnLogicReset;

	public GizmoClickDelegate OnAddToInventory;

	private bool visible;

	private bool showEditButton;

	private bool showSettingsButton;

	private bool showCloneButton;

	private bool showLogicResetButton;

	private bool showAddToInventoryButton;

	private Vector3 worldPosition = Vector3.zero;

	private UXCamera uxCamera;

	private Camera mainCamera;

	public Vector3 WorldPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return worldPosition;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			worldPosition = value;
		}
	}

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			UpdateVisiblity();
		}
	}

	public bool ShowEditButton
	{
		get
		{
			return showEditButton;
		}
		set
		{
			showEditButton = value;
			UpdateVisiblity();
		}
	}

	public bool ShowSettingButton
	{
		get
		{
			return showSettingsButton;
		}
		set
		{
			showSettingsButton = value;
			UpdateVisiblity();
		}
	}

	public bool ShowCloneButton
	{
		get
		{
			return showCloneButton;
		}
		set
		{
			showCloneButton = value;
			UpdateVisiblity();
		}
	}

	public bool ShowAddToInventoryButton
	{
		get
		{
			return showAddToInventoryButton;
		}
		set
		{
			showAddToInventoryButton = value;
			UpdateVisiblity();
		}
	}

	public bool ShowLogicResetButton
	{
		get
		{
			return showLogicResetButton;
		}
		set
		{
			showLogicResetButton = value;
			UpdateVisiblity();
		}
	}

	public MVGUISelectionGizmo()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Awake()
	{
		uxCamera = Object.FindObjectOfType(typeof(UXCamera)) as UXCamera;
		mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		visible = Object.op_Implicit((Object)(object)((Component)this).GetComponent<UXVisible>());
		UXMouseClickObject uXMouseClickObject = clone;
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnClone != null)
			{
				OnClone();
			}
			return false;
		}));
		UXMouseClickObject uXMouseClickObject2 = rotate;
		uXMouseClickObject2.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject2.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnRotate != null)
			{
				OnRotate();
			}
			return false;
		}));
		UXMouseClickObject uXMouseClickObject3 = xzTranslate;
		uXMouseClickObject3.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject3.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnXZtranslate != null)
			{
				OnXZtranslate();
			}
			return false;
		}));
		UXMouseClickObject uXMouseClickObject4 = yTranslate;
		uXMouseClickObject4.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject4.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnYtranslate != null)
			{
				OnYtranslate();
			}
			return false;
		}));
		UXMouseClickObject uXMouseClickObject5 = delete;
		uXMouseClickObject5.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject5.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnDelete != null)
			{
				OnDelete();
			}
		}));
		UXMouseClickObject uXMouseClickObject6 = open;
		uXMouseClickObject6.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject6.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		UXMouseClickObject uXMouseClickObject7 = open;
		uXMouseClickObject7.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(uXMouseClickObject7.OnMouseUp, (UXMouseClickObject.OnMouseUpDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnOpen != null)
			{
				OnOpen();
			}
		}));
		UXMouseClickObject uXMouseClickObject8 = editButton;
		uXMouseClickObject8.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject8.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnEdit != null)
			{
				OnEdit();
			}
		}));
		UXMouseClickObject uXMouseClickObject9 = addToInventory;
		uXMouseClickObject9.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject9.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnAddToInventory != null)
			{
				OnAddToInventory();
			}
		}));
		UXMouseClickObject uXMouseClickObject10 = logicReset;
		uXMouseClickObject10.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject10.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			if (OnLogicReset != null)
			{
				OnLogicReset();
			}
		}));
	}

	public void Start()
	{
		Visible = false;
	}

	public void Update()
	{
		UpdatePosition();
	}

	public void OnEnable()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = mainCamera.WorldToScreenPoint(worldPosition);
		if (val.z < -1f)
		{
			val.x = -20f;
			val.y = -20f;
		}
		val.z = 10f;
		((Component)this).transform.position = ((Component)uxCamera).camera.ScreenToWorldPoint(val);
	}

	private void UpdateVisiblity()
	{
		((Component)clone).gameObject.SetActiveRecursively(visible);
		((Component)delete).gameObject.SetActiveRecursively(visible);
		((Component)rotate).gameObject.SetActiveRecursively(visible);
		((Component)xzTranslate).gameObject.SetActiveRecursively(visible);
		((Component)yTranslate).gameObject.SetActiveRecursively(visible);
		((Component)open).gameObject.SetActiveRecursively(visible && showEditButton);
		((Component)editButton).gameObject.SetActiveRecursively(visible && showSettingsButton);
		((Component)addToInventory).gameObject.SetActiveRecursively(visible && showAddToInventoryButton);
		((Component)logicReset).gameObject.SetActiveRecursively(visible && showLogicResetButton);
	}
}
