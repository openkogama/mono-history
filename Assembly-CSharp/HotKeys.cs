using MV.Common;
using UnityEngine;

internal class HotKeys
{
	private DevHotKeys devHotkeys = new DevHotKeys();

	private AEditController EditController => MVGameController.Instance.EditController;

	private bool AllowHotkeys => (Object)(object)UXUtils.FindGUIObjectOfType<UXFocusManager>().CurrentFocus == (Object)null && !UXUtils.FindGUIObjectOfType<UXDialogFactory>().DialogOpen;

	public void HandleInput()
	{
		devHotkeys.HandleInput();
		if (AllowHotkeys && !Input.GetKey((KeyCode)48))
		{
			HandleSharedHotKeys();
			HandlePlayModeHotKeys();
			HandleEditModeHotKeys();
			HandleCharacterEditorModeHotKeys();
		}
	}

	private void HandleSharedHotKeys()
	{
		if (MVInputWrapper.GetKeyUp((KeyCode)111))
		{
			MVGameController.Instance.IngameController.ToggleFullScreen();
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)116) || MVInputWrapper.GetKeyUp((KeyCode)13))
		{
			if (!MVGameController.Instance.Game.TouristChatAllowed && MVGameController.Instance.IsTouristSession)
			{
				MVGameController.Instance.IngameController.ShowRegisterPopup();
			}
			else
			{
				MVGameController.Instance.IngameController.ShowChat(fromShortcut: true);
			}
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)107))
		{
			MVGameController.Instance.IngameController.RespawnAvatar();
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)104))
		{
			MVBody body = MVGameController.Instance.WOCM.AvatarLocal.Body;
			body.AccessoryParticlesVisible = !body.AccessoryParticlesVisible;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)9) || MVInputWrapper.GetKeyUp((KeyCode)109))
		{
			MVGameController.Instance.IngameController.ShowPlayersWindow(show: false);
		}
		else if (MVInputWrapper.GetKeyDown((KeyCode)9) || MVInputWrapper.GetKeyDown((KeyCode)109))
		{
			MVGameController.Instance.IngameController.ShowPlayersWindow(show: true);
		}
	}

	private void HandlePlayModeHotKeys()
	{
		bool flag = MVGameController.Instance.EditorController != null && MVGameController.Instance.EditorController.PlayInEditor;
		if (MVGameController.Instance.Game.GameMode != MVGameMode.Play && !flag)
		{
			return;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)113))
		{
			MVEquipable component = MVGameController.Instance.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
			if ((Object)(object)component != (Object)null)
			{
				component.Equip(AvatarItemType.Hand, null);
			}
		}
		if (!MVInputWrapper.GetKeyDown((KeyCode)101))
		{
		}
	}

	private void HandleEditModeHotKeys()
	{
		if (MVGameController.Instance.Game.GameMode != MVGameMode.Edit)
		{
			return;
		}
		HandleEditControllerHotKeys();
		if (MVInputWrapper.GetKeyDown((KeyCode)118))
		{
			MVWorldObjectClient singleSelectedWO = MVGameController.Instance.EditController.EditorStateMachine.SingleSelectedWO;
			if (singleSelectedWO != null)
			{
				MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(singleSelectedWO);
			}
		}
	}

	private void HandleCharacterEditorModeHotKeys()
	{
		if (MVGameController.Instance.Game.GameMode == MVGameMode.CharacterEditor)
		{
			HandleEditControllerHotKeys();
		}
	}

	private void HandleEditControllerHotKeys()
	{
		if (EditController == null)
		{
			return;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)112) && MVGameController.Instance.Game.GameMode != MVGameMode.CharacterEditor)
		{
			EditController.TogglePlayInEditor();
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)108))
		{
			EditController.ToggleLogicRendering();
		}
		if (!EditController.PlayInEditor)
		{
			if (MVInputWrapper.GetKeyDown((KeyCode)103))
			{
				EditController.ToggleGridSnap();
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)102))
			{
				EditController.ToggleDrawPlane();
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)49))
			{
				EditController.CubeTools.editCubeButton.Toggle();
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)50))
			{
				EditController.CubeTools.deleteCubeButton.Toggle();
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)51))
			{
				EditController.CubeTools.paintCubeButton.Toggle();
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)114))
			{
				EditController.ShowMaterialChangeWindow();
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)105))
			{
				EditController.ShowInventory();
			}
			if (MVInputWrapper.GetKeyUp((KeyCode)110))
			{
				EditController.ShowNewModelWindow();
			}
		}
	}
}
