using UnityEngine;

public class MVGUIRoot : MonoBehaviour
{
	public void Awake()
	{
		Object.DontDestroyOnLoad(transform.gameObject);
	}
}
