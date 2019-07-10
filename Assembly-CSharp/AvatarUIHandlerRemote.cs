using System;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.Subscription;
using UnityEngine;

public class AvatarUIHandlerRemote : AvatarUIHandler
{
	private Material avatarNameMaterial;

	private Material avatarHealthMaterial;

	private Material avatarShieldMaterial;

	private Material avatarTeamIconMaterial;

	private CullingSubscriberBase cullingSubscriberBase;

	[SerializeField]
	private AvatarBadge avatarBadge;

	[SerializeField]
	private TextMesh avatarName;

	[SerializeField]
	private Renderer healthBarRenderer;

	[SerializeField]
	private Renderer shieldBarRenderer;

	[SerializeField]
	private Renderer teamIconRenderer;

	[SerializeField]
	private HealthBar healthBar;

	[SerializeField]
	private ShieldBar shieldBar;

	[SerializeField]
	private Material teamIconMaterial;

	[SerializeField]
	private Material enemyIconMaterial;

	[SerializeField]
	private TeamIconScaleWithDistance teamIcon;

	[SerializeField]
	private Transform nameTagLabel;

	[SerializeField]
	private MeshRenderer mobileIcon;

	[SerializeField]
	private Texture androidTexture;

	[SerializeField]
	private Texture iOSTexture;

	[SerializeField]
	private SayChatBubbleHandler sayChatBubbleHandler;

	[SerializeField]
	private GameObject memberFrame;

	private bool shouldShowMobileIcon;

	private bool nameTagLabelVisible;

	public bool NameTagLabelVisible
	{
		get
		{
			return nameTagLabelVisible;
		}
		set
		{
			nameTagLabelVisible = value;
			Renderer[] componentsInChildren = nameTagLabel.GetComponentsInChildren<Renderer>(includeInactive: true);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = nameTagLabelVisible;
			}
		}
	}

	public HealthBar HealthBar => healthBar;

	public ShieldBar ShieldBar => shieldBar;

	public SayChatBubbleHandler SayChatBubbleHandler => sayChatBubbleHandler;

	public void ShowMobileIcon(BuildTarget bT)
	{
		mobileIcon.gameObject.SetActive(value: true);
		switch (bT)
		{
		case BuildTarget.Android:
			mobileIcon.material.mainTexture = androidTexture;
			break;
		case BuildTarget.IOS:
			mobileIcon.material.mainTexture = iOSTexture;
			break;
		}
		shouldShowMobileIcon = true;
	}

	public override void Initialize(bool isLocal, MVWorldObjectClient wo, int ownerActorNr, ChatAnchor chatBubbleAnchor)
	{
		base.Initialize(isLocal, wo, ownerActorNr, chatBubbleAnchor);
		avatarBadge.Initialize(ownerActorNr);
		cullingSubscriberBase = new CullingSubscriberBase(0.5f, teamIconRenderer.transform.position, OnStateChanged);
		teamIcon.gameObject.SetActive(value: true);
		avatarNameMaterial = avatarName.GetComponent<Renderer>().material;
		avatarHealthMaterial = healthBarRenderer.material;
		avatarShieldMaterial = shieldBarRenderer.material;
		sayChatBubbleHandler.Initialize(ownerActorNr, chatBubbleAnchor);
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(ownerActorNr);
		if (playerUnsafe != null)
		{
			memberFrame.SetActive(playerUnsafe.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost));
		}
		UpdateNameTag();
		UpdateHealthBarColor();
		ChatCommandManager.UpdateChatCommandCallback(ChatCommand.HideAllUI, (Action)Delegate.Combine(ChatCommandManager.GetChatCommandCallback(ChatCommand.HideAllUI), new Action(HideUI)));
	}

	public override void SetShouldShowUI(bool shouldShow)
	{
		shouldShowUI = shouldShow;
		base.SetShouldShowUI(shouldShow);
		if (avatarBadge != null)
		{
			avatarBadge.gameObject.SetActive(shouldShow);
		}
		healthBar.gameObject.SetActive(shouldShow);
		shieldBar.gameObject.SetActive(shouldShow);
		if (shouldShowMobileIcon)
		{
			mobileIcon.gameObject.SetActive(shouldShow);
		}
		NameTagLabelVisible = shouldShow;
		teamIcon.gameObject.SetActive(shouldShow);
	}

	public override void Activate()
	{
		base.Activate();
		sayChatBubbleHandler.Activate();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		sayChatBubbleHandler.Deactivate();
	}

	public override void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = teamIconRenderer.transform.position;
		base.OnPositionChanged(arg0, positionChangedEventArgs);
	}

	public override void HandleTeamChange()
	{
		UpdateNameTag();
		UpdateHealthBarColor();
		base.HandleTeamChange();
	}

	public void UpdateHealthBarColor()
	{
		if (IsOnSameTeamAsLocalAvatar())
		{
			SetHealthBarColor(isFriendly: true);
		}
		else
		{
			SetHealthBarColor(isFriendly: false);
		}
	}

	public void UpdateNameTag()
	{
		if (!MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(ownerActorNr))
		{
			return;
		}
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(ownerActorNr);
		avatarName.text = playerUnsafe.UserProfileData.UserName;
		Color color = Color.white;
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			switch (playerUnsafe.Team)
			{
			case MVTeam.Blue:
				color = Color.blue;
				break;
			case MVTeam.Red:
				color = Color.red;
				break;
			case MVTeam.Green:
				color = Color.green;
				break;
			case MVTeam.Yellow:
				color = Color.yellow;
				break;
			}
		}
		avatarNameMaterial.color = color;
	}

	public void SetHealthBarColor(bool isFriendly)
	{
		avatarShieldMaterial.color = new Color(25f / 255f, 25f / 255f, 112f / 255f);
		if (isFriendly)
		{
			avatarHealthMaterial.color = Color.green;
			teamIconRenderer.material = teamIconMaterial;
			return;
		}
		Color red = Color.red;
		red.r = 1f;
		red.g = 99f / 255f;
		red.b = 71f / 255f;
		avatarHealthMaterial.color = red;
		enemyIconMaterial.color = red;
		teamIconRenderer.material = enemyIconMaterial;
	}

	private void OnStateChanged(CullingGroupEvent cullingEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingEvent, cullingSubscriberBase.DistanceBandIndex);
		teamIconRenderer.gameObject.SetActive(active);
	}

	private bool IsOnSameTeamAsLocalAvatar()
	{
		if (MVGameControllerBase.Game.LocalPlayer.IsOnSameTeam(worldObject))
		{
			return true;
		}
		return false;
	}

	private void HideUI()
	{
		SetShouldShowUI(shouldShow: false);
	}

	protected override void OnDestroy()
	{
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		UnityEngine.Object.Destroy(avatarNameMaterial);
		UnityEngine.Object.Destroy(avatarHealthMaterial);
		ChatCommandManager.UpdateChatCommandCallback(ChatCommand.HideAllUI, (Action)Delegate.Remove(ChatCommandManager.GetChatCommandCallback(ChatCommand.HideAllUI), new Action(HideUI)));
		base.OnDestroy();
	}
}
