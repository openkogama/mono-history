using UnityEngine;

public class FreeGoldAndroid : MonoBehaviour, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	[SerializeField]
	private GameObject freeGoldButton;

	public void UpdateControllerUpdate()
	{
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	public void Initialize()
	{
	}
}
