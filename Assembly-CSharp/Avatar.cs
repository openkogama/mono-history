using System;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour, IBulletImpactVisualizer, IMovable
{
	public MVAvatar mvAvatar;

	private bool isLocal;

	private Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers = new Dictionary<AvatarModifierPackageType, AvatarModifier>();

	private Dictionary<AvatarModifierPackageType, byte> currentModifierByteState = new Dictionary<AvatarModifierPackageType, byte>();

	private byte[] modifierEffectCount = new byte[22];

	private InteractionDataHandlerBase interactionDataHandler;

	private Collider avatarCollider;

	[SerializeField]
	private AvatarLevelUp avatarLevelUp;

	[SerializeField]
	private AvatarFader avatarFader;

	[SerializeField]
	public GameObject root;

	[SerializeField]
	private AvatarBulletImpactVisualizer bulletImpactVisualizer;

	[SerializeField]
	private WaterSplashComponent waterSplashComponent;

	[SerializeField]
	private AvatarEnabledChangeHandler enabledChangeHandler;

	[SerializeField]
	protected AvatarUIHandler avatarUIHandler;

	[SerializeField]
	private ChatAnchor chatBubbleAnchor;

	public bool IsLocal => isLocal;

	public InteractionDataHandlerBase InteractionDataHandlerBase => interactionDataHandler;

	public Collider Collider => avatarCollider;

	public AvatarFader AvatarFader => avatarFader;

	public AvatarEnabledChangeHandler EnabledChangeHandler => enabledChangeHandler;

	public AvatarUIHandler AvatarUIHandler => avatarUIHandler;

	public ChatAnchor ChatBubbleAnchor => chatBubbleAnchor;

	public Vector3 Velocity => mvAvatar.VelocityAbsolute;

	public Bounds Bounds => mvAvatar.GetLocalBounds(BoundsContext.Default);

	public Vector3 Position => transform.position;

	public virtual void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		this.mvAvatar = mvAvatar;
		avatarFader.BodyTransform = mvAvatar.Body.Transform;
		this.isLocal = isLocal;
		interactionDataHandler = GetComponent<InteractionDataHandlerBase>();
		avatarCollider = GetComponent<Collider>();
		avatarLevelUp.Init(mvAvatar.OwnerActorNr);
		waterSplashComponent.Initialize(this);
		avatarUIHandler.Initialize(IsLocal, mvAvatar, mvAvatar.OwnerActorNr, chatBubbleAnchor);
		chatBubbleAnchor.Initialize(isLocal, this);
	}

	public void UpdateModifiers(Dictionary<object, object> newModifiers)
	{
		List<AvatarModifierPackageType> list = new List<AvatarModifierPackageType>();
		foreach (AvatarModifierPackageType key in modifiers.Keys)
		{
			string text = "_" + key;
			if (!newModifiers.ContainsKey(text))
			{
				AvatarModifierPackageType item = (AvatarModifierPackageType)Enum.Parse(typeof(AvatarModifierPackageType), text.TrimStart('_'));
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
			AvatarModifierPackageType avatarModifierPackageType2 = (AvatarModifierPackageType)Enum.Parse(typeof(AvatarModifierPackageType), text2.TrimStart('_'));
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

	public void OnEnterVehicle()
	{
		waterSplashComponent.enabled = false;
	}

	public void OnExitVehicle()
	{
		waterSplashComponent.enabled = true;
	}

	public void StartBlinking(BlinkType type, float duration = float.PositiveInfinity)
	{
		mvAvatar.Body.StartBlinking(type, duration);
	}

	public void StopBlinking(BlinkType type)
	{
		mvAvatar.Body.StopBlinking(type);
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage = 100f)
	{
		bulletImpactVisualizer.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
	}

	public bool HasModifierEffect(AvatarModifierEffect modifierEffect)
	{
		return modifierEffectCount[(int)modifierEffect] > 0;
	}
}
