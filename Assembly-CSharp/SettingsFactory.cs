using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

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
	private SoundInventoryController soundInventoryControllerPrefab;

	[SerializeField]
	private LeverSettings leverSettingsPrefab;

	[SerializeField]
	private ShootablePlateSettings shootablePlateSettingsPrefab;

	[SerializeField]
	private CollectTheItemSettings collectTheItemSettingsPrefab;

	[SerializeField]
	private CollectTheItemDropoffSettings collectTheItemDropoffSettingsPrefab;

	[SerializeField]
	private GodzillaSettings godzillaSettingsPrefab;

	[SerializeField]
	private LevelRequirementSettings levelRequirementSettingsPrefab;

	[SerializeField]
	private GameCoinRequirementSettings gameCoinRequirementSettingsPrefab;

	[SerializeField]
	private StarsRequirementSettings starsRequirementSettingsPrefab;

	[SerializeField]
	private TeamRequirementSettings teamRequirementSettingsPrefab;

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
		case WorldObjectType.GodzillaTrigger:
		{
			GodzillaSettings godzillaSettings = Object.Instantiate(godzillaSettingsPrefab);
			godzillaSettings.Initialize(woID, gameObject);
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

	private void CreateBlueprintSettings(int woID)
	{
		MVWorldObject worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		Dictionary<object, object> dictionary = (Dictionary<object, object>)worldObjectClient.Data["BlueprintData"];
		switch ((BlueprintType)(byte)dictionary[BlueprintData.ClientSideType.ToString()])
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
