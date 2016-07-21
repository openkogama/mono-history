using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public abstract class MVLogicObject : MVWorldObjectClient, IUpdatecontrollerSubscriber
{
	protected bool disabledByLod;

	private Bounds localBounds = new Bounds(Vector3.zero, Vector3.one);

	protected CullingSubscriberBase cullingSubscriberBase;

	private GameObject lodGameObject;

	protected MVLogicObject(Dictionary<object, object> data, ObjectPrefab prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanClone | InteractionFlags.CanResetLogic;
		PlayInteractionType = PlayInteractionType.ExcludeFromInteraction;
		gameObject.layer = LayerMask.NameToLayer("Logic");
		previewLayerMask |= LayerFlags.Logic;
		localBounds = ComputeLocalBounds(gameObject.transform.position, component.MeshRenderers);
	}

	protected virtual void OnUpdate()
	{
	}

	public override void Reset()
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		UpdateController.AddUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_20);
		SharedLinkFunctions.EvaluateLinks(this);
		SharedLinkFunctions.UpdateOutputLinks(this);
	}

	protected CullingSubscriberBase SetupCulling(GameObject lodGameObject)
	{
		this.lodGameObject = lodGameObject;
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		cullingSubscriberBase = new CullingSubscriberBase(2f, WorldPosition, OnStateChanged);
		return cullingSubscriberBase;
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos;
	}

	protected virtual void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
		lodGameObject.SetActive(active);
	}

	public override void Destroy()
	{
		UpdateController.RemoveUpdateObject(this);
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
	}

	public virtual void UpdateControllerUpdate()
	{
		OnUpdate();
		if (HasInputConnector)
		{
			SharedLinkFunctions.EvaluateLinks(this);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return localBounds;
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
	}

	public override void DeSelect()
	{
		RemoveSelectionBox();
		selectedConnector = SelectedConnector.None;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		if (HasInputConnector)
		{
			inputConnectorObject.SetActive(value: false);
		}
		if (HasObjectConnector)
		{
			objectConnectorObject.SetActive(value: false);
		}
		if (HasOutputConnector)
		{
			outputConnectorObject.SetActive(value: false);
		}
	}

	protected Bounds ComputeLocalBounds(Vector3 origin, MeshRenderer[] meshRenderers)
	{
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		if (meshRenderers.Length > 0)
		{
			Bounds bounds = meshRenderers[0].bounds;
			bounds.center -= origin;
			result = bounds;
			for (int i = 1; i < meshRenderers.Length; i++)
			{
				bounds = meshRenderers[i].bounds;
				bounds.center -= origin;
				result.Encapsulate(bounds);
			}
		}
		else
		{
			Debug.LogWarning("Mesh filters required for correct bounds", GameObject);
		}
		return result;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one *= 1f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public virtual void UpdateControllerFixedUpdate()
	{
	}
}
