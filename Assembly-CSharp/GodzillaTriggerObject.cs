using System.Collections.Generic;
using UnityEngine;

public class GodzillaTriggerObject : MVTriggerBoxObject
{
	[SerializeField]
	private ShockWaveEmitter shockWaveEmitter;

	[SerializeField]
	private GameObject logicCube;

	[SerializeField]
	private List<GameObject> toHideOnEntry = new List<GameObject>();

	[SerializeField]
	private Material inventoryCoverMaterial;

	[SerializeField]
	private MeshRenderer cover;

	public ShockWaveEmitter ShockWaveEmitter => shockWaveEmitter;

	public GameObject LogicCube => logicCube;

	public List<GameObject> ToHideOnEntry => toHideOnEntry;

	public Material InventoryCoverMaterial => inventoryCoverMaterial;

	public MeshRenderer Cover => cover;
}
