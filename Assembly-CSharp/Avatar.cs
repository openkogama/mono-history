using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public class Avatar : MonoBehaviour, IBulletImpactVisualizer
{
	public MVAvatar mvAvatar;

	private bool isLocal;

	private Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers = new Dictionary<AvatarModifierPackageType, AvatarModifier>();

	private Dictionary<AvatarModifierPackageType, byte> currentModifierByteState = new Dictionary<AvatarModifierPackageType, byte>();

	private InteractionDataHandlerBase interactionDataHandler;

	private Collider avatarCollider;

	[SerializeField]
	private AvatarBadge avatarBadge;

	[SerializeField]
	private TextMesh avatarName;

	[SerializeField]
	private Renderer healthBarRenderer;

	[SerializeField]
	private Renderer teamIconRenderer;

	[SerializeField]
	private Material teamIconMaterial;

	[SerializeField]
	private Material enemyIconMaterial;

	[SerializeField]
	private AvatarLevelUp avatarLevelUp;

	[SerializeField]
	private Transform nameTagLabel;

	[SerializeField]
	private AvatarFader avatarFader;

	[SerializeField]
	private GameObject mobileIcon;

	[SerializeField]
	public GameObject root;

	[SerializeField]
	private AvatarBulletImpactVisualizer bulletImpactVisualizer;

	private CullingSubscriberBase cullingSubscriberBase;

	private Material avatarNameMaterial;

	private Material avatarHealthMaterial;

	private Material avatarTeamIconMaterial;

	private bool nameTagLabelVisible;

	public bool IsLocal => isLocal;

	public InteractionDataHandlerBase InteractionDataHandlerBase => interactionDataHandler;

	public Collider Collider => avatarCollider;

	public AvatarFader AvatarFader => avatarFader;

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

	public void ShowMobileIcon()
	{
		mobileIcon.SetActive(value: true);
	}

	public void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		this.mvAvatar = mvAvatar;
		avatarFader.BodyTransform = mvAvatar.Body.Transform;
		this.isLocal = isLocal;
		interactionDataHandler = GetComponent<InteractionDataHandlerBase>();
		avatarCollider = GetComponent<Collider>();
		if (isLocal)
		{
			UnityEngine.Object.Destroy(avatarBadge.gameObject);
			teamIconRenderer.gameObject.SetActive(value: false);
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXpProgress));
			avatarName.gameObject.SetActive(value: false);
		}
		else
		{
			avatarBadge.Initialize(mvAvatar.OwnerActorNr);
			cullingSubscriberBase = new CullingSubscriberBase(0.5f, teamIconRenderer.transform.position, OnStateChanged);
			mvAvatar.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(mvAvatar.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		}
		avatarLevelUp.Init(mvAvatar.OwnerActorNr);
		avatarNameMaterial = avatarName.GetComponent<Renderer>().material;
		avatarHealthMaterial = healthBarRenderer.material;
	}

	private void OnStateChanged(CullingGroupEvent cullingEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingEvent, cullingSubscriberBase.DistanceBandIndex);
		teamIconRenderer.gameObject.SetActive(active);
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = teamIconRenderer.transform.position;
	}

	private void OnXpProgress(XPProgressData xpProgressData)
	{
		AvatarPooledXPParticles avatarPooledXPParticles = PrefabPool.Instance.EnumPoolManager.Instantiate<AvatarPooledXPParticles>(PoolEnums.XP);
		avatarPooledXPParticles.transform.parent = transform;
		avatarPooledXPParticles.transform.localPosition = Vector3.up;
		avatarPooledXPParticles.transform.localRotation = Quaternion.identity;
		avatarPooledXPParticles.transform.localScale = Vector3.one;
		avatarPooledXPParticles.Initialize(xpProgressData.XPDelta);
		avatarPooledXPParticles.Play();
	}

	public void UpdateModifiers(Dictionary<object, object> newModifiers)
	{
		List<AvatarModifierPackageType> list = new List<AvatarModifierPackageType>();
		foreach (AvatarModifierPackageType key in modifiers.Keys)
		{
			string text = "_" + key;
			if (!newModifiers.ContainsKey(text))
			{
				AvatarModifierPackageType item = (AvatarModifierPackageType)(int)Enum.Parse(typeof(AvatarModifierPackageType), text.TrimStart('_'));
				list.Add(item);
			}
		}
		foreach (AvatarModifierPackageType item2 in list)
		{
			modifiers[item2].Deactivate(this);
			modifiers.Remove(item2);
			currentModifierByteState.Remove(item2);
		}
		foreach (KeyValuePair<object, object> newModifier in newModifiers)
		{
			string text2 = newModifier.Key as string;
			AvatarModifierPackageType avatarModifierPackageType = (AvatarModifierPackageType)(int)Enum.Parse(typeof(AvatarModifierPackageType), text2.TrimStart('_'));
			if (!modifiers.ContainsKey(avatarModifierPackageType))
			{
				AvatarModifier avatarModifier = AvatarModifier.CreateFromType(avatarModifierPackageType, this);
				if (avatarModifier != null)
				{
					if (!avatarModifier.EvaluateShouldBeAdded(modifiers))
					{
						UnityEngine.Object.Destroy(avatarModifier.gameObject);
						continue;
					}
					avatarModifier.transform.parent = transform;
					avatarModifier.transform.localPosition = Vector3.zero;
					modifiers.Add(avatarModifierPackageType, avatarModifier);
					currentModifierByteState.Add(avatarModifierPackageType, (byte)newModifier.Value);
					avatarModifier.Activate(this);
				}
			}
			else if ((byte)newModifier.Value != currentModifierByteState[avatarModifierPackageType])
			{
				modifiers[avatarModifierPackageType].ResetTimeStamp();
				currentModifierByteState[avatarModifierPackageType] = (byte)newModifier.Value;
			}
		}
	}

	public void UpdateNameTag()
	{
		avatarName.text = MVGameControllerBase.Game.Players[mvAvatar.OwnerActorNr].Username;
		Color color = Color.white;
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[mvAvatar.OwnerActorNr];
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			switch (mVPlayer.Team)
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
			SetHealthBarColor(MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mvAvatar.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr));
		}
		avatarNameMaterial.color = color;
	}

	public void SetHealthBarColor(bool isFriendly)
	{
		if (isFriendly)
		{
			avatarHealthMaterial.color = Color.green;
			teamIconRenderer.material = teamIconMaterial;
		}
		else
		{
			avatarHealthMaterial.color = Color.red;
			teamIconRenderer.material = enemyIconMaterial;
		}
	}

	public void StartBlinking(BlinkType type, float duration = float.PositiveInfinity)
	{
		mvAvatar.Body.StartBlinking(type, duration);
	}

	public void StopBlinking(BlinkType type)
	{
		mvAvatar.Body.StopBlinking(type);
	}

	private void OnDestroy()
	{
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		UnityEngine.Object.Destroy(avatarNameMaterial);
		UnityEngine.Object.Destroy(avatarHealthMaterial);
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		bulletImpactVisualizer.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
	}
}
