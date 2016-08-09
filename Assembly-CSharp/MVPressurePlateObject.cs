using UnityEngine;

public class MVPressurePlateObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private Transform plateModelTransform;

	public GameObject useInteractionRotator;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public Transform PlateModelTranform => plateModelTransform;

	protected override void OnValidate()
	{
	}
}
