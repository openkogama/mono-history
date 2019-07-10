using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class DesktopCubeModelingController : MonoBehaviour
{
	[SerializeField]
	private RawImage materialsButtonImage;

	[SerializeField]
	private DesktopCubeModelingToolsController desktopCubeModelingController;

	[SerializeField]
	private Sprite errorSprite;

	[SerializeField]
	private UploadGameScreenshotHandler screenshotHandler;

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine)
	{
		desktopCubeModelingController.Initialize(cubeModelingStateMachine);
	}

	public void SetMaterial(byte materialId)
	{
		materialsButtonImage.texture = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialId).ButtonTexture;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.SetCurrentCubeMaterial(materialId);
	}

	public void PublishGame()
	{
		if (MVGameControllerBase.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Are you sure you wish to publish your game?"), PublishCallback, TM._("Publish Game?"));
			});
		}
		else
		{
			NotificationController.PushNotification(TM._("You must be the owner in order to publish game!"), errorSprite, 3);
		}
	}

	public void PublishCallback(bool confirmed, ConfirmationPopup popup)
	{
		popup.Pop();
		if (!confirmed)
		{
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnPublishedPlanet = (UnityAction<string>)Delegate.Combine(game.OnPublishedPlanet, new UnityAction<string>(OnPublishPlanetFinished));
		string errorText = string.Empty;
		if (!MVGameControllerBase.OperationRequests.PublishPlanet(ref errorText))
		{
			MVNetworkGame game2 = MVGameControllerBase.Game;
			game2.OnPublishedPlanet = (UnityAction<string>)Delegate.Remove(game2.OnPublishedPlanet, new UnityAction<string>(OnPublishPlanetFinished));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(errorText, TM._("Error: "));
			});
		}
	}

	private void OnPublishPlanetFinished(string completionMessage)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnPublishedPlanet = (UnityAction<string>)Delegate.Remove(game.OnPublishedPlanet, new UnityAction<string>(OnPublishPlanetFinished));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		NotificationController.PushNotification(completionMessage);
	}

	public void TakeScreenshot()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.CreateErrorNotificationPopup(TM._("Image upload is disabled in standalone. Reload game using the browser version.\n"));
		});
	}
}
