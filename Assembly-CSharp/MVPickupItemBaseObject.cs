using UnityEngine;

public class MVPickupItemBaseObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private GreyOutObjectScript pickupItem;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AudioSource AudioSource => audioSource;

	public GreyOutObjectScript PickupItem => pickupItem;

	protected override void OnValidate()
	{
		pickupItem = GetComponent<GreyOutObjectScript>();
		triggerBoxEvents = GetComponentInChildren<TriggerBoxEvents>();
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		pickupItem.pickupObject.transform.Rotate(Vector3.up, 68f * Time.deltaTime);
	}
}
