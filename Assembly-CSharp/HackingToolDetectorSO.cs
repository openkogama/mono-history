using UnityEngine;

public class HackingToolDetectorSO : ScriptableObject
{
	[Tooltip("Scans per second.")]
	[SerializeField]
	private float scanFrequency = 1f / 60f;

	public float ScanFrequency => scanFrequency;
}
