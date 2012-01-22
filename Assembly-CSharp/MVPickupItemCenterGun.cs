using UnityEngine;

public class MVPickupItemCenterGun : MVPickupItemBase
{
	private GameObject pickupMesh;

	private bool isVisible = true;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/PickupItemCenterGun"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		PickupItemObjectScript component = gameObject.GetComponent<PickupItemObjectScript>();
		pickupMesh = component.pickupObject;
		base.CreateMVWOC(local);
	}

	protected override void OnUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (isVisible)
		{
			pickupMesh.transform.Rotate(Vector3.up, 68f * Time.deltaTime);
		}
	}

	public override void OnPickup()
	{
		isVisible = false;
		pickupMesh.SetActiveRecursively(false);
		gameObject.audio.Play();
	}

	public override void OnRespawn()
	{
		isVisible = true;
		pickupMesh.SetActiveRecursively(true);
	}
}
