using UnityEngine;

public class MVGUISubTree : MonoBehaviour
{
	public void Awake()
	{
		UXUtils.AddSubTree(transform);
	}
}
