using UnityEngine;

public class GodzillaArea : MonoBehaviour
{
	[SerializeField]
	private KillZone killZone;

	[SerializeField]
	private ForceField forceField;

	public GameObject KillZone => killZone.gameObject;

	public GameObject ForceField => forceField.gameObject;
}
