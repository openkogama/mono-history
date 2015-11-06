using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVRotator : MVMovable
{
	private Vector3 initAngularVelocity;

	private static HashSet<MVRotator> selectedRotators = new HashSet<MVRotator>();

	private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	public Vector3 InitAngularVelocity => initAngularVelocity;

	public bool Horizontal => AngularVelocity.x == 0f && AngularVelocity.y != 0f && AngularVelocity.z == 0f;

	public bool Vertical => AngularVelocity.x != 0f && AngularVelocity.y == 0f && AngularVelocity.z == 0f;

	public override Vector3 WorldPivot => WorldPosition;

	public MVRotator(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void Initialize()
	{
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
			Debug.LogWarning("Rotator " + id + " init - movable's cube model is NULL! If this is a new platform group restart the session. Otherwise it is broken.");
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
		Bounds localBounds = base.GetLocalBounds(boundsContext);
		if (boundsContext == BoundsContext.Preview)
		{
			Bounds bounds = new Bounds(-localBounds.center, localBounds.size);
			localBounds.Encapsulate(bounds);
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
