using UnityEngine;

public class PickupItemObjectScript : MonoBehaviour
{
	public GameObject pickupObject;

	private void Awake()
	{
		((Behaviour)this).enabled = false;
	}
}
