using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class FireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	private string buttonName;

	private void OnDisable()
	{
		Reset();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		CrossPlatformInputManager.SetButtonUp(buttonName);
	}

	private void Reset()
	{
		CrossPlatformInputManager.SetButtonUp(buttonName);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		CrossPlatformInputManager.SetButtonDown(buttonName);
	}
}
