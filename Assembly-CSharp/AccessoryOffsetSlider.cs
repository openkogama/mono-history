using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AccessoryOffsetSlider : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private float defaultValue;

	private AccessorySlotType accessorySlot;

	private MVBody avatarBody;

	private bool isInPreview;

	public bool IsInPreview
	{
		set
		{
			isInPreview = value;
		}
	}

	public void Initialize(AccessorySlotType accessorySlot, int streamingAssetID)
	{
		this.accessorySlot = accessorySlot;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(Initialize);
			});
		}
		else
		{
			Initialize(MVGameControllerBase.LocalPlayer.Body);
		}
	}

	private void Initialize(MVBody avatarBody)
	{
		this.avatarBody = avatarBody;
		if (!isInPreview)
		{
			slider.value = avatarBody.GetAccessoryOffset(accessorySlot);
		}
		else
		{
			slider.value = defaultValue;
		}
	}

	public void ValueChanged()
	{
		float value = slider.value;
		avatarBody.ApplyAccessoryOffset(value, accessorySlot);
	}

	public void ChangeValue(float value)
	{
		slider.value += value;
	}

	private void Reset()
	{
		slider = GetComponent<Slider>();
	}

	public void SyncPosition()
	{
		avatarBody.SyncOffset(accessorySlot, slider.value);
	}
}
