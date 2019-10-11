using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsFactory : MonoBehaviour
{
	[SerializeField]
	private bool previewSettingsPopup;

	[SerializeField]
	private WorldObjectType worldObjectType;

	[SerializeField]
	private MaterialsController materialsController;

	[SerializeField]
	private PointLightSettings pointLightSettingsPrefab;

	[SerializeField]
	private SmokeSettings smokeSettingsPrefab;

	[SerializeField]
	private KillLimitSettings killLimitSettingsPrefab;

	[SerializeField]
	private CubeGunSettings cubeGunSettingsPrefab;

	[SerializeField]
	private MessageBoxSettings messageBoxSettingsPrefab;

	[SerializeField]
	private SkyboxSettings skyboxSettingsPrefab;

	[SerializeField]
	private RoundCubeSettings roundCubeSettingsPrefab;

	[SerializeField]
	private TimeTriggerSettings timeTriggerSettingsPrefab;

	[SerializeField]
	private ToggleBoxSettings toggleBoxSettingsPrefab;

	[SerializeField]
	private ObjectEnablerSettings objectEnablerPrefab;

	[SerializeField]
	private PulseBoxSettings pulseBoxPrefab;

	[SerializeField]
	private CountingCubeSettings countingCubeSettingsPrefab;

	[SerializeField]
	private RotatorSettings rotatorSettingsPrefab;

	[SerializeField]
	private WindTurbineSettings windTurbineSettingsPrefab;

	[SerializeField]
	private OculusSettings oculusSettingsPrefab;

	[SerializeField]
	private PressurePlateSettings pressurePlateSettingsPrefab;

	[SerializeField]
	private GameCoinChestSettings gameCoinChestSettingsPrefab;

	[SerializeField]
	private WaterBoxSettings waterBoxSettingsPrefab;

	[SerializeField]
	private CameraBoxSettings cameraBoxSettingsPrefab;

	[SerializeField]
	private MovablesSettings movablesSettingsPrefab;

	[SerializeField]
	private SoundEmitterSettings soundEmitterSettingsPrefab;

	[SerializeField]
	private GlobalSoundEmitterSettings globalSoundEmitterSettingsPrefab;

	[SerializeField]
	private SoundInventoryController soundInventoryControllerPrefab;

	[SerializeField]
	private SoundInventoryController globalSoundInventoryControllerPrefab;

	[SerializeField]
	private LeverSettings leverSettingsPrefab;

	[SerializeField]
	private ShootablePlateSettings shootablePlateSettingsPrefab;

	[SerializeField]
	private CollectTheItemSettings collectTheItemSettingsPrefab;

	[SerializeField]
	private CollectTheItemDropoffSettings collectTheItemDropoffSettingsPrefab;

	[SerializeField]
	private FireSettings fireSettingsPrefab;

	[SerializeField]
	private TriggerCubeSettings triggerCubeSettingsPrefab;

	[SerializeField]
	private TeamEditorSettings teamEditorSettingsPrefab;

	[SerializeField]
	private GamePointSettings gamePointSettingsPrefab;

	[SerializeField]
	private GamePointMinorRewardSettings gamePointMinorRewardSettingsPrefab;

	[SerializeField]
	private GamePointChestSettings gamePointChestSettingsPrefab;

	[SerializeField]
	private SpawnRoleEditorMenu spawnRoleEditorPrefab;

	[SerializeField]
	private RespawnSettings respawnSettingsPrefab;

	[SerializeField]
	private LevelRequirementSettings levelRequirementSettingsPrefab;

	[SerializeField]
	private GameCoinRequirementSettings gameCoinRequirementSettingsPrefab;

	[SerializeField]
	private StarsRequirementSettings starsRequirementSettingsPrefab;

	[SerializeField]
	private TeamRequirementSettings teamRequirementSettingsPrefab;

	[SerializeField]
	private GameRankRequirementSettings gameRankRequirementSettingsPrefab;

	public void CreateSettingsDialog(int woID)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		CreateSettingsDialog(woID, worldObjectClient.WorldObjectType);
	}

	public void CreateSettingsDialog(int woID, UseRequirementType requirementType)
	{
		switch (requirementType)
		{
		case UseRequirementType.Level:
		{
			LevelRequirementSettings levelRequirementSettings = Object.Instantiate(levelRequirementSettingsPrefab);
			levelRequirementSettings.Initialize(woID, gameObject);
			break;
		}
		case UseRequirementType.GameCoin:
		{
			GameCoinRequirementSettings gameCoinRequirementSettings = Object.Instantiate(gameCoinRequirementSettingsPrefab);
			gameCoinRequirementSettings.Initialize(woID, gameObject);
			break;
		}
		case UseRequirementType.Star:
		{
			StarsRequirementSettings starsRequirementSettings = Object.Instantiate(starsRequirementSettingsPrefab);
			starsRequirementSettings.Initialize(woID, gameObject);
			break;
		}
		case UseRequirementType.Team:
		{
			TeamRequirementSettings teamRequirementSettings = Object.Instantiate(teamRequirementSettingsPrefab);
			teamRequirementSettings.Initialize(woID, gameObject);
			break;
		}
		case UseRequirementType.GameRank:
		{
			GameRankRequirementSettings gameRankRequirementSettings = Object.Instantiate(gameRankRequirementSettingsPrefab);
			gameRankRequirementSettings.Initialize(woID, gameObject);
			break;
		}
		}
	}

	public void CreateSettingsDialog(int woID, WorldObjectType worldObjectType)
	{
		switch (worldObjectType)
		{
		case WorldObjectType.PointLight:
		{
			PointLightSettings pointLightSettings = Object.Instantiate(pointLightSettingsPrefab);
			pointLightSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.Smoke:
		{
			SmokeSettings smokeSettings = Object.Instantiate(smokeSettingsPrefab);
			smokeSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.KillLimit:
		{
			KillLimitSettings killLimitSettings2 = Object.Instantiate(killLimitSettingsPrefab);
			killLimitSettings2.Initialize(woID, gameObject, TM._("Kill Limit"));
			break;
		}
		case WorldObjectType.OculusKillLimit:
		{
			KillLimitSettings killLimitSettings = Object.Instantiate(killLimitSettingsPrefab);
			killLimitSettings.Initialize(woID, gameObject, TM._("Oculus Kill Limit"));
			break;
		}
		case WorldObjectType.PickupCubeGun:
		{
			CubeGunSettings cubeGunSettings = Object.Instantiate(cubeGunSettingsPrefab);
			cubeGunSettings.Initialize(woID, materialsController);
			break;
		}
		case WorldObjectType.TextMsg:
		{
			MessageBoxSettings messageBoxSettings = Object.Instantiate(messageBoxSettingsPrefab);
			messageBoxSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.Skybox:
		{
			SkyboxSettings skyboxSettings = Object.Instantiate(skyboxSettingsPrefab);
			skyboxSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.RoundCube:
		{
			RoundCubeSettings roundCubeSettings = Object.Instantiate(roundCubeSettingsPrefab);
			roundCubeSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.TimeTrigger:
		{
			TimeTriggerSettings timeTriggerSettings = Object.Instantiate(timeTriggerSettingsPrefab);
			timeTriggerSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.ToggleBox:
		{
			ToggleBoxSettings toggleBoxSettings = Object.Instantiate(toggleBoxSettingsPrefab);
			toggleBoxSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.ModelToggle:
		{
			ObjectEnablerSettings objectEnablerSettings = Object.Instantiate(objectEnablerPrefab);
			objectEnablerSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.PulseBox:
		{
			PulseBoxSettings pulseBoxSettings = Object.Instantiate(pulseBoxPrefab);
			pulseBoxSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.CountingCube:
		{
			CountingCubeSettings countingCubeSettings = Object.Instantiate(countingCubeSettingsPrefab);
			countingCubeSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.Blueprint:
			CreateBlueprintSettings(woID);
			break;
		case WorldObjectType.WindTurbine:
		{
			WindTurbineSettings windTurbineSettings = Object.Instantiate(windTurbineSettingsPrefab);
			windTurbineSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.AdvancedGhost:
		{
			OculusSettings oculusSettings = Object.Instantiate(oculusSettingsPrefab);
			oculusSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.PressurePlate:
		{
			PressurePlateSettings pressurePlateSettings = Object.Instantiate(pressurePlateSettingsPrefab);
			pressurePlateSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.GameCoinChest:
		{
			GameCoinChestSettings gameCoinChestSettings = Object.Instantiate(gameCoinChestSettingsPrefab);
			gameCoinChestSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.WaterPlane:
		{
			WaterBoxSettings waterBoxSettings = Object.Instantiate(waterBoxSettingsPrefab);
			waterBoxSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.CameraSettings:
		{
			CameraBoxSettings cameraBoxSettings = Object.Instantiate(cameraBoxSettingsPrefab);
			cameraBoxSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.SoundEmitter:
		{
			SoundEmitterSettings soundEmitterSettings = Object.Instantiate(soundEmitterSettingsPrefab);
			soundEmitterSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.GlobalSoundEmitter:
		{
			GlobalSoundEmitterSettings globalSoundEmitterSettings = Object.Instantiate(globalSoundEmitterSettingsPrefab);
			globalSoundEmitterSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.UseLever:
		{
			LeverSettings leverSettings = Object.Instantiate(leverSettingsPrefab);
			leverSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.ShootableButton:
		{
			ShootablePlateSettings shootablePlateSettings = Object.Instantiate(shootablePlateSettingsPrefab);
			shootablePlateSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.CollectTheItemCollectable:
		{
			CollectTheItemSettings collectTheItemSettings = Object.Instantiate(collectTheItemSettingsPrefab);
			collectTheItemSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.CollectTheItemDropOff:
		{
			CollectTheItemDropoffSettings collectTheItemDropoffSettings = Object.Instantiate(collectTheItemDropoffSettingsPrefab);
			collectTheItemDropoffSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.Fire:
		{
			FireSettings fireSettings = Object.Instantiate(fireSettingsPrefab);
			fireSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.TriggerCube:
		{
			TriggerCubeSettings triggerCubeSettings = Object.Instantiate(triggerCubeSettingsPrefab);
			triggerCubeSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.GamePointChest:
		{
			GamePointChestSettings gamePointChestSettings = Object.Instantiate(gamePointChestSettingsPrefab);
			gamePointChestSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.TeamEditor:
		{
			TeamEditorSettings teamEditorSettings = Object.Instantiate(teamEditorSettingsPrefab);
			teamEditorSettings.Initialize(woID, gameObject);
			break;
		}
		case WorldObjectType.AvatarSpawnRoleCreator:
		{
			SpawnRoleEditorMenu spawnRoleEditor = Object.Instantiate(spawnRoleEditorPrefab);
			spawnRoleEditor.Initialize(woID);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(spawnRoleEditor.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
			break;
		}
		default:
			Debug.LogError("WorldObjectType has no settings dialogue.");
			break;
		}
	}

	public void CreateSoundsInventory(int woID)
	{
		SoundInventoryController soundInventoryController = Object.Instantiate(soundInventoryControllerPrefab);
		soundInventoryController.Initialize(woID, gameObject);
	}

	public void CreateGlobalSoundsInventory(int woID)
	{
		SoundInventoryController soundInventoryController = Object.Instantiate(globalSoundInventoryControllerPrefab);
		soundInventoryController.Initialize(woID, gameObject);
	}

	public void CreateGamePointsSettings(int woID)
	{
		GamePointSettings gamePointSettings = Object.Instantiate(gamePointSettingsPrefab);
		gamePointSettings.Initialize(woID, gameObject);
	}

	public void CreateGamePointsMinorRewardSettings(int woID)
	{
		GamePointMinorRewardSettings gamePointMinorRewardSettings = Object.Instantiate(gamePointMinorRewardSettingsPrefab);
		gamePointMinorRewardSettings.Initialize(woID, gameObject);
	}

	public void CreateRespawnSetting(int woID)
	{
		RespawnSettings respawnSettings = Object.Instantiate(respawnSettingsPrefab);
		respawnSettings.Initialize(woID, gameObject);
	}

	private void CreateBlueprintSettings(int woID)
	{
		MVWorldObject worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		Dictionary<object, object> dictionary = (Dictionary<object, object>)worldObjectClient.Data["BlueprintData"];
		switch ((BlueprintType)dictionary[BlueprintData.ClientSideType.ToString()])
		{
		case BlueprintType.MovingPlatformGroup:
			CreateMovablesSettings(woID);
			break;
		case BlueprintType.Rotator:
			CreateRotatorSettings(woID);
			break;
		}
	}

	private void CreateRotatorSettings(int woID)
	{
		RotatorSettings rotatorSettings = Object.Instantiate(rotatorSettingsPrefab);
		rotatorSettings.Initialize(woID, gameObject);
	}

	private void CreateMovablesSettings(int woID)
	{
		MovablesSettings movablesSettings = Object.Instantiate(movablesSettingsPrefab);
		movablesSettings.Initialize(woID, gameObject);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying && previewSettingsPopup)
		{
			CreateSettingsDialog(-1, worldObjectType);
		}
	}
}
