using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVDragon : MVBlueprintBase
{
	private const string prefabPathDragonPosition = "Prefabs/Blueprints/DragonPosition";

	private const string prefabPathDragonTargetArea = "Prefabs/Blueprints/DragonTargetArea";

	private MVDragonHead dragonHead;

	private MVCubeModelInstance dragonNeck;

	private GameObject dragonTargetArea;

	public MVDragonHead DragonHead => dragonHead;

	public MVCubeModelInstance DragonNeck => dragonNeck;

	public GameObject DragonTargetArea => dragonTargetArea;

	public bool IsPlayMode => MVGameController.Instance.EditController != null && MVGameController.Instance.EditController.PlayInEditor;

	public override Vector3 WorldPivot
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return WorldPosition;
		}
	}

	public MVDragon(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/DragonPosition", worldObjects)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		dragonTargetArea = LoadPrefab("Prefabs/Blueprints/DragonTargetArea");
		dragonTargetArea.transform.Translate(2f * Vector3.left);
		interactionFlags |= InteractionFlags.CanEdit;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
		gameObject.GetComponent<Dragon>().InitializeInventory(this);
		dragonHead.initThis(this);
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeCommon();
		gameObject.GetComponent<Dragon>().Initialize(this);
		dragonHead.initThis(this);
	}

	private void InitializeCommon()
	{
		dragonHead = GetChild((int)childIdMap["DragonHead"]) as MVDragonHead;
		dragonNeck = GetChild((int)childIdMap["DragonNeck"]) as MVCubeModelInstance;
		if (dragonHead == null)
		{
			Debug.Log((object)"Missing dragon head");
		}
		if (dragonNeck == null)
		{
			Debug.Log((object)"Missing dragon Neck");
		}
		if (dragonHead != null && dragonNeck != null)
		{
			if (HasInteractionFlag(InteractionFlags.IsPreview))
			{
				AddPreviewBoxesToElements();
				dragonHead.PreviewOwnerProfileId = PreviewOwnerProfileId;
				dragonNeck.PreviewOwnerProfileId = PreviewOwnerProfileId;
				dragonHead.InteractionFlags |= InteractionFlags.IsPreview;
				dragonNeck.InteractionFlags |= InteractionFlags.IsPreview;
			}
			dragonNeck.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup | InteractionFlags.NotUserTransformable;
		}
	}

	public override void Select(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.Select(color);
		Dragon component = GameObject.GetComponent<Dragon>();
		component.UpdateNeck(fullUpdate: true);
	}

	public override void DeSelect()
	{
		base.DeSelect();
		Dragon component = GameObject.GetComponent<Dragon>();
		component.UpdateNeck(fullUpdate: true);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		if (dragonNeck == null)
		{
			return false;
		}
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(this);
		e.SelectWO(dragonNeck.Id, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		return true;
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		e.ExitGroupToRoot();
		return true;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Transform component = ((Component)GameObject.GetComponentInChildren<MeshRenderer>()).gameObject.GetComponent<Transform>();
		Bounds val = new Bounds(Vector3.zero, component.localScale);
		return boundsContext switch
		{
			BoundsContext.Insert => val, 
			_ => base.GetLocalBounds(boundsContext), 
		};
	}

	public override void SetWorldObjectToPurchased()
	{
		base.SetWorldObjectToPurchased();
		dragonHead.SetWorldObjectToPurchased();
		dragonNeck.SetWorldObjectToPurchased();
	}

	public override void AddPreviewBox()
	{
	}

	private void AddPreviewBoxesToElements()
	{
		dragonHead.AddPreviewBox();
		dragonNeck.AddPreviewBox();
	}
}
