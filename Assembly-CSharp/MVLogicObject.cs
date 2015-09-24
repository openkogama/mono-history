using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public abstract class MVLogicObject : MVWorldObjectClient
{
	protected bool disabledByLod;

	private Bounds localBounds = new Bounds(Vector3.zero, Vector3.one);

	protected float cullDistance = 145f;

	protected MVLogicObject(Dictionary<object, object> data, string prefabPath, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
	{
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanClone | InteractionFlags.CanResetLogic;
		PlayInteractionType = PlayInteractionType.ExcludeFromInteraction;
		gameObject.layer = LayerMask.NameToLayer("Logic");
		previewLayerMask |= LayerFlags.Logic;
		MeshRenderer[] meshRenderers = (from r in gameObject.GetComponentsInChildren<MeshRenderer>()
			where r.name != "ioConnectorCube" && r.name != "ioConnectorSphere"
			select r).ToArray();
		localBounds = ComputeLocalBounds(gameObject.transform.position, meshRenderers);
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
		(from r in GameObject.GetComponentsInChildren<MeshRenderer>()
			where r.name == "ioConnectorCube" || r.name == "ioConnectorSphere"
			select r).ToList().ForEach((MeshRenderer r) =>
		{
			r.gameObject.SetActive(value: false);
		});
	}

	public override void ChangeLOD(float distance)
	{
		if (disabledByLod && distance < cullDistance)
		{
			disabledByLod = false;
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			disabledByLod = true;
			Renderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren2;
			foreach (Renderer renderer2 in array2)
			{
				renderer2.enabled = false;
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
}
