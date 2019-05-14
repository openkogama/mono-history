using UnityEngine;

public class TriggerCubePrefab : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private TriggerCubeTintObject tintObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public TriggerCubeTintObject TintObject => tintObject;

	public void SetScale(Vector3 scale)
	{
		triggerBoxEvents.transform.localScale = scale;
	}

	protected override void OnValidate()
	{
	}
}
