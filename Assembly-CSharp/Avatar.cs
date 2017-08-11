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

	private byte[] modifierEffectCount = new byte[21];

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
	private TeamIconScaleWithDistance teamIcon;

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
			teamIcon.gameObject.SetActive(value: true);
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
		for (int i = 0; i < list.Count; i++)
		{
			AvatarModifierPackageType avatarModifierPackageType = list[i];
			modifiers[avatarModifierPackageType].Deactivate(this);
			modifiers.Remove(avatarModifierPackageType);
			currentModifierByteState.Remove(avatarModifierPackageType);
			AvatarModifierPackage.AvatarModifier[] avatarModifiers = AvatarModifierPackageFactory.GetPackage(avatarModifierPackageType).avatarModifiers;
			for (int j = 0; j < avatarModifiers.Length; j++)
			{
				modifierEffectCount[(int)avatarModifiers[j].avatarModifierEffect]--;
			}
		}
		foreach (KeyValuePair<object, object> newModifier in newModifiers)
		{
			string text2 = (string)newModifier.Key;
			AvatarModifierPackageType avatarModifierPackageType2 = (AvatarModifierPackageType)(int)Enum.Parse(typeof(AvatarModifierPackageType), text2.TrimStart('_'));
			if (!modifiers.ContainsKey(avatarModifierPackageType2))
			{
				AvatarModifier avatarModifier = AvatarModifier.CreateFromType(avatarModifierPackageType2, this);
				if (!(avatarModifier != null))
				{
					continue;
				}
				if (!avatarModifier.EvaluateShouldBeAdded(modifiers))
				{
					UnityEngine.Object.Destroy(avatarModifier.gameObject);
					continue;
				}
				avatarModifier.transform.parent = transform;
				avatarModifier.transform.localPosition = Vector3.zero;
				modifiers.Add(avatarModifierPackageType2, avatarModifier);
				currentModifierByteState.Add(avatarModifierPackageType2, (byte)newModifier.Value);
				avatarModifier.Activate(this);
				AvatarModifierPackage.AvatarModifier[] avatarModifiers2 = AvatarModifierPackageFactory.GetPackage(avatarModifier.ModifierType).avatarModifiers;
				for (int k = 0; k < avatarModifiers2.Length; k++)
				{
					modifierEffectCount[(int)avatarModifiers2[k].avatarModifierEffect]++;
				}
			}
			else if ((byte)newModifier.Value != currentModifierByteState[avatarModifierPackageType2])
			{
				modifiers[avatarModifierPackageType2].ResetTimeStamp();
				currentModifierByteState[avatarModifierPackageType2] = (byte)newModifier.Value;
			}
		}
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

	public bool HasModifierPackage(AvatarModifierPackageType modifierPackageType)
	{
		return modifiers.ContainsKey(modifierPackageType);
	}

	public bool HasModifierEffect(AvatarModifierEffect modifierEffect)
	{
		return modifierEffectCount[(int)modifierEffect] > 0;
	}
}
