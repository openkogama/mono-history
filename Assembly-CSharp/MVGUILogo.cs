using UnityEngine;

public class MVGUILogo : MonoBehaviour
{
	[SerializeField]
	private UXView _uxView;

	private void Awake()
	{
		Debug.LogWarning((object)"Hardcoded disable of MVGUILogo...");
		((Component)this).gameObject.SetActiveRecursively(false);
	}

	private void Update()
	{
		if (!MVGameController.Instance.GameJoined || MVGameController.Instance.IsTouristSession)
		{
			_uxView.Show();
		}
		else if (MVGameController.Instance.IsTouristSession && MVGameController.Instance.Game.IsPlaying && !LockCursorManager.HasFocusAndLockCursor)
		{
			_uxView.Show();
		}
		else
		{
			_uxView.Hide();
		}
	}
}
