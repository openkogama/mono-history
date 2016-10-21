using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class Avatar : MonoBehaviour
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
	private AvatarLevelUp avatarLevelUp;

	[SerializeField]
	private Transform nameTagLabel;

	[SerializeField]
	private AvatarFader avatarFader;

	[SerializeField]
	private GameObject mobileIcon;

	[SerializeField]
	public GameObject root;

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
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXpProgress));
		}
		else
		{
			avatarBadge.Initialize(mvAvatar.OwnerActorNr);
		}
		avatarLevelUp.Init(mvAvatar.OwnerActorNr);
	}

	private void OnXpProgress(XPProgressData xpProgressData)
	{
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(PrefabPool.Instance.ParticleXP);
		particleSystem.transform.parent = transform;
		particleSystem.transform.localPosition = Vector3.up;
		particleSystem.transform.localRotation = Quaternion.identity;
		particleSystem.transform.localScale = Vector3.one;
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		ParticleSystem.MinMaxCurve rate = emission.rate;
		rate.constantMax = xpProgressData.XPDelta;
		emission.rate = rate;
		particleSystem.Play();
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
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			switch (MVGameControllerBase.Game.Players[mvAvatar.OwnerActorNr].Team)
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
		avatarName.GetComponent<Renderer>().material.color = color;
	}

	public void StartBlinking(BlinkType type, float duration)
	{
		mvAvatar.Body.StartBlinking(type, duration);
	}

	public void StopBlinking(BlinkType type)
	{
		mvAvatar.Body.StopBlinking(type);
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(avatarName.GetComponent<Renderer>().material);
	}
}
