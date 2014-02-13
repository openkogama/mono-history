using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class Avatar : MonoBehaviour
{
	public MVAvatar mvAvatar;

	private HealthBar healthBar;

	private bool isLocal;

	private string username;

	private Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers = new Dictionary<AvatarModifierPackageType, AvatarModifier>();

	public bool IsLocal => isLocal;

	public HealthBar HealthAndOxygenBar => healthBar;

	public bool ShowHealth
	{
		set
		{
			((Component)healthBar).gameObject.SetActiveRecursively(value);
		}
	}

	public bool IsHealthShown => ((Component)healthBar).gameObject.active;

	public string NameTag
	{
		set
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			((Component)this).gameObject.GetComponentInChildren<TextMesh>().text = value;
			Color color = Color.white;
			switch (MVGameController.Instance.Game.Players[mvAvatar.OwnerActorNr].Team)
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
			((Component)((Component)this).gameObject.GetComponentInChildren<TextMesh>()).renderer.material.color = color;
		}
	}

	public float Health
	{
		set
		{
			healthBar.Health = value;
		}
	}

	public void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		this.mvAvatar = mvAvatar;
		this.isLocal = isLocal;
		healthBar = ((Component)this).GetComponentInChildren<HealthBar>();
		healthBar.Oxygen = 0f;
	}

	public void UpdateModifiers(Hashtable newModifiers)
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		List<AvatarModifierPackageType> list = new List<AvatarModifierPackageType>();
		foreach (AvatarModifierPackageType key in modifiers.Keys)
		{
			string text = "_" + key;
			if (!newModifiers.ContainsKey(text))
			{
				AvatarModifierPackageType item = (AvatarModifierPackageType)(int)Enum.Parse(typeof(AvatarModifierPackageType), text.TrimStart(new char[1] { '_' }));
				list.Add(item);
			}
		}
		foreach (AvatarModifierPackageType item2 in list)
		{
			modifiers[item2].Deactivate(this);
			modifiers.Remove(item2);
		}
		foreach (DictionaryEntry newModifier in newModifiers)
		{
			string text2 = newModifier.Key as string;
			AvatarModifierPackageType avatarModifierPackageType = (AvatarModifierPackageType)(int)Enum.Parse(typeof(AvatarModifierPackageType), text2.TrimStart(new char[1] { '_' }));
			if (!modifiers.ContainsKey(avatarModifierPackageType))
			{
				AvatarModifier avatarModifier = AvatarModifier.CreateFromType(avatarModifierPackageType, this);
				if ((Object)(object)avatarModifier != (Object)null)
				{
					((Component)avatarModifier).transform.parent = ((Component)this).transform;
					((Component)avatarModifier).transform.localPosition = Vector3.zero;
					modifiers.Add(avatarModifierPackageType, avatarModifier);
					avatarModifier.Activate(this);
				}
			}
		}
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
