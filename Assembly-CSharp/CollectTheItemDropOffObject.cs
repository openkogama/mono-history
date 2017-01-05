using UnityEngine;

public class CollectTheItemDropOffObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private CollectTheItemBlinker blinker;

	public CollectTheItemBlinker Blinker => blinker;

	public GameObject VisualObject => visualObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	protected override void OnValidate()
	{
	}
}
