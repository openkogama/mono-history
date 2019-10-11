using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragInputHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
{
	private List<IDragInputReciever> dragInputRecievers = new List<IDragInputReciever>();

	public void AddInputReciever(IDragInputReciever newInputReciever)
	{
		dragInputRecievers.Add(newInputReciever);
	}

	public void RemoveInputReciever(IDragInputReciever inputReciever)
	{
		dragInputRecievers.Remove(inputReciever);
	}

	public void OnBeginDrag(PointerEventData data)
	{
		for (int i = 0; i < dragInputRecievers.Count; i++)
		{
			dragInputRecievers[i].OnBeginDrag();
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		for (int i = 0; i < dragInputRecievers.Count; i++)
		{
			dragInputRecievers[i].OnEndDrag();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		for (int i = 0; i < dragInputRecievers.Count; i++)
		{
			dragInputRecievers[i].OnDrag();
		}
	}
}
