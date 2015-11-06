using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVSentryGunBlueprint : MVBlueprintBase
{
	private MVCubeModelBase editableCubes;

	private MVSentryGun gun;

	public MVCubeModelBase EditableCubesWO => editableCubes;

	public MVSentryGunBlueprint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanEdit;
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeCommon();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	public void InitializeCommon()
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)Data["BlueprintData"];
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary["ChildrenMap"];
		if (dictionary2 == null)
		{
			Debug.LogWarning("MVSentryGunBlueprint does not have any children. Removing it");
			MVGameControllerBase.WOCM.UnregisterWorldObject(id);
			return;
		}
		editableCubes = (MVCubeModelBase)GetChild("editableCubeModel");
		gun = (MVSentryGun)GetChild("sentryGun");
		if (editableCubes == null)
		{
			Debug.Log("Missing editable cubes");
		}
		if (gun == null)
		{
			Debug.Log("Missing gun");
		}
		gun.RaycastIgnoreWorldObjectIds = new HashSet<int> { gun.Id, editableCubes.Id };
		gun.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup | InteractionFlags.NotUserTransformable;
		editableCubes.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup | InteractionFlags.NotUserTransformable;
		editableCubes.ModelingConstraintBuilder = () => new ModelingBoxCountConstraint(editableCubes, new IntVector(-4, -2, -4), new IntVector(3, 8, 3), 50);
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		MVSentryGunBlueprint mVSentryGunBlueprint = (MVSentryGunBlueprint)wo;
		Dictionary<object, object> dictionary = (Dictionary<object, object>)mVSentryGunBlueprint.Data["BlueprintData"];
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary["ChildrenMap"];
		int key = (int)dictionary2["editableCubeModel"];
		int key2 = (int)dictionary2["sentryGun"];
		MVCubeModelInstance wo2 = (MVCubeModelInstance)koGaMaPackageClient.worldObjects[key];
		MVSentryGun wo3 = (MVSentryGun)koGaMaPackageClient.worldObjects[key2];
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("editableCubeModel");
		MVSentryGun mVSentryGun = (MVSentryGun)GetChild("sentryGun");
		return mVSentryGun.CompareWithKoGaMaPackage(wo3, koGaMaPackageClient, ref insertedByProfileId) && mVCubeModelInstance.CompareWithKoGaMaPackage(wo2, koGaMaPackageClient, ref insertedByProfileId);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(this);
		e.SelectWO(EditableCubesWO.Id, addToSelection: false);
		e.Event = EditorEvent.EditCubes;
		return true;
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		e.ExitGroup();
		e.Event = EditorEvent.ESTerrainEdit;
		return true;
	}
}
