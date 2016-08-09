using UnityEngine;

public class MVPickupItemBaseObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private GreyOutObjectScript pickupItem;

	public GameObject useInteractionRotator;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public AudioSource AudioSource => audioSource;

	public GreyOutObjectScript PickupItem => pickupItem;

	protected override void OnValidate()
	{
		pickupItem = GetComponent<GreyOutObjectScript>();
		triggerBoxEvents = GetComponentInChildren<TriggerBoxEvents>();
		audioSource = GetComponent<AudioSource>();
	}
}
