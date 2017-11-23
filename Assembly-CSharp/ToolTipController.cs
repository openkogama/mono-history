using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipController : MonoBehaviour, IEventSystemHandler, IHandleToolTip
{
	private bool updatedThisFrame;

	[SerializeField]
	private ToolTipUI toolTipUi;

	private void Awake()
	{
		toolTipUi = Object.Instantiate(toolTipUi);
		toolTipUi.transform.SetParent(transform, worldPositionStays: false);
	}

	public void SendToolTip(Vector2 position, string toolTip)
	{
		if (!string.IsNullOrEmpty(toolTip))
		{
			if (!toolTipUi.gameObject.activeSelf)
			{
				toolTipUi.gameObject.SetActive(value: true);
			}
			toolTipUi.Set(position, toolTip);
			updatedThisFrame = true;
		}
	}

	private void LateUpdate()
	{
		if (!updatedThisFrame)
		{
			toolTipUi.gameObject.SetActive(value: false);
		}
		updatedThisFrame = false;
	}
}
