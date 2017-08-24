using UnityEngine;

public class TailDebug : MonoBehaviour
{
	public bool forceupdate;

	private void OnValidate()
	{
		transform.up = transform.position;
	}
}
