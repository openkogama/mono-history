using System.Collections;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIDialogBoxWrapper
{
	private static MVGUIDialogBoxWrapper _instance;

	public static MVGUIDialogBoxWrapper Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MVGUIDialogBoxWrapper();
			}
			return _instance;
		}
	}

	private MVGUIDialogBoxWrapper()
	{
	}

	public void ShowSettingsDialog(MVWorldObjectClient wo)
	{
		switch (wo.WorldObjectType)
		{
		case WorldObjectType.Skybox:
			new MVGUISettingsDialogSkybox();
			break;
		case WorldObjectType.PointLight:
			new MVGUISettingsDialogLight();
			break;
		case WorldObjectType.SoundEmitter:
			new MVGUISettingsDialogSoundEmitter();
			break;
		case WorldObjectType.TimeTrigger:
			new MVGUISettingsDialogTimeTrigger();
			break;
		case WorldObjectType.TriggerBox:
			new MVGUISettingsDialogTriggerBox();
			break;
		case WorldObjectType.TextMsg:
			new MVGUISettingsDialogTextMsg();
			break;
		case WorldObjectType.ToggleBox:
			new MVGUISettingsDialogToggleBox();
			break;
		case WorldObjectType.ModelToggle:
			new MVGUIObjectEnablerSettingsBox();
			break;
		case WorldObjectType.CubeModel:
		case WorldObjectType.Mover:
		case WorldObjectType.Path:
		case WorldObjectType.PathNode:
		case WorldObjectType.Group:
		case WorldObjectType.Action:
		case WorldObjectType.BlueprintActivator:
		case WorldObjectType.Battery:
		case WorldObjectType.Negate:
		case WorldObjectType.And:
		case WorldObjectType.Explosives:
		case WorldObjectType.Fire:
		case WorldObjectType.Smoke:
			break;
		case WorldObjectType.WaterPlane:
		case WorldObjectType.WaterPlanePreset:
			new MVGUISettingsDialogWaterPlane();
			break;
		case WorldObjectType.PulseBox:
			new MVGUISettingsDialogPulseBox();
			break;
		case WorldObjectType.PressurePlate:
			new MVGUIPressurePlateSettingsBox();
			break;
		case WorldObjectType.Blueprint:
			CreateBluprintDialog();
			break;
		case WorldObjectType.PickupCubeGun:
			new MVGUISettingsDialogCubeGun();
			break;
		case WorldObjectType.RoundCube:
			new MVGUISettingsDialogRoundCube();
			break;
		case WorldObjectType.AdvancedGhost:
			new MVGUISettingsDialogAdvancedGhost();
			break;
		default:
			Debug.LogError((object)("No settings dialog available for wo type " + wo.WorldObjectType));
			break;
		}
	}

	private bool CreateBluprintDialog()
	{
		MVWorldObjectClient settingsDialogSelectionWO = MVGameController.Instance.EditorController.GetSettingsDialogSelectionWO();
		Hashtable hashtable = (Hashtable)settingsDialogSelectionWO.Data["BlueprintData"];
		switch ((BlueprintType)(byte)hashtable[BlueprintData.ClientSideType.ToString()])
		{
		case BlueprintType.MovingPlatformGroup:
			new MVGUISettingsDialogMovingPlatform();
			return true;
		case BlueprintType.Rotator:
			new MVGUISettingsDialogRotator();
			return true;
		default:
			return false;
		}
	}
}
