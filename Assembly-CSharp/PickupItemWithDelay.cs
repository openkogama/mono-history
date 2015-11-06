using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public abstract class PickupItemWithDelay : PickupItem
{
	public Color crossHairCannotFireLow = Color.red;

	public Color crossHairCannotFireHigh = Color.yellow;

	public Color crossHairCanFire = Color.green;

	[SerializeField]
	protected ObscuredFloat fireInterval = 1f;

	protected bool isFiring;

	private float lastFireTime;

	protected virtual bool IsAmmoDepleted => false;

	public override Color CrossHairColor
	{
		get
		{
			float num = Time.time - lastFireTime;
			float num2 = Mathf.Clamp01(num / (float)fireInterval);
			if (num2 < 1f)
			{
				return Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, num2);
			}
			return crossHairCanFire;
		}
	}

	protected virtual void OnFire(bool isLocal)
	{
	}

	protected virtual void OnStart()
	{
	}

	private void Start()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		OnStart();
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (isFiring)
		{
			Debug.Log("Got TriggerStart, but were firing");
		}
		else
		{
			StartCoroutine(DoAutoFire());
		}
	}

	public override void TriggerEnd()
	{
		isFiring = false;
	}

	private IEnumerator DoAutoFire()
	{
		isFiring = true;
		while (isFiring)
		{
			if (!IsAmmoDepleted && Time.time - lastFireTime > (float)fireInterval)
			{
				lastFireTime = Time.time;
				OnFire(owner.IsLocal);
				firedThisFrame = true;
			}
			if (IsAmmoDepleted)
			{
				MVEquipable equipable = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
				if (equipable != null)
				{
					equipable.Unequip();
				}
				isFiring = false;
				break;
			}
			yield return 0;
		}
	}
}
