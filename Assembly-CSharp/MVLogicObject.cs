using MV.WorldObject;
using UnityEngine;

public abstract class MVLogicObject : MVWorldObjectClient, WorldObjectWithClone, WorldObjectWithLogicReset
{
	private Color selectedColor = new Color(1f, 0.6f, 0.6f, 1f);

	private Color[] deselectColors;

	protected MVLogicObject()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	}

	protected virtual void OnUpdate()
	{
	}

	public override void ResetLogic()
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

	public override void Select(Color color)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (deselectColors == null)
		{
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			int num = 0;
			deselectColors = new Color[componentsInChildren.Length];
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val in array)
			{
				ref Color reference = ref deselectColors[num++];
				reference = new Color(((Renderer)val).material.color.r, ((Renderer)val).material.color.g, ((Renderer)val).material.color.b, ((Renderer)val).material.color.a);
				((Renderer)val).material.color = selectedColor;
			}
		}
	}

	public override void DeSelect()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (deselectColors != null)
		{
			MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
			int num = 0;
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val in array)
			{
				((Renderer)val).material.color = deselectColors[num++];
			}
			deselectColors = null;
			selectedConnector = SelectedConnector.None;
		}
	}

	public override void Initialize()
	{
		SharedLinkFunctions.EvaluateLinks(this);
		SharedLinkFunctions.UpdateOutputLinks(this);
	}

	public void Clone()
	{
		MVGameController.Instance.EditorController.CloneLogicObject(WorldObjectType, Data);
	}

	public void Reset()
	{
		MVGameController.Instance.Game.ResetLogicChunk(id);
	}
}
