using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private List<HoverInputReceiver> hoverInputReceivers = new List<HoverInputReceiver>();

	public void SubscribeToHoverInput(HoverInputReceiver newInputReceiver)
	{
		hoverInputReceivers.Add(newInputReceiver);
	}

	public void UnsubscribeToHoverInput(HoverInputReceiver newInputReceiver)
	{
		hoverInputReceivers.Remove(newInputReceiver);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		for (int i = 0; i < hoverInputReceivers.Count; i++)
		{
			hoverInputReceivers[i].OnHoverEnter();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		for (int i = 0; i < hoverInputReceivers.Count; i++)
		{
			hoverInputReceivers[i].OnHoverExit();
		}
	}
}
