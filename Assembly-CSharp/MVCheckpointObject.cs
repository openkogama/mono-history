using UnityEngine;

public class MVCheckpointObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private Animation objAnimation;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public Animation Animation => objAnimation;

	protected override void OnValidate()
	{
	}
}
