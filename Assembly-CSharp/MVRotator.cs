using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class MVRotator : MVMovable
{
	protected CullingSubscriberBase cullingSubscriberBase;

	private Vector3 initAngularVelocity;

	private static HashSet<MVRotator> selectedRotators = new HashSet<MVRotator>();

	public override MVWorldObjectDocumentationType DocumentationType
	{
		get
		{
			if (Horizontal)
			{
				return MVWorldObjectDocumentationType.HorizontalRotator;
			}
			if (Vertical)
			{
				return MVWorldObjectDocumentationType.VerticalRotator;
			}
			return MVWorldObjectDocumentationType.Missing;
		}
	}

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
		SetupCulling();
	}

	private void SetupCulling()
	{
		cullingSubscriberBase = new CullingSubscriberBase(OnStateChanged);
		SetupCullingSphere();
		MVCubeModelInstance mVCubeModelInstance = CubeModel;
		mVCubeModelInstance.Changed = (Action<CubeModelChangedEventArgs>)Delegate.Combine(mVCubeModelInstance.Changed, new Action<CubeModelChangedEventArgs>(Changed));
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
	}

	private void OnPositionChanged(object sender, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = Position;
	}

	private void SetupCullingSphere()
	{
		Bounds worldBounds = CubeModel.GetWorldBounds();
		float radius = (Position - worldBounds.center).magnitude + worldBounds.extents.magnitude;
		cullingSubscriberBase.Setup(radius, Position);
		cullingSubscriberBase.DistanceBandIndex = 2;
	}

	private void Changed(CubeModelChangedEventArgs cubeModelChangedEventArgs)
	{
		SetupCullingSphere();
	}

	private void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool visible = CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
		SetVisible(visible);
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
		base.Destroy();
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
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
