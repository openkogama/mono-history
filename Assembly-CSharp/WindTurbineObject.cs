using UnityEngine;

public class WindTurbineObject : ObjectPrefab
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private Transform areaColliderTransform;

	[SerializeField]
	private Collider areaCollider;

	[SerializeField]
	private Collider editorCollider;

	[SerializeField]
	private ParticleSystem windParticleSystem;

	[SerializeField]
	private TriggerBoxEvents triggerBoxEvents;

	public GameObject VisualObject => visualObject;

	public Transform AreaColliderTransform => areaColliderTransform;

	public Collider AreaCollider => areaCollider;

	public Collider EditorCollider => editorCollider;

	public ParticleSystem WindParticleSystem => windParticleSystem;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;
}
