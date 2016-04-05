using UnityEngine;

public class ShootableButtonObject : ObjectPrefab
{
	[SerializeField]
	private Collider editCollider;

	[SerializeField]
	private GreyOutObjectScript greyOutObject;

	[SerializeField]
	private Collider targetCollider2D;

	[SerializeField]
	private Collider targetCollider3D;

	public Collider EditCollider => editCollider;

	public Collider TargetCollider2D => targetCollider2D;

	public Collider TargetCollider3D => targetCollider3D;

	public GreyOutObjectScript GreyOutObject => greyOutObject;

	protected override void OnValidate()
	{
	}
}
