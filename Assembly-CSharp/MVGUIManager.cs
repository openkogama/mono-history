using MV.WorldObject;
using UnityEngine;

public class MVGUIManager : MonoBehaviour
{
	public Camera guiCamera;

	public MVGUIGameHUD hud;

	public MVGUIGameState gameState;

	public UXView toolsView;

	public UXView exitView;

	public UXView fullscreenView;

	public UXView loginView;

	public Material rootMaterial;

	private GameObject helpGO;

	private void Start()
	{
	}

	public void ShowEditorGUI()
	{
		toolsView.Show();
	}

	public void HideEditorGUI()
	{
		toolsView.Hide();
	}

	public void ShowLoginView()
	{
		loginView.Show();
	}

	public void HideLoginView()
	{
		loginView.Hide();
	}

	public void ShowInGameMenu()
	{
		fullscreenView.Show();
	}

	public void HideInGameMenu()
	{
		fullscreenView.Hide();
	}

	public bool ShowContextMenu(WorldObjectType type, Vector3 mousePos)
	{
		switch (type)
		{
		case WorldObjectType.CubeModel:
			MVGUIContextMenuCubeModel.New();
			return true;
		case WorldObjectType.Path:
		case WorldObjectType.PathNode:
			return true;
		case WorldObjectType.PointLight:
			MVGUILightSettingsDialog.New(MVGameController.Instance.EditorController.GetContextMenuSelectionWO());
			return true;
		case WorldObjectType.Action:
			return true;
		case WorldObjectType.Mover:
			return true;
		case WorldObjectType.BlueprintActivator:
			return true;
		case WorldObjectType.SoundEmitter:
			return true;
		case WorldObjectType.Group:
			return true;
		case WorldObjectType.TimeTrigger:
			MVGUIContextMenuTimeTrigger.New();
			return true;
		case WorldObjectType.TriggerBox:
			MVGUIContextMenuTriggerBox.New();
			return true;
		case WorldObjectType.TextMsg:
			MVGUIContextMenuTextMsg.New();
			return true;
		case WorldObjectType.ToggleBox:
			MVGUIContextMenuToggleBox.New();
			return true;
		case WorldObjectType.Battery:
		case WorldObjectType.Negate:
		case WorldObjectType.And:
		case WorldObjectType.Explosives:
		case WorldObjectType.Fire:
		case WorldObjectType.Smoke:
			MVGUIContextMenuLogicCube.New();
			return true;
		default:
			return false;
		}
	}

	public void ShowGameHUD()
	{
		hud.View.Show();
	}

	public void HideGameHUD()
	{
		hud.View.Hide();
	}

	public void LightButtonOnClick()
	{
		Debug.Log((object)"Add light - DEPRECATED - THIS IS EZ-GUI STUFF");
	}

	public void PathButtonOnClick()
	{
		Debug.Log((object)"Add path");
	}

	public void MoverButtonOnClick()
	{
		Debug.Log((object)"Add mover");
	}

	public void SpawnPointButtonOnClick()
	{
		Debug.Log((object)"Add Spawn Point");
		MVGameController.Instance.EditorController.AddSpawnPoint();
	}

	public void TriggerBoxButtonOnClick()
	{
		Debug.Log((object)"Add - DEPRECATED - THIS IS EZ-GUI STUFF");
	}

	public void ActionActivateButtonOnClick()
	{
		Debug.Log((object)"Add ActionActivate");
	}

	public void ActionDeactivateButtonOnClick()
	{
		Debug.Log((object)"Add ActionDeactivate");
	}

	public void BlueprintActivatorButtonOnClick()
	{
		Debug.Log((object)"Add BlueprintActivator");
	}

	public void BlueprintFireButtonOnClick()
	{
		Debug.Log((object)"Add BlueprintFire");
	}

	public void BlueprintSmokeButtonOnClick()
	{
		Debug.Log((object)"Add BlueprintSmoke");
	}

	public void BlueprintExplosionButtonOnClick()
	{
		Debug.Log((object)"Add BlueprintExplosion");
	}

	public void ParticleEmitterButtonOnClick()
	{
		Debug.Log((object)"Add ParticleEmitter");
	}

	public void SoundEmitterButtonOnClick()
	{
		Debug.Log((object)"Add SoundEmitter");
		MVGameController.Instance.EditorController.AddSoundEmitter();
	}

	public void PublishButtonOnClick()
	{
		Debug.Log((object)"Publish planet");
		MVGameController.Instance.Game.PublishPlanet(new byte[0]);
	}

	public void ShowMessageBox(string text)
	{
		MVGUIMessageBox.New(text);
	}

	public void ShowReconnectDialog(string text, MVGUIReconnectDialog.OnReconnectDelegate reconnectCallback, MVGUIReconnectDialog.OnDisconnectDelegate disconnectCallback)
	{
		MVGUIReconnectDialog.New(text, reconnectCallback, disconnectCallback);
	}

	public void ShowPurchaseItemBox(string text, int price, int itemID, int prototypeID)
	{
		MVGUIPurchaseItemBox.New(text, price, itemID, prototypeID);
	}

	public void HideHelp()
	{
		if ((Object)(object)helpGO != (Object)null)
		{
			Object.Destroy((Object)(object)helpGO);
		}
	}

	public void FullscreenToggleOnClick()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}
}
