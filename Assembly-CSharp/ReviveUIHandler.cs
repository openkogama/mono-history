using System;
using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ReviveUIHandler : ReviveUIHandlerBase
{
	private const float minDistanceToCameraBeforeShowAvatar = 3f;

	private bool roundEndedWhileWatchingAd;

	private int currentSafePointSelected;

	private GameObject bodyClone;

	public override void Initialize(UnityAction onContinueClicked)
	{
		base.Initialize(onContinueClicked);
		ReviveState value = MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value;
		currentSafePointSelected = value.GetSafeGroundedPositions().Count - 1;
		value.SetSafeGroundedDataIndex(currentSafePointSelected);
		if ((value.SafeGroundedData.Position - value.SafeGroundedData.CameraPosition).sqrMagnitude > 3f)
		{
			CreateAvatarBodyForScreenshot();
		}
		GameObject gameObject = new GameObject("GenerateTexture");
		ReviveScreenshotGenerator reviveScreenshotGenerator = gameObject.AddComponent<ReviveScreenshotGenerator>();
		reviveScreenshotGenerator.GenerateTextureDataCameraViewAtTransform(OnGenerateTextureComplete, value.SafeGroundedData.CameraPosition, value.SafeGroundedData.CameraRotation, (int)targetTexture.rectTransform.rect.width, (int)targetTexture.rectTransform.rect.height);
	}

	protected override void RoundEnded(IWinningCondition condition)
	{
		roundEndedWhileWatchingAd = true;
	}

	private void OnGenerateTextureComplete(byte[] generatedTexture)
	{
		Texture2D texture2D = new Texture2D((int)targetTexture.rectTransform.rect.width, (int)targetTexture.rectTransform.rect.height, TextureFormat.ARGB32, mipChain: false);
		texture2D.LoadImage(generatedTexture);
		targetTexture.texture = texture2D;
		CleanupAvatarBodyAfterScreenshot();
	}

	protected override void Update()
	{
		bool isBlocked = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			isBlocked = x.IsUIElementBlocked(gameObject);
		});
		if (!isBlocked)
		{
			if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded || roundEndedWhileWatchingAd)
			{
				OnRewardedAdWatched(RewardedAdResult.ErrorTimeout);
			}
			else
			{
				base.Update();
			}
		}
	}

	public override void OnWatchAdClicked()
	{
		base.OnWatchAdClicked();
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.MoveBodyToSafeSpot(currentSafePointSelected);
	}

	protected override void OnAdFinishedContinue()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SpawnAtSafeSpot(currentSafePointSelected);
	}

	protected override void OnRewardedAdWatched(RewardedAdResult result)
	{
		Debug.Log("RESULT OF REVIVE: " + result);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(RoundEnded));
		if (roundEndedWhileWatchingAd)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Game ended while you were busy. Restarting from spawn."), string.Empty);
			});
			roundEndedWhileWatchingAd = false;
			return;
		}
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			OnAdFinishedContinue();
			break;
		case RewardedAdResult.ErrorTimeout:
			continueButton.onClick.Invoke();
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
		case RewardedAdResult.RewardNotUnlocked:
		{
			NotificationPopup popup = UnityEngine.Object.Instantiate(errorNotification);
			popup.Initialize(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, () =>
				{
					continueButton.onClick.Invoke();
				}, UIGroupFlags.Popup);
			});
			break;
		}
		}
	}

	private void CreateAvatarBodyForScreenshot()
	{
		bodyClone = ((MVAvatarLocal)MVGameControllerBase.WOCM.GetWorldObjectClient(MVGameControllerBase.SpawnRoleDataMediatorLocal.WoId)).Body.CreateClone();
		MonoBehaviour[] componentsInChildren = bodyClone.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] array = componentsInChildren;
		foreach (MonoBehaviour monoBehaviour in array)
		{
			monoBehaviour.enabled = false;
		}
		AvatarBlobShadowController[] componentsInChildren2 = bodyClone.GetComponentsInChildren<AvatarBlobShadowController>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].enabled = true;
		}
		PickupItem[] componentsInChildren3 = bodyClone.GetComponentsInChildren<PickupItem>();
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			componentsInChildren3[k].gameObject.SetActive(value: false);
		}
		AvatarModifier[] componentsInChildren4 = bodyClone.GetComponentsInChildren<AvatarModifier>();
		for (int l = 0; l < componentsInChildren4.Length; l++)
		{
			componentsInChildren4[l].gameObject.SetActive(value: false);
		}
		AnimatedSpriteSheetTexture[] componentsInChildren5 = bodyClone.GetComponentsInChildren<AnimatedSpriteSheetTexture>(includeInactive: true);
		for (int m = 0; m < componentsInChildren5.Length; m++)
		{
			componentsInChildren5[m].enabled = true;
		}
		AnimatedTextureOffset[] componentsInChildren6 = bodyClone.GetComponentsInChildren<AnimatedTextureOffset>(includeInactive: true);
		for (int n = 0; n < componentsInChildren6.Length; n++)
		{
			componentsInChildren6[n].enabled = true;
		}
		SelectionBox[] componentsInChildren7 = bodyClone.GetComponentsInChildren<SelectionBox>();
		for (int num = 0; num < componentsInChildren7.Length; num++)
		{
			UnityEngine.Object.Destroy(componentsInChildren7[num].gameObject);
		}
		InvulnerabilityBubble componentInChildren = bodyClone.GetComponentInChildren<InvulnerabilityBubble>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(value: false);
		}
		SkinnedMeshOptimizer[] componentsInChildren8 = bodyClone.GetComponentsInChildren<SkinnedMeshOptimizer>();
		for (int num2 = 0; num2 < componentsInChildren8.Length; num2++)
		{
			if (componentsInChildren8[num2] != null)
			{
				componentsInChildren8[num2].DisableOptimizer();
				componentsInChildren8[num2].TurnOffMesh();
				UnityEngine.Object.Destroy(componentsInChildren8[num2]);
			}
		}
		MeshRenderer[] componentsInChildren9 = bodyClone.GetComponentsInChildren<MeshRenderer>();
		for (int num3 = 0; num3 < componentsInChildren9.Length; num3++)
		{
			for (int num4 = 0; num4 < componentsInChildren9[num3].materials.Length; num4++)
			{
				if (componentsInChildren9[num3].materials[num4].HasProperty("_Color"))
				{
					Color color = componentsInChildren9[num3].materials[num4].color;
					color.a = 1f;
					componentsInChildren9[num3].materials[num4].color = color;
				}
			}
		}
		Animation componentInChildren2 = bodyClone.GetComponentInChildren<Animation>();
		componentInChildren2.Play("Idle");
		ActivateOnAnimationBase[] componentsInChildren10 = componentInChildren2.GetComponentsInChildren<ActivateOnAnimationBase>();
		for (int num5 = 0; num5 < componentsInChildren10.Length; num5++)
		{
			componentsInChildren10[num5].OnAvatarAnimationChange("Idle");
		}
		SafeSpotData safeGroundedDataAtSelectedIndex = MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value.GetSafeGroundedDataAtSelectedIndex();
		Vector3 eulerAngles = safeGroundedDataAtSelectedIndex.Rotation.eulerAngles;
		Vector3 eulerAngles2 = safeGroundedDataAtSelectedIndex.CameraRotation.eulerAngles;
		Vector3 euler = new Vector3(eulerAngles.x, eulerAngles2.y, eulerAngles.z);
		bodyClone.transform.position = MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value.SafeGroundedData.Position;
		bodyClone.transform.rotation = Quaternion.Euler(euler);
		bodyClone.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Default));
	}

	private void CleanupAvatarBodyAfterScreenshot()
	{
		UnityEngine.Object.Destroy(bodyClone);
	}
}
