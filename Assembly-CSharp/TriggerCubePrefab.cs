using UnityEngine;

public class TriggerCubePrefab : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public void SetScale(Vector3 scale)
	{
		triggerBoxEvents.transform.localScale = scale;
	}

	protected override void OnValidate()
	{
	}
}
