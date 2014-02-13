using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVSentryGunBlueprint : MVBlueprintBase
{
	private MVCubeModelBase editableCubes;

	private MVSentryGun gun;

	public MVCubeModelBase EditableCubesWO => editableCubes;

	public MVSentryGunBlueprint(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
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
		Hashtable hashtable = (Hashtable)Data["BlueprintData"];
		Hashtable hashtable2 = (Hashtable)hashtable["ChildrenMap"];
		if (hashtable2 == null)
		{
			Debug.LogWarning((object)"MVSentryGunBlueprint does not have any children. Removing it");
			MVGameController.Instance.WOCM.UnregisterWorldObject(id);
			return;
		}
		editableCubes = (MVCubeModelBase)GetChild("editableCubeModel");
		gun = (MVSentryGun)GetChild("sentryGun");
		if (editableCubes == null)
		{
			Debug.Log((object)"Missing editable cubes");
		}
		if (gun == null)
		{
			Debug.Log((object)"Missing gun");
		}
		gun.RaycastIgnoreWorldObjectIds = new HashSet<int> { gun.Id, editableCubes.Id };
		gun.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup | InteractionFlags.NotUserTransformable;
		editableCubes.InteractionFlags |= InteractionFlags.SelectionRequiresEditGroup | InteractionFlags.NotUserTransformable;
		editableCubes.ModelingConstraintBuilder = () => new ModelingBoxCountConstraint(editableCubes, new IntVector(-4, -2, -4), new IntVector(3, 8, 3), 50);
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		MVSentryGunBlueprint mVSentryGunBlueprint = (MVSentryGunBlueprint)wo;
		Hashtable hashtable = (Hashtable)mVSentryGunBlueprint.Data["BlueprintData"];
		Hashtable hashtable2 = (Hashtable)hashtable["ChildrenMap"];
		int key = (int)hashtable2["editableCubeModel"];
		int key2 = (int)hashtable2["sentryGun"];
		MVCubeModelInstance wo2 = (MVCubeModelInstance)koGaMaPackageClient.worldObjects[key];
		MVSentryGun wo3 = (MVSentryGun)koGaMaPackageClient.worldObjects[key2];
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("editableCubeModel");
		MVSentryGun mVSentryGun = (MVSentryGun)GetChild("sentryGun");
		return mVSentryGun.CompareWithKoGaMaPackage(wo3, koGaMaPackageClient, ref insertedByProfileId) && mVCubeModelInstance.CompareWithKoGaMaPackage(wo2, koGaMaPackageClient, ref insertedByProfileId);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(this);
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
