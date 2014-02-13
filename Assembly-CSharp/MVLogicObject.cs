using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public abstract class MVLogicObject : MVWorldObjectClient
{
	protected bool disabledByLod;

	private Bounds localBounds = new Bounds(Vector3.zero, Vector3.one);

	protected float cullDistance = 145f;

	protected MVLogicObject(Hashtable data, string prefabPath, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanClone | InteractionFlags.CanResetLogic;
		PlayInteractionType = PlayInteractionType.ExcludeFromInteraction;
		gameObject.layer = LayerMask.NameToLayer("Logic");
		previewLayerMask |= LayerFlags.Logic;
		MeshRenderer[] meshRenderers = (from r in gameObject.GetComponentsInChildren<MeshRenderer>()
			where ((Object)r).name != "ioConnectorCube" && ((Object)r).name != "ioConnectorSphere"
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

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
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
			where ((Object)r).name == "ioConnectorCube" || ((Object)r).name == "ioConnectorSphere"
			select r).ToList().ForEach((MeshRenderer r) =>
		{
			((Component)r).gameObject.active = false;
		});
	}

	public override void ChangeLOD(float distance)
	{
		if (disabledByLod && distance < cullDistance)
		{
			disabledByLod = false;
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer val in array)
			{
				val.enabled = true;
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			disabledByLod = true;
			Renderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren2;
			foreach (Renderer val2 in array2)
			{
				val2.enabled = false;
			}
		}
	}

	protected Bounds ComputeLocalBounds(Vector3 origin, MeshRenderer[] meshRenderers)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		if (meshRenderers.Length > 0)
		{
			Bounds bounds = ((Renderer)meshRenderers[0]).bounds;
			bounds.center -= origin;
			result = bounds;
			for (int i = 1; i < meshRenderers.Length; i++)
			{
				bounds = ((Renderer)meshRenderers[i]).bounds;
				bounds.center -= origin;
				result.Encapsulate(bounds);
			}
		}
		else
		{
			Debug.LogWarning((object)"Mesh filters required for correct bounds", (Object)(object)GameObject);
		}
		return result;
	}
}
