using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class Avatar : MonoBehaviour
{
	public MVAvatar mvAvatar;

	private bool isLocal;

	private Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers = new Dictionary<AvatarModifierPackageType, AvatarModifier>();

	private static string _particlePrefab = "ParticleFX/XP";

	[SerializeField]
	private AvatarBadge avatarBadge;

	[SerializeField]
	private TextMesh avatarName;

	[SerializeField]
	private AvatarLevelUp avatarLevelUp;

	[SerializeField]
	private Transform nameTagLabel;

	private bool nameTagLabelVisible;

	public bool IsLocal => isLocal;

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

	public void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		this.mvAvatar = mvAvatar;
		this.isLocal = isLocal;
		if (isLocal)
		{
			UnityEngine.Object.Destroy(avatarBadge.gameObject);
			MVLocalPlayer localPlayer = MVGameController.Game.LocalPlayer;
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
		ParticleSystem component = (UnityEngine.Object.Instantiate(Resources.Load(_particlePrefab)) as GameObject).GetComponent<ParticleSystem>();
		component.transform.parent = transform;
		component.transform.localPosition = Vector3.up;
		component.transform.localRotation = Quaternion.identity;
		component.transform.localScale = Vector3.one;
		component.emissionRate = xpProgressData.XPDelta;
		component.Play();
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
					avatarModifier.transform.parent = transform;
					avatarModifier.transform.localPosition = Vector3.zero;
					modifiers.Add(avatarModifierPackageType, avatarModifier);
					avatarModifier.Activate(this);
				}
			}
		}
	}

	public void UpdateNameTag()
	{
		avatarName.text = MVGameController.Game.Players[mvAvatar.OwnerActorNr].Username;
		Color color = Color.white;
		if (MVGameController.Game.TeamManager.TeamCount() > 1)
		{
			switch (MVGameController.Game.Players[mvAvatar.OwnerActorNr].Team)
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
}
