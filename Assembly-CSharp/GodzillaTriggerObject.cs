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

	public ShockWaveEmitter ShockWaveEmitter => shockWaveEmitter;

	public GameObject LogicCube => logicCube;

	public List<GameObject> ToHideOnEntry => toHideOnEntry;
}
