using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public abstract class PickupItemWithDelay : PickupItem, IUpdatecontrollerSubscriber
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

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (isFiring)
		{
			Debug.Log("Got TriggerStart, but were firing");
		}
		else
		{
			isFiring = true;
		}
	}

	public override void TriggerEnd()
	{
		isFiring = false;
	}

	public override void OnEquip()
	{
		base.OnEquip();
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	private void Fire()
	{
		if (!IsAmmoDepleted && Time.time - lastFireTime > (float)fireInterval)
		{
			lastFireTime = Time.time;
			OnFire(owner.IsLocal);
			firedThisFrame = true;
		}
		if (IsAmmoDepleted)
		{
			MVEquipable component = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
			isFiring = false;
		}
	}

	protected virtual void OnDestroy()
	{
		Debug.Log("OnDestroy");
		UpdateController.RemoveUpdateObject(this);
	}

	public virtual void UpdateControllerUpdate()
	{
		if (isFiring)
		{
			Fire();
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
