using UnityEngine;

public class CFX_AutodestructWhenNoChildren : MonoBehaviour
{
	private void Update()
	{
		if (transform.childCount == 0)
		{
			Object.Destroy(gameObject);
		}
	}
}
