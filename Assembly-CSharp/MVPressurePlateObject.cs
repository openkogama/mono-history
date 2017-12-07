using UnityEngine;

public class MVPressurePlateObject : ObjectPrefab
{
	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private Transform plateModelTransform;

	[SerializeField]
	private PressurePlateTintObject tintObject;

	[SerializeField]
	private GameObject plateLogicModel;

	public GameObject useInteractionRotator;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public GameObject TriggerBoxLogic => plateLogicModel;

	public Transform PlateModelTranform => plateModelTransform;

	public PressurePlateTintObject TintObject => tintObject;

	protected override void OnValidate()
	{
	}
}
