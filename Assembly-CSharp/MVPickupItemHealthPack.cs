using UnityEngine;

public class MVPickupItemHealthPack : MVPickupItemBase
{
	private GameObject pickupMesh;

	private bool isVisible = true;

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/PickupItemHealthPackObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		PickupItemObjectScript component = gameObject.GetComponent<PickupItemObjectScript>();
		pickupMesh = component.pickupObject;
		pickupMesh.transform.Rotate(new Vector3(Random.Range(0f, -180f), Random.Range(0f, -180f), Random.Range(0f, 180f)));
		base.CreateMVWOC(local);
	}

	protected override void OnUpdate()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (isVisible)
		{
			pickupMesh.transform.Rotate(new Vector3(-62f * Time.deltaTime, -28.3f * Time.deltaTime, 22.4f * Time.deltaTime));
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
