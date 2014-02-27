using UnityEngine;

public class MVGUITouristButtons : MonoBehaviour
{
	[SerializeField]
	private UXView _uxViewTouristButtons;

	private void Start()
	{
		if (MVGameController.Instance.IsTouristSession && UXUtils.FindGUIObjectOfType<MVGUIRoot>().ShowRegisterMenuForTourist)
		{
			UXUtils.FindGUIObjectOfType<LockCursorManager>().ForceLoseFocus();
		}
		else
		{
			((Component)this).gameObject.SetActiveRecursively(false);
		}
	}

	private void Update()
	{
		if (!LockCursorManager.HasFocusAndLockCursor && MVGameController.Instance.IsTouristSession)
		{
			_uxViewTouristButtons.Show();
		}
		else
		{
			_uxViewTouristButtons.Hide();
		}
	}
}
