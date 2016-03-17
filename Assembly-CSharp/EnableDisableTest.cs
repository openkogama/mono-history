using UnityEngine;

public class EnableDisableTest : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		Debug.Log("OnEnable");
	}

	private void OnDisable()
	{
		Debug.Log("OnDisable");
	}
}
