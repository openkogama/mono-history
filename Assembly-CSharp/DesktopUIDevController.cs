using UnityEngine;

public class DesktopUIDevController : MonoBehaviour
{
	[SerializeField]
	private UIStack uiStack;

	[SerializeField]
	private GameObject stackBottom;

	private void Awake()
	{
		uiStack.Push(stackBottom);
	}
}
