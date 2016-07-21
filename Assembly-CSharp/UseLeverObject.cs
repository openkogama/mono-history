using UnityEngine;

public class UseLeverObject : ObjectPrefab
{
	[SerializeField]
	private Collider leverCollider;

	[SerializeField]
	private Collider editCollider;

	[SerializeField]
	private UseInteractor useInteractor;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	[SerializeField]
	private Transform plateButtonTransform;

	[SerializeField]
	private GameObject visualRoot;

	public Collider LeverCollider => leverCollider;

	public Collider EditCollider => editCollider;

	public UseInteractor UseInteractor
	{
		get
		{
			return useInteractor;
		}
		set
		{
			useInteractor = value;
		}
	}

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;

	public Transform PlateButtonTransform => plateButtonTransform;

	public GameObject VisualRoot => visualRoot;

	protected override void OnValidate()
	{
	}
}
