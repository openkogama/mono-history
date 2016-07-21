using UnityEngine;

public class MVTriggerBoxObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
