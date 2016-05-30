using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OfferAvatarPopup : MonoBehaviour
{
	[SerializeField]
	private Text offerName;

	[SerializeField]
	private Text offerCreator;

	[SerializeField]
	private Text additionalSpins;

	[SerializeField]
	private Text price;

	[SerializeField]
	private RawImage avatarImage;

	[SerializeField]
	private AvatarPreviewer screenShooter;

	[SerializeField]
	private GameObject dropShadow;

	[SerializeField]
	private AvatarOfferCelebration celebratoryPopup;

	private MVWorldObjectClient worldObject;

	private MVBody b;

	private int walkPlays = 1;

	private string prevAnim;

	private readonly List<string> animations = new List<string> { "Walk", "Jump", "Idle" };

	private readonly Dictionary<object, object> animData = new Dictionary<object, object>();

	private float timer;

	public void CreateOffer(ActorOfferAvatar offer, MVWorldObjectClient wo)
	{
		worldObject = wo;
		offerName.text = offer.name;
		additionalSpins.text = offer.extraSpins.ToString();
		price.text = offer.priceGold.ToString();
		offerCreator.text = string.Format(offerCreator.text, offer.creatorName);
		dropShadow = UnityEngine.Object.Instantiate(dropShadow);
		screenShooter = UnityEngine.Object.Instantiate(screenShooter);
		screenShooter.Initialize(1024, 1024, CameraClearFlags.Color, wo.PreviewLayerMask, new Vector3(0f, 0f, 0f), null, new Vector3(0f, 0f, 0f), offer.name, wo, wo.GameObject);
		screenShooter.OverrideCameraForPreviewer(new Vector3(0f, 336.024f, 0f), new Vector3(1.284f, 0.893f, -3.221f));
		dropShadow.transform.position = Vector3.zero;
		dropShadow.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Preview));
		avatarImage.texture = screenShooter.previewCam.targetTexture;
		b = (MVBody)wo;
		prevAnim = "Walk";
		b.Animation.Play(prevAnim);
		Animation component = b.Animation.GetComponent<Animation>();
		foreach (AnimationState item in component)
		{
			item.wrapMode = WrapMode.Loop;
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (walkPlays < 3 && (timer >= b.Animation.GetAnimationTime(prevAnim) || timer >= 1.3f))
		{
			timer = 0f;
			prevAnim = "Walk";
			animData["state"] = prevAnim;
			animData["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			walkPlays++;
			b.Animation.ComputeBlendAnimation(animData);
		}
		else if (walkPlays >= 3 && timer >= b.Animation.GetAnimationTime(prevAnim))
		{
			prevAnim = animations[UnityEngine.Random.Range(1, animations.Count)];
			timer = 0f;
			walkPlays = 0;
			animData["state"] = prevAnim;
			animData["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			b.Animation.ComputeBlendAnimation(animData);
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(dropShadow);
		UnityEngine.Object.Destroy(screenShooter);
		UnityEngine.Object.Destroy(worldObject.GameObject);
		dropShadow = null;
		worldObject = null;
		b = null;
		screenShooter = null;
	}

	public void OnPurchaseOffer()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		OffersManager.ClaimOffer();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		b.Animation.Stop();
	}

	public void OnClose()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (returnCode == 0)
		{
			UnityEngine.Object.Destroy(dropShadow);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary["state"] = "TPose";
			dictionary["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			b.Animation.ComputeBlendAnimation(dictionary);
			AvatarOfferCelebration popup = UnityEngine.Object.Instantiate(celebratoryPopup);
			popup.Initialize(b, avatarImage.mainTexture, screenShooter);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking | UIPushOption.HideAll, OnCelebrationClosed, UIGroupFlags.Popup);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, int.Parse(price.text), 0);
			});
		}
	}

	private void OnCelebrationClosed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
