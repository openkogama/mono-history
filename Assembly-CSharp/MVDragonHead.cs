using System.Collections;
using System.Collections.Generic;

public class MVDragonHead : MVBlueprintBase
{
	private const string prefabPath = "Prefabs/Blueprints/DragonHead";

	private MVCubeModelInstance model;

	public MVDragonHead(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/DragonHead", worldObjects)
	{
	}

	public void initThis(MVDragon d)
	{
		gameObject.GetComponent<DragonHead>().Initialize(d);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeCommon();
	}

	private void InitializeCommon()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		model = GetChild((int)childIdMap["head"]) as MVCubeModelInstance;
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.DirectlySelectable | InteractionFlags.CanEdit;
		if (model != null)
		{
			model.GameObject.transform.position = gameObject.transform.position;
		}
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		if (model == null)
		{
			return false;
		}
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(this);
		e.SelectWO(model.Id, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		return true;
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		e.ExitGroupToRoot();
		return true;
	}
}
