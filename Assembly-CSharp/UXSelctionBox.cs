using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("UX/Elements/Selection Box")]
public class UXSelctionBox : MonoBehaviour
{
	private void Awake()
	{
		((Component)this).GetComponent<MeshFilter>().mesh = UXUtils.BuildPlaneMesh();
	}
}
