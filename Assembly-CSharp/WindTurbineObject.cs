using UnityEngine;

public class WindTurbineObject : ObjectPrefab
{
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

	public Transform AreaColliderTransform => areaColliderTransform;

	public Collider AreaCollider => areaCollider;

	public Collider EditorCollider => editorCollider;

	public ParticleSystem WindParticleSystem => windParticleSystem;

	public TriggerBoxEvents TriggerBoxEvents => triggerBoxEvents;
}
