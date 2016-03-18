using UnityEngine;

public class MVTriggerBoxObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	protected override void OnValidate()
	{
	}
}
