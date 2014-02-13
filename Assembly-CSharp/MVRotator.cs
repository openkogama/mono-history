using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVRotator : MVMovable
{
	private Vector3 initAngularVelocity;

	private static HashSet<MVRotator> selectedRotators = new HashSet<MVRotator>();

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public Vector3 InitAngularVelocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return initAngularVelocity;
		}
	}

	public bool Horizontal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			return AngularVelocity.x == 0f && AngularVelocity.y != 0f && AngularVelocity.z == 0f;
		}
	}

	public bool Vertical
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			return AngularVelocity.x != 0f && AngularVelocity.y == 0f && AngularVelocity.z == 0f;
		}
	}

	public override Vector3 WorldPivot
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return WorldPosition;
		}
	}

	public MVRotator(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void Initialize()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		interactionFlags |= InteractionFlags.CanRotateY | InteractionFlags.CanEdit | InteractionFlags.HasSettings;
		List<MVWorldObjectClient> blueprintWorldObjectsByType = WOCM.GetBlueprintWorldObjectsByType(typeof(MVRotator));
		foreach (MVWorldObjectClient item in blueprintWorldObjectsByType)
		{
			if (item != this)
			{
				item.SelectedChanged += WorldObjectClient_SelectedChangedHandler;
			}
		}
		WOCM.SubscribeWOCreatedEvent(typeof(MVRotator), WOCM_WorldObjectCreatedHandler);
		initAngularVelocity = AngularVelocity;
		IntVector min = new IntVector(-15, -15, -15);
		IntVector max = new IntVector(15, 15, 15);
		if (CubeModel == null)
		{
			Debug.LogWarning((object)("Rotator " + id + " init - movable's cube model is NULL! If this is a new platform group restart the session. Otherwise it is broken."));
			return;
		}
		CubeModel.ModelingConstraintBuilder = () => new ModelingBoxCountConstraint(CubeModel, min, max, 1);
		CubeModel.InteractionFlags &= ~InteractionFlags.DirectlySelectable;
		CubeModel.BeingEditedChanged += MVCubeModelBase_BeingEditedChanged;
		if (HasInteractionFlag(InteractionFlags.IsPreview))
		{
			CubeModel.AddPreviewBox();
			CubeModel.PreviewOwnerProfileId = PreviewOwnerProfileId;
			CubeModel.InteractionFlags |= InteractionFlags.IsPreview;
		}
	}

	protected override void OnSelectedChanged(bool selected)
	{
		if (selected)
		{
			selectedRotators.Add(this);
		}
		else
		{
			selectedRotators.Remove(this);
		}
		bool flag = selectedRotators.Count == 0 || (selectedRotators.Count == 1 && selectedRotators.Contains(this));
		if (!CubeModel.BeingEdited && flag)
		{
			PausedMovement = selected || selectedRotators.Contains(this);
		}
		base.OnSelectedChanged(selected);
	}

	private void MVCubeModelBase_BeingEditedChanged(object sender, EditStateEventArgs e)
	{
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)sender;
		PausedMovement = mVCubeModelBase.BeingEdited;
	}

	private void WorldObjectClient_SelectedChangedHandler(object sender, SelectedEventArgs e)
	{
		if (!Selected && !CubeModel.BeingEdited)
		{
			PausedMovement = e.Selected || 0 < selectedRotators.Count;
		}
	}

	private void WOCM_WorldObjectCreatedHandler(object sender, WorldObjectCreatedEventArgs e)
	{
		e.WorldObject.SelectedChanged += WorldObjectClient_SelectedChangedHandler;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Bounds localBounds = base.GetLocalBounds(boundsContext);
		if (boundsContext == BoundsContext.Preview)
		{
			Bounds val = new Bounds(-localBounds.center, localBounds.size);
			localBounds.Encapsulate(val);
			return localBounds;
		}
		return localBounds;
	}

	public override void Destroy()
	{
		selectedRotators.Remove(this);
		List<MVWorldObjectClient> blueprintWorldObjectsByType = WOCM.GetBlueprintWorldObjectsByType(typeof(MVRotator));
		foreach (MVWorldObjectClient item in blueprintWorldObjectsByType)
		{
			item.SelectedChanged -= WorldObjectClient_SelectedChangedHandler;
		}
		WOCM.UnsubscribeWOCreatedEvent(typeof(MVRotator), WOCM_WorldObjectCreatedHandler);
		base.Destroy();
	}

	public override void SetWorldObjectToPurchased()
	{
		base.SetWorldObjectToPurchased();
		CubeModel.SetWorldObjectToPurchased();
	}

	public override void AddPreviewBox()
	{
	}
}
