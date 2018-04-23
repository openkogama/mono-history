using MV.WorldObject;
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
	private GameObject mobileIcon;

	[SerializeField]
	private SayChatBubbleHandler sayChatBubbleHandler;

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

	public void ShowMobileIcon()
	{
		mobileIcon.SetActive(value: true);
		shouldShowMobileIcon = true;
	}

	public override void Initialize(bool isLocal, MVAvatar mvAvatar)
	{
		base.Initialize(isLocal, mvAvatar);
		avatarBadge.Initialize(mvAvatar.OwnerActorNr);
		cullingSubscriberBase = new CullingSubscriberBase(0.5f, teamIconRenderer.transform.position, OnStateChanged);
		teamIcon.gameObject.SetActive(value: true);
		avatarNameMaterial = avatarName.GetComponent<Renderer>().material;
		avatarHealthMaterial = healthBarRenderer.material;
		avatarShieldMaterial = shieldBarRenderer.material;
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
			mobileIcon.SetActive(shouldShow);
		}
		teamIcon.gameObject.SetActive(shouldShow);
	}

	public override void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = teamIconRenderer.transform.position;
		base.OnPositionChanged(arg0, positionChangedEventArgs);
	}

	public override void HandleTeamChange()
	{
		if (IsOnSameTeamAsLocalAvatar())
		{
			SetHealthBarColor(isFriendly: true);
		}
		else
		{
			SetHealthBarColor(isFriendly: false);
		}
		base.HandleTeamChange();
	}

	public void UpdateNameTag()
	{
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(mvAvatar.OwnerActorNr);
		avatarName.text = playerUnsafe.Username;
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
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			SetHealthBarColor(MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mvAvatar, MVGameControllerBase.Game.LocalPlayer.Avatar));
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
		if (MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mvAvatar, MVGameControllerBase.Game.LocalPlayer.Avatar))
		{
			return true;
		}
		return false;
	}

	protected override void OnDestroy()
	{
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		Object.Destroy(avatarNameMaterial);
		Object.Destroy(avatarHealthMaterial);
		base.OnDestroy();
	}
}
