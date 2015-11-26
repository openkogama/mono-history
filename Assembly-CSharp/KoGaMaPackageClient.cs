using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class KoGaMaPackageClient
{
	public Dictionary<int, RuntimePrototypeCubeModel> prototypes = new Dictionary<int, RuntimePrototypeCubeModel>();

	public Dictionary<int, MVWorldObjectClient> worldObjects = new Dictionary<int, MVWorldObjectClient>();

	public Dictionary<int, Link> links = new Dictionary<int, Link>();

	public Dictionary<int, ObjectLink> objectLinks = new Dictionary<int, ObjectLink>();

	public int worldObjectRoot;

	public KoGaMaPackageClient(BytePacker koGaMaData, bool readRuntimeValues)
	{
		worldObjectRoot = KogamaDataHandler.GetKoGaMaData(koGaMaData, HandleDeserializedData, readRuntimeValues);
	}

	public void Destroy()
	{
		MVWorldObjectClient mVWorldObjectClient = worldObjects[worldObjectRoot];
		GameObject gameObject = mVWorldObjectClient.GameObject;
		mVWorldObjectClient.Destroy();
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}

	public void HandleDeserializedData(Dictionary<object, object> returnData, KogamaDataType dataType)
	{
		switch (dataType)
		{
		case KogamaDataType.Prototypes:
			AddPrototype(returnData);
			break;
		case KogamaDataType.WorldObjects:
			AddWorldObject(returnData);
			break;
		case KogamaDataType.Links:
			AddLink(returnData);
			break;
		case KogamaDataType.ObjectLinks:
			AddObjectLink(returnData);
			break;
		}
	}

	private void AddLink(Dictionary<object, object> data)
	{
		Link link = new Link();
		link.id = (int)data[LinkDataParameter.Id];
		link.outputWOID = (int)data[LinkDataParameter.OutputWOID];
		link.inputWOID = (int)data[LinkDataParameter.InputWOID];
		links.Add(link.id, link);
	}

	private void AddObjectLink(Dictionary<object, object> data)
	{
		ObjectLink objectLink = new ObjectLink();
		objectLink.id = (int)data[ObjectLinkDataParameter.Id];
		objectLink.objectConnectorWOID = (int)data[ObjectLinkDataParameter.ObjectLinkConnectorWOID];
		objectLink.objectWOID = (int)data[ObjectLinkDataParameter.ObjectWOID];
		objectLinks.Add(objectLink.id, objectLink);
	}

	private void AddPrototype(Dictionary<object, object> data)
	{
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = new RuntimePrototypeCubeModel((int)data[PrototypeDataParameters.Id], (int)data[PrototypeDataParameters.AuthorProfileId], (float)data[PrototypeDataParameters.Scale], (byte[])data[PrototypeDataParameters.Data]);
		prototypes.Add(runtimePrototypeCubeModel.PrototypeId, runtimePrototypeCubeModel);
	}

	private void AddWorldObject(Dictionary<object, object> data)
	{
		MVWorldObjectClient mVWorldObjectClient = WorldObjectFactory(data, worldObjects, prototypes);
		worldObjects.Add(mVWorldObjectClient.Id, mVWorldObjectClient);
	}

	public static float Compare(KoGaMaPackageClient koGaMaPackageClientOriginal, KoGaMaPackageClient koGaMaPackageClientDesendant)
	{
		MVWorldObjectClient mVWorldObjectClient = koGaMaPackageClientOriginal.worldObjects[koGaMaPackageClientOriginal.worldObjectRoot];
		MVWorldObjectClient mVWorldObjectClient2 = koGaMaPackageClientDesendant.worldObjects[koGaMaPackageClientDesendant.worldObjectRoot];
		if (mVWorldObjectClient2.WorldObjectType != mVWorldObjectClient.WorldObjectType)
		{
			Debug.LogError($"Comparing packages with different worldObject types {mVWorldObjectClient.WorldObjectType} and {mVWorldObjectClient2.WorldObjectType}");
			return 0f;
		}
		int matchingCubeCount = 0;
		int investigatedCubeCount = 0;
		mVWorldObjectClient.Compare(mVWorldObjectClient2, visibleCubesOnly: true, ref matchingCubeCount, ref investigatedCubeCount);
		if (investigatedCubeCount == 0)
		{
			return 0f;
		}
		Debug.Log($"InvestigatedCubeCount {investigatedCubeCount} MatchingCubeCount {matchingCubeCount} ");
		return (float)matchingCubeCount / (float)investigatedCubeCount;
	}

	public static MVWorldObjectClient WorldObjectFactory(Dictionary<object, object> worldObjectData, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		WorldObjectType worldObjectType = (WorldObjectType)(int)worldObjectData[WorldObjectDataParameters.WorldObjectType];
		if (worldObjectType == WorldObjectType.Avatar)
		{
		}
		switch (worldObjectType)
		{
		case WorldObjectType.Avatar:
			if ((int)worldObjectData[WorldObjectDataParameters.OwnerActorNumber] == MVGameControllerBase.Game.LocalPlayer.ActorNr)
			{
				return new MVAvatarLocal(worldObjectData, worldObjects);
			}
			return new MVAvatarRemote(worldObjectData, worldObjects);
		case WorldObjectType.CubeModel:
			return new MVCubeModelInstance(worldObjectData, worldObjects, prototypes);
		case WorldObjectType.Skybox:
			return new MVSkybox(worldObjectData, worldObjects);
		case WorldObjectType.PointLight:
			return new MVPointLight(worldObjectData, worldObjects);
		case WorldObjectType.LightPreset:
			return new MVPointLightPreset(worldObjectData, worldObjects);
		case WorldObjectType.SpawnPoint:
			Debug.LogError("Attempt to create abstract SpawnPoint. Is Server up-to-date? return ing blue spawn-point");
			return new MVSpawnPointBlue(worldObjectData, worldObjects);
		case WorldObjectType.SpawnPointBlue:
			return new MVSpawnPointBlue(worldObjectData, worldObjects);
		case WorldObjectType.SpawnPointRed:
			return new MVSpawnPointRed(worldObjectData, worldObjects);
		case WorldObjectType.SpawnPointGreen:
			return new MVSpawnPointGreen(worldObjectData, worldObjects);
		case WorldObjectType.SpawnPointYellow:
			return new MVSpawnPointYellow(worldObjectData, worldObjects);
		case WorldObjectType.CubeModelPrototypeTerrain:
			return new MVCubeModelPrototypeTerrain(worldObjectData, worldObjects, prototypes);
		case WorldObjectType.Group:
			return new MVGroup(worldObjectData, worldObjects);
		case WorldObjectType.TriggerBox:
			return new MVTriggerBox(worldObjectData, worldObjects);
		case WorldObjectType.SoundEmitter:
			return new MVSoundEmitter(worldObjectData, worldObjects);
		case WorldObjectType.Flag:
			return new MVFlag(worldObjectData, worldObjects);
		case WorldObjectType.TestLogicCube:
			return new TestLogicCube(worldObjectData, worldObjects);
		case WorldObjectType.Battery:
			return new MVBattery(worldObjectData, worldObjects);
		case WorldObjectType.ToggleBox:
			return new MVToggleBox(worldObjectData, worldObjects);
		case WorldObjectType.Negate:
			return new MVNegate(worldObjectData, worldObjects);
		case WorldObjectType.And:
			return new MVAnd(worldObjectData, worldObjects);
		case WorldObjectType.Explosives:
			return new MVExplosives(worldObjectData, worldObjects);
		case WorldObjectType.TextMsg:
			return new MVTextMsg(worldObjectData, worldObjects);
		case WorldObjectType.Fire:
			return new MVFire(worldObjectData, worldObjects);
		case WorldObjectType.Smoke:
			return new MVSmoke(worldObjectData, worldObjects);
		case WorldObjectType.TimeTrigger:
			return new MVTimeTrigger(worldObjectData, worldObjects);
		case WorldObjectType.Teleporter:
			return new MVTeleporter(worldObjectData, worldObjects);
		case WorldObjectType.Goal:
			return new MVGoal(worldObjectData, worldObjects);
		case WorldObjectType.PickupItemSpawner:
			return new MVPickupItemBase(worldObjectData, worldObjects);
		case WorldObjectType.PickupCubeGun:
			return new MVCubeGun(worldObjectData, worldObjects);
		case WorldObjectType.CubeModelTerrainFineGrained:
			return new MVCubeModelFineGrainedTerrain(worldObjectData, worldObjects, prototypes);
		case WorldObjectType.PressurePlate:
			return new MVPressurePlate(worldObjectData, worldObjects);
		case WorldObjectType.ModelToggle:
			return new MVObjectEnabler(worldObjectData, worldObjects);
		case WorldObjectType.PulseBox:
			return new MVPulseBox(worldObjectData, worldObjects);
		case WorldObjectType.SentryGun:
			return new MVSentryGun(worldObjectData, worldObjects);
		case WorldObjectType.RandomBox:
			return new MVRandomBox(worldObjectData, worldObjects);
		case WorldObjectType.WaterPlane:
			return new MVWaterPlane(worldObjectData, worldObjects);
		case WorldObjectType.WaterPlanePreset:
			return new MVWaterPlanePreset(worldObjectData, worldObjects);
		case WorldObjectType.CollectibleItem:
			if (MVGameControllerBase.GameSessionData.planetID == 2527584 && MVGameControllerBase.GameSessionData.region == "br")
			{
				return new MVCollectible(worldObjectData, worldObjects, "Prefabs/CollectibleObjectFanta");
			}
			return new MVCollectible(worldObjectData, worldObjects);
		case WorldObjectType.MovingPlatformNode:
			return new MVMovingPlatformNode(worldObjectData, worldObjects);
		case WorldObjectType.Ghost:
			return new MVGhostInstance(worldObjectData, worldObjects);
		case WorldObjectType.CheckPoint:
			return new MVCheckpoint(worldObjectData, worldObjects);
		case WorldObjectType.Blueprint:
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)worldObjectData[WorldObjectDataParameters.Data];
			Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary["BlueprintData"];
			BlueprintType blueprintType = (BlueprintType)(byte)dictionary2[BlueprintData.ClientSideType.ToString()];
			switch (blueprintType)
			{
			case BlueprintType.Movable:
				return new MVMovable(worldObjectData, worldObjects);
			case BlueprintType.Teleporter:
				return new MVTeleportGroup(worldObjectData, worldObjects);
			case BlueprintType.SentryGun:
				return new MVSentryGunBlueprint(worldObjectData, worldObjects);
			case BlueprintType.Body:
				return new MVBody(worldObjectData, worldObjects);
			case BlueprintType.MovingPlatform:
				return new MVMovingPlatform(worldObjectData, worldObjects);
			case BlueprintType.MovingPlatformGroup:
				return new MVMovingPlatformGroup(worldObjectData, worldObjects);
			case BlueprintType.Rotator:
				return new MVRotator(worldObjectData, worldObjects);
			case BlueprintType.Ghost:
				return new MVGhost(worldObjectData, worldObjects);
			default:
				Debug.LogError("WOCM trying to create unknown blueprint: " + blueprintType);
				return null;
			}
		}
		case WorldObjectType.HoverCraft:
			return new MVHoverCraft(worldObjectData, worldObjects);
		case WorldObjectType.JetPack:
			return new MVJetPack(worldObjectData, worldObjects);
		case WorldObjectType.WorldObjectSpawnerVehicle:
			return new MVWorldObjectSpawnerVehicle(worldObjectData, worldObjects);
		case WorldObjectType.RoundCube:
			return new MVRoundCube(worldObjectData, worldObjects);
		case WorldObjectType.AdvancedGhost:
			return new MVAdvancedGhost(worldObjectData, worldObjects);
		case WorldObjectType.KillLimit:
			return new MVKillLimit(worldObjectData, worldObjects);
		case WorldObjectType.OculusKillLimit:
			return new MVOculusKillLimit(worldObjectData, worldObjects);
		case WorldObjectType.HamsterWheel:
			return new MVHamsterWheel(worldObjectData, worldObjects);
		case WorldObjectType.CameraSettings:
			return new MVCameraSettings(worldObjectData, worldObjects);
		case WorldObjectType.GravityCube:
			return new MVGravityCube(worldObjectData, worldObjects);
		case WorldObjectType.GameCoin:
			return new MVGameCoin(worldObjectData, worldObjects);
		case WorldObjectType.GameCoinChest:
			return new MVGameCoinChest(worldObjectData, worldObjects);
		case WorldObjectType.WindTurbine:
			return new WindTurbine(worldObjectData, worldObjects);
		case WorldObjectType.ShootableButton:
			return new ShootableButton(worldObjectData, worldObjects);
		case WorldObjectType.UseLever:
			return new UseLever(worldObjectData, worldObjects);
		default:
			Debug.LogError("WOCM trying to create unknown type: " + worldObjectType);
			return null;
		}
	}

	public override string ToString()
	{
		return "protypes.Count " + prototypes.Count + "\n worldObjects.Count " + worldObjects.Count + "\n links.Count " + links.Count + "\n objectLinks.Count " + objectLinks.Count;
	}
}
