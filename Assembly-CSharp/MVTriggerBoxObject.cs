using UnityEngine;

public class MVTriggerBoxObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	public GameObject useInteractionRotator;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
