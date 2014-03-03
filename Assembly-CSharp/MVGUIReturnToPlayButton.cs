using UnityEngine;

public class MVGUIReturnToPlayButton : MonoBehaviour
{
	[SerializeField]
	private UXView _uxView;

	[SerializeField]
	private UXView _uxViewTouristHelpText;

	public void Awake()
	{
		if (!UXUtils.FindGUIObjectOfType<MVGUIRoot>().ShowRegisterMenuForTourist)
		{
			((Component)this).gameObject.SetActiveRecursively(false);
		}
	}

	private void Update()
	{
		if (MVGameController.Instance.IsTouristSession && MVGameController.Instance.Game.IsPlaying && !LockCursorManager.HasFocusAndLockCursor)
		{
			_uxView.Show();
			_uxViewTouristHelpText.Show();
		}
		else
		{
			_uxView.Hide();
			_uxViewTouristHelpText.Hide();
		}
	}
}
