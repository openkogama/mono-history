using UnityEngine;

public class FinishLineObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private TintObject tintObject;

	public GameObject useInteractionRotator;

	public GameObject VisualObject => visualObject;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public TintObject TintObject => tintObject;

	protected override void OnValidate()
	{
	}
}
