using UnityEngine;

public class CFX_AutodestructWhenNoChildren : MonoBehaviour
{
	private void Update()
	{
		if (((Component)this).transform.GetChildCount() == 0)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
