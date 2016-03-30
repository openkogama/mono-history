using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public abstract class MVLogicObject : MVWorldObjectClient
{
	protected bool disabledByLod;

	private Bounds localBounds = new Bounds(Vector3.zero, Vector3.one);

	protected float cullDistance = 145f;

	protected MVLogicObject(Dictionary<object, object> data, GameObject prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanClone | InteractionFlags.CanResetLogic;
		PlayInteractionType = PlayInteractionType.ExcludeFromInteraction;
		gameObject.layer = LayerMask.NameToLayer("Logic");
		previewLayerMask |= LayerFlags.Logic;
		localBounds = ComputeLocalBounds(gameObject.transform.position, gameObject.GetComponentsInChildren<MeshRenderer>());
	}

	protected virtual void OnUpdate()
	{
	}

	public override void Reset()
	{
	}

	public void Update()
	{
		OnUpdate();
		if (HasInputConnector)
		{
			SharedLinkFunctions.EvaluateLinks(this);
		}
	}

	public virtual void FixedUpdate()
	{
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

	public override void Initialize()
	{
		base.Initialize();
		SharedLinkFunctions.EvaluateLinks(this);
		SharedLinkFunctions.UpdateOutputLinks(this);
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

	public override void ChangeLOD(float distance)
	{
		if (disabledByLod && distance < cullDistance)
		{
			disabledByLod = false;
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			disabledByLod = true;
			MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].enabled = false;
			}
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
}
