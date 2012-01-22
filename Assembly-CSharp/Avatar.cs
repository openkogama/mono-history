using System;
using UnityEngine;

public class Avatar : MonoBehaviour
{
	public MVWorldObjectClient worldObject;

	public MVAvatar mvAvatar;

	private HealthBar healthBar;

	private bool isLocal;

	private string username;

	private AvatarAnimation avatarAnimation;

	private Vector3 lastPosition;

	private Vector3 lookDirection;

	private Vector3 lookOrigin;

	private GameObject crossHair;

	private GUIText ammoText;

	private Vector3 velocity = Vector3.zero;

	private AvatarItemSlot[] avatarItemSlots;

	private bool inGunMode;

	private PlayModeWithInertiaCamera thirdPersonCam;

	public AvatarAnimation AvatarAnimation => avatarAnimation;

	public bool IsLocal => isLocal;

	public Vector3 LookDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lookDirection;
		}
	}

	public Vector3 LookOrigin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return lookOrigin;
		}
	}

	public bool InGunMode => inGunMode;

	public bool ShowHealth
	{
		set
		{
			((Component)healthBar).gameObject.SetActiveRecursively(value);
		}
	}

	public string NameTag
	{
		set
		{
			((Component)this).gameObject.GetComponentInChildren<TextMesh>().text = value;
		}
	}

	public float Health
	{
		set
		{
			healthBar.Health = value;
		}
	}

	public Avatar()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Initialize(MVAvatar mvAvatar, bool isLocal)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		worldObject = mvAvatar;
		this.mvAvatar = mvAvatar;
		this.isLocal = isLocal;
		if (isLocal)
		{
			crossHair = GameObject.Find("CrossHair");
			ammoText = crossHair.GetComponentInChildren<GUIText>();
			crossHair.SetActiveRecursively(false);
			Debug.LogWarning((object)("Ammo text = " + ammoText));
			thirdPersonCam = Object.FindObjectOfType(typeof(PlayModeWithInertiaCamera)) as PlayModeWithInertiaCamera;
		}
		InitializeAvatarAnimation();
		healthBar = ((Component)this).GetComponentInChildren<HealthBar>();
		avatarItemSlots = new AvatarItemSlot[Enum.GetValues(typeof(AvatarItemSlotName)).Length];
		avatarItemSlots[0] = new AvatarItemSlot();
		avatarItemSlots[0].offset = new Vector3(0f, 1f, 0f);
		avatarItemSlots[1] = new AvatarItemSlot();
		avatarItemSlots[1].offset = new Vector3(1f, 1f, 0f);
		avatarItemSlots[2] = new AvatarItemSlot();
		avatarItemSlots[2].offset = new Vector3(-1f, 1f, 0f);
		avatarItemSlots[3] = new AvatarItemSlot();
		avatarItemSlots[3].offset = new Vector3(0f, 2f, 0f);
	}

	public void Equip(AvatarItemSlotName slotName, string prefabName)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (prefabName.Length == 0)
		{
			Unequip(slotName);
			return;
		}
		GameObject val = (GameObject)Object.Instantiate(Resources.Load(prefabName));
		if ((Object)(object)val == (Object)null)
		{
			Debug.LogWarning((object)("Cannot equip item with prefabName: " + prefabName + ", since prefab was not found"));
			return;
		}
		AvatarItem component = val.GetComponent<AvatarItem>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogWarning((object)"Item to equip is missing the AvatarItem script");
			return;
		}
		component.owner = this;
		if ((Object)(object)avatarItemSlots[(int)slotName].equippedItem != (Object)null)
		{
			Unequip(slotName);
		}
		val.transform.position = ((Component)this).gameObject.transform.position;
		val.transform.rotation = ((Component)this).gameObject.transform.rotation;
		val.transform.parent = ((Component)this).gameObject.transform;
		val.transform.localPosition = avatarItemSlots[(int)slotName].offset;
		avatarItemSlots[(int)slotName].equippedItem = component;
		inGunMode = true;
		if (Object.op_Implicit((Object)(object)crossHair))
		{
			crossHair.SetActiveRecursively(true);
		}
	}

	private void Unequip(AvatarItemSlotName slotName)
	{
		if ((Object)(object)avatarItemSlots[(int)slotName].equippedItem == (Object)null)
		{
			return;
		}
		avatarItemSlots[(int)slotName].equippedItem.itemObject.transform.parent = null;
		Object.Destroy((Object)(object)avatarItemSlots[(int)slotName].equippedItem.itemObject);
		Object.Destroy((Object)(object)avatarItemSlots[(int)slotName].equippedItem);
		avatarItemSlots[(int)slotName].equippedItem = null;
		bool flag = false;
		AvatarItemSlot[] array = avatarItemSlots;
		foreach (AvatarItemSlot avatarItemSlot in array)
		{
			if ((Object)(object)avatarItemSlot.equippedItem != (Object)null)
			{
				flag = true;
			}
		}
		inGunMode = flag;
		if (!inGunMode && Object.op_Implicit((Object)(object)crossHair))
		{
			crossHair.SetActiveRecursively(false);
		}
	}

	public void HandleFiring(bool isFiring)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (inGunMode && isFiring)
		{
			Vector3 forward = lookDirection;
			forward.y = 0f;
			if (velocity.sqrMagnitude < 0.01f)
			{
				((Component)this).transform.forward = forward;
			}
		}
		foreach (object value in Enum.GetValues(typeof(AvatarItemSlotName)))
		{
			if ((Object)(object)avatarItemSlots[(int)value].equippedItem != (Object)null)
			{
				if (isFiring)
				{
					avatarItemSlots[(int)value].equippedItem.TriggerBegin(mvAvatar.Id);
				}
				else
				{
					avatarItemSlots[(int)value].equippedItem.TriggerEnd();
				}
			}
		}
	}

	public void SetAnimation(string animationState)
	{
		AvatarAnimation.SetState(animationState);
	}

	private void InitializeAvatarAnimation()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		string text = "Prefabs/Avatar/Animations/";
		Object val = Object.Instantiate(Resources.Load(text + "Boy"));
		avatarAnimation = ((GameObject)((val is GameObject) ? val : null)).GetComponent<AvatarAnimation>();
		avatarAnimation.Initialize();
		((Component)avatarAnimation).transform.parent = ((Component)this).transform;
		((Component)avatarAnimation).transform.localPosition = Vector3.zero;
		avatarAnimation.SetVisible(visible: true);
	}

	public void SetAvatarVisibility(bool isVisible)
	{
		avatarAnimation.SetVisible(isVisible);
	}

	public void Update()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (isLocal)
		{
			lookDirection = thirdPersonCam.FireDirection;
			lookOrigin = thirdPersonCam.FireOrigin;
		}
		velocity = ((Component)this).transform.position - lastPosition;
		if (inGunMode)
		{
			Vector3 forward = lookDirection;
			forward.y = 0f;
			if (velocity.sqrMagnitude > 0.01f)
			{
				((Component)this).transform.forward = forward;
			}
			if (Object.op_Implicit((Object)(object)ammoText))
			{
				ammoText.text = string.Empty + avatarItemSlots[0].equippedItem.Quantity;
			}
			thirdPersonCam.lookAtOffset = new Vector3(0.7f, 0.4f, 0f);
		}
		else
		{
			thirdPersonCam.lookAtOffset = new Vector3(0f, 0.7f, 0f);
		}
		if (!isLocal)
		{
			Vector3 position = ((Component)this).transform.position;
			avatarAnimation.Move(Vector3.Distance(position, lastPosition) * 0.2f);
		}
		lastPosition = ((Component)this).transform.position;
	}

	public void SetLineOfFire(Vector3 camOrigin, Vector3 camDir)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		lookOrigin = camOrigin;
		lookDirection = camDir;
	}
}
