using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarEditModeBodyController : MonoBehaviour, IEventSystemHandler, IAvatarEditAnimationState
{
	private List<MVBody> bodies = new List<MVBody>();

	private MVSpawnPointRed bodySpawnPoint;

	private Vector3 displayPos;

	private Quaternion displayRotation;

	private Vector3 hidePos;

	private int currentBodyIndex;

	private AvatarRepositoryItem purchasingItem;

	private string currentActionSuccessMessage;

	private bool playingPurchaseSoundAfterScreenshot;

	[SerializeField]
	public AvatarPictureTakerUGUI pictureTaker;

	[SerializeField]
	private NotificationPopup notificationPopup;

	[SerializeField]
	private PleaseWaitPopup pleaseWaitPopupPrefab;

	[SerializeField]
	private UploadAvatarScreenshotHandler uploadAvatarScreenshotHandler;

	[SerializeField]
	private ResetAvatarHandler resetAvatarHandler;

	private List<string> animations = new List<string> { "Idle", "Jump", "Dead", "Swim", "Walk" };

	private int currentAnimationIndex;

	public Action<int, Texture2D> Picture2DTakenCallback;

	private GameObject publishAvatarBtn;

	public MVBody CurrentBody => bodies[currentBodyIndex];

	public Vector3 DisplayPos => displayPos;

	public void Initialize()
	{
		pictureTaker = UnityEngine.Object.Instantiate(pictureTaker);
		bodySpawnPoint = (MVSpawnPointRed)MVGameControllerBase.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointRed);
		displayPos = bodySpawnPoint.WorldPosition - Vector3.up;
		hidePos = bodySpawnPoint.WorldPosition - 51f * Vector3.up;
		displayRotation = bodySpawnPoint.WorldRotation;
		bodySpawnPoint.GameObject.SetActive(value: false);
		IEnumerable<MVWorldObjectClient> enumerable = from s in MVGameControllerBase.WOCM.GetWorldObjectClientsWhere((MVWorldObjectClient wo) => wo is MVBody mVBody2 && mVBody2.AttachedAvatar == null)
			orderby s.Id
			select s;
		foreach (MVWorldObjectClient item in enumerable)
		{
			MVBody mVBody = item as MVBody;
			mVBody.ShadowVisible = false;
			mVBody.Visible = false;
			Animation component = mVBody.Animation.GetComponent<Animation>();
			foreach (AnimationState item2 in component)
			{
				item2.wrapMode = WrapMode.Loop;
			}
			bodies.Add(mVBody);
		}
		CurrentBody.WorldPosition = displayPos;
		CurrentBody.WorldRotation = displayRotation;
		CurrentBody.Visible = true;
		foreach (MVBody item3 in bodies.Skip(1))
		{
			item3.WorldPosition = hidePos;
			item3.WorldRotation = bodySpawnPoint.WorldRotation;
		}
	}

	public void SetPublishAvatarGO(GameObject publishAvatarGO)
	{
		publishAvatarBtn = publishAvatarGO;
		SetPublishAvatarButtonActive();
	}

	public void ResetCurrentBody()
	{
		ResetAvatarHandler resetHandler = UnityEngine.Object.Instantiate(resetAvatarHandler);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(resetHandler.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		resetHandler.ResetAvatar(CurrentBody, ExecuteReset);
	}

	private void ExecuteReset()
	{
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(ResetCallback));
		MVGameControllerBase.OperationRequests.ResetAvatar(CurrentBody.Id);
	}

	private void ResetCallback(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(ResetCallback));
		int id = e.RootWO.Id;
		MVBody mVBody = (MVBody)MVGameControllerBase.WOCM.GetWorldObjectClient(id);
		MVGameControllerBase.Game.AvatarMetaDataWoMap.ResetAvatar(CurrentBody.Id, id);
		bodies[currentBodyIndex] = mVBody;
		SetCurrentBody(currentBodyIndex);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSetBodyGroup x, BaseEventData y) =>
		{
			x.SetBodyGroup(CurrentBody);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CERoamUUI);
		});
		MVGameControllerBase.OperationRequests.SetActiveAvatar(id);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		mVBody.ShadowVisible = false;
		Animation component = mVBody.Animation.GetComponent<Animation>();
		foreach (AnimationState item in component)
		{
			item.wrapMode = WrapMode.Loop;
		}
	}

	public void SetCurrentBodyByWoId(int woId)
	{
		for (int i = 0; i < bodies.Count; i++)
		{
			if (bodies[i].Id == woId)
			{
				SetCurrentBody(i);
				AvatarSelectionController.CurrentlySelectedSlotIndex = i;
				break;
			}
		}
	}

	public void SetCurrentBody(int index)
	{
		CurrentBody.ShadowVisible = false;
		CurrentBody.Visible = false;
		CurrentBody.WorldPosition = hidePos;
		CurrentBody.WorldRotation = bodySpawnPoint.WorldRotation;
		currentBodyIndex = index;
		CurrentBody.WorldPosition = displayPos;
		CurrentBody.WorldRotation = displayRotation;
		CurrentBody.Visible = true;
		SharedCubeFunctions.SetLayerRecursively(CurrentBody.Transform, select: true);
		SetPublishAvatarButtonActive();
	}

	private void SetPublishAvatarButtonActive()
	{
		if (!(publishAvatarBtn == null))
		{
			MVGameControllerBase.Game.AvatarMetaDataWoMap.TryGetValue(CurrentBody.Id, out var avatarMetaData);
			if (avatarMetaData != null)
			{
				publishAvatarBtn.SetActive(avatarMetaData.canBeSoldOnMarketPlace);
			}
		}
	}

	public void CaptureScreenshotForBody(int index, Action<int, Texture2D> OnPictureTaken)
	{
		pictureTaker.TakePicture(bodies[index], index, OnPictureTaken, isCurrentBody: false);
	}

	public void CaptureScreenshotsForAllAvatars(Action<int, Texture2D> OnPictureTaken)
	{
		Picture2DTakenCallback = OnPictureTaken;
		for (int i = 0; i < bodies.Count; i++)
		{
			GenerateIconForBody(i);
		}
	}

	private void GenerateIconForBody(int index)
	{
		pictureTaker.TakePicture(bodies[index], index, Picture2DTakenCallback, bodies[index] == CurrentBody);
	}

	public void TakeScreenshot()
	{
		UploadAvatarScreenshotHandler screenshotHandler = UnityEngine.Object.Instantiate(uploadAvatarScreenshotHandler);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(screenshotHandler.gameObject, UIPushOption.Blocking | UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
		screenshotHandler.TakeScreenshot(CurrentBody, ScreenShotCallback);
	}

	public void PurchaseAvatar(AvatarRepositoryItem item)
	{
		purchasingItem = item;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Purchase Avatar?"), OnPurchaseAvatarConfirmation, TM._("Confirm"));
		});
	}

	private void OnPurchaseAvatarConfirmation(bool confirmed, ConfirmationPopup confirmationPopup)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (confirmed)
		{
			PleaseWaitPopup popup = UnityEngine.Object.Instantiate(pleaseWaitPopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnProductPurchaseAvatarResponse));
			World world = MVGameControllerBase.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
			MVGameControllerBase.OperationRequests.PurchaseAvatar(purchasingItem.itemID);
		}
	}

	private void OnProductPurchaseAvatarResponse(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnProductPurchaseAvatarResponse));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopToGroup(UIGroupFlags.MainUI);
		});
		Debug.Log("Avatar purchase response: " + (MVPurchaseReturnCode)returnCode);
		if (returnCode != 0)
		{
			World world = MVGameControllerBase.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, purchasingItem.priceGold, 0);
			});
		}
	}

	private void InitializedPurchasedAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		if (e.RootWO == null)
		{
			return;
		}
		Debug.Log("Purchased avatar has been added to world. WorldObjectId is: " + e.RootWO);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(e.RootWO.Id);
		MVBody mVBody = worldObjectClient as MVBody;
		mVBody.ShadowVisible = false;
		mVBody.Visible = false;
		Animation component = mVBody.Animation.GetComponent<Animation>();
		foreach (AnimationState item in component)
		{
			item.wrapMode = WrapMode.Loop;
		}
		bodies.Add(mVBody);
		int num = bodies.Count - 1;
		GenerateIconForBody(num);
		SetCurrentBody(num);
		AvatarSelectionController.CurrentlySelectedSlotIndex = num;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnActiveAvatarSet = (Action)Delegate.Combine(game.OnActiveAvatarSet, new Action(OnActiveAvatarSetAfterPurchase));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSetBodyGroup x, BaseEventData y) =>
		{
			x.SetBodyGroup(CurrentBody);
		});
		MVGameControllerBase.OperationRequests.SetActiveAvatar(e.RootWO.Id);
	}

	private void OnActiveAvatarSetAfterPurchase()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnActiveAvatarSet = (Action)Delegate.Remove(game.OnActiveAvatarSet, new Action(OnActiveAvatarSetAfterPurchase));
		UploadAvatarScreenshotHandler uploadAvatarScreenshotHandler = UnityEngine.Object.Instantiate(this.uploadAvatarScreenshotHandler);
		uploadAvatarScreenshotHandler.TakePurchasedScreenshot(CurrentBody, ScreenShotCallback);
	}

	public void SellCurrentAvatar(SellAvatarController avatarSeller)
	{
		avatarSeller.Initialize(CurrentBody.Id, CurrentBody);
	}

	private void ScreenShotCallback(Texture2D screenshotTex, string successMessage)
	{
		currentActionSuccessMessage = successMessage;
		DataUploadManager.UploadData(screenshotTex.EncodeToPNG(), UploadedImageData);
	}

	private void UploadedImageData()
	{
		MVGameControllerBase.Game.ScreenshotUploaded += MVNetworGame_ScreenshotUploadedHandler;
		MVGameControllerBase.OperationRequests.UploadScreenshot(ImageType.Avatar);
	}

	private void MVNetworGame_ScreenshotUploadedHandler(object sender, ScreenshotUploadedEventArgs e)
	{
		MVGameControllerBase.Game.ScreenshotUploaded -= MVNetworGame_ScreenshotUploadedHandler;
		NotificationPopup popup = UnityEngine.Object.Instantiate(notificationPopup);
		if (e.Uploaded)
		{
			if (playingPurchaseSoundAfterScreenshot)
			{
				playingPurchaseSoundAfterScreenshot = false;
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPurchaseSoundManager x, BaseEventData y) =>
				{
					x.PlayPurchaseSound();
				});
			}
			popup.Initialize(currentActionSuccessMessage, "Success!");
		}
		else
		{
			popup.Initialize("There was a server communication problem. Try again!", "Action failed!");
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopToGroup(UIGroupFlags.MainUI);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CERoamUUI);
		});
	}

	public void SetToNextAnimation()
	{
		currentAnimationIndex = ((currentAnimationIndex < animations.Count - 1) ? (currentAnimationIndex + 1) : 0);
		Set(animations[currentAnimationIndex]);
	}

	public void Set(string animation)
	{
		CurrentBody.Animation.Play(animation);
		ActivateOnAnimationBase[] componentsInChildren = CurrentBody.GameObject.GetComponentsInChildren<ActivateOnAnimationBase>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] is AccessoryAnimationHandler)
			{
				((AccessoryAnimationHandler)componentsInChildren[i]).SetAllAnimationToLooping();
			}
			componentsInChildren[i].OnAvatarAnimationChange(animation);
		}
	}
}
