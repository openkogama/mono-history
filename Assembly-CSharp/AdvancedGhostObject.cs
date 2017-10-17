using UnityEngine;

public class AdvancedGhostObject : ObjectPrefab
{
	[SerializeField]
	private AdvancedGhostTintObject tintObject;

	public TintObject TintObject => tintObject;

	public Material TintedMaterial => tintObject.Material;

	protected void Reset()
	{
		tintObject = GetComponent<AdvancedGhostTintObject>();
	}

	protected override void OnValidate()
	{
	}
}
