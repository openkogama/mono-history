using UnityEngine;

public class MVCheckpointObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private Animation animation;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public Animation Animation => animation;

	protected override void OnValidate()
	{
	}
}
