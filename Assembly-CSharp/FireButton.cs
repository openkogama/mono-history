using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class FireButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IEventSystemHandler
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
