using UnityEngine;

public class CollectTheItemDropOffObject : ObjectPrefab
{
	[SerializeField]
	private Collider editCollider;

	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private CollectTheItemBlinker blinker;

	[SerializeField]
	private GameObject cullingObject;

	[SerializeField]
	private GreyOutObjectScript greyout;

	public GameObject CullingObject => cullingObject;

	public CollectTheItemBlinker Blinker => blinker;

	public GameObject VisualObject => visualObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public Collider EditCollider => editCollider;

	public GreyOutObjectScript GreyOutScript => greyout;

	protected override void OnValidate()
	{
	}
}
