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
	private AudioSource screenShotSound;

	[SerializeField]
	private Sprite errorSprite;

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine)
	{
		desktopCubeModelingController.Initialize(cubeModelingStateMachine);
	}

	public void SetMaterial(byte materialId)
	{
		materialsButtonImage.texture = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialId).buttonTexture;
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.CurrentCubeMaterial = materialId;
	}

	public void PublishGame()
	{
		if (MVGameControllerBase.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create("Are you sure you wish to publish your game?", PublishCallback, "Publish Game?");
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
				x.Create(errorText, "Error: ");
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
		if (MVGameControllerBase.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			MVGameControllerBase.Game.ScreenshotUploaded += OnScreenShotUploaded;
			MVGameControllerBase.OperationRequests.UploadGameScreenShot();
		}
		else
		{
			NotificationController.PushNotification(TM._("You must be the owner in order to take a screenshot!"), errorSprite, 3);
		}
	}

	private void OnScreenShotUploaded(object sender, ScreenshotUploadedEventArgs args)
	{
		MVGameControllerBase.Game.ScreenshotUploaded -= OnScreenShotUploaded;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		screenShotSound.Play();
		string text = TM._("Screenshot Successfully uploaded");
		if (!args.Uploaded)
		{
			text = TM._("Failed to upload screenshot");
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(text, TM._("Screenshot upload"));
		});
	}
}
