using UnityEngine;

public class MVCheckpointObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private Animation objAnimation;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public Animation Animation => objAnimation;

	public GameObject VisualObject => visualObject;

	protected override void OnValidate()
	{
	}
}
