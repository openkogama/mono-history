using System.Collections;
using UnityEngine;

public abstract class PickupItemWithDelay : PickupItem
{
	public Color crossHairCannotFireLow = Color.red;

	public Color crossHairCannotFireHigh = Color.yellow;

	public Color crossHairCanFire = Color.green;

	public Transform muzzlePoint;

	public float fireInterval = 1f;

	protected bool isFiring;

	private float lastFireTime;

	protected virtual bool IsAmmoDepleted => false;

	public override Color CrossHairColor
	{
		get
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			float num = Time.time - lastFireTime;
			float num2 = Mathf.Clamp01(num / fireInterval);
			if (num2 < 1f)
			{
				return Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, num2);
			}
			return crossHairCanFire;
		}
	}

	protected PickupItemWithDelay()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	protected virtual void OnFire(bool isLocal)
	{
	}

	protected virtual void OnStart()
	{
	}

	private void Start()
	{
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		OnStart();
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (isFiring)
		{
			Debug.Log((object)"Got TriggerStart, but were firing");
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(DoAutoFire());
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
			if (!IsAmmoDepleted && Time.time - lastFireTime > fireInterval)
			{
				lastFireTime = Time.time;
				OnFire(owner.IsLocal);
			}
			if (IsAmmoDepleted)
			{
				MVEquipable equipable = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
				if ((Object)(object)equipable != (Object)null)
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
