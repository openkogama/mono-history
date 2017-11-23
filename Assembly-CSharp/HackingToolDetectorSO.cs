using UnityEngine;

public class HackingToolDetectorSO : ScriptableObject
{
	[SerializeField]
	[Tooltip("Scans per second.")]
	private float scanFrequency = 1f / 60f;

	public float ScanFrequency => scanFrequency;
}
