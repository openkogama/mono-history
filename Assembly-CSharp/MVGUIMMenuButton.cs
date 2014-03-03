using UnityEngine;

public class MVGUIMMenuButton : MonoBehaviour
{
	[SerializeField]
	private UXBaseButton _baseButton;

	[SerializeField]
	private UXView _uxView;

	private bool isDead;

	private void Start()
	{
		_baseButton.OnClick = () =>
		{
			MVGameController.Instance.IngameController.ToggleMenu();
		};
	}

	private void Update()
	{
		bool flag = false;
		if (!isDead && MVGameController.Instance.WOCM.AvatarLocal.IsDead)
		{
			flag = true;
			isDead = true;
		}
		else if (isDead && !MVGameController.Instance.WOCM.AvatarLocal.IsDead)
		{
			isDead = false;
		}
		if (!MVGameController.Instance.GameJoined)
		{
			_uxView.Hide();
		}
		else if (MVGameController.Instance.Game.IsPlaying)
		{
			if (flag && MVGameController.Instance.IsTouristSession && UXUtils.FindGUIObjectOfType<MVGUIRoot>().ShowRegisterMenuForTourist)
			{
				_uxView.Show();
				UXUtils.FindGUIObjectOfType<LockCursorManager>().ForceLoseFocus();
				return;
			}
			_uxView.Hide();
			if (!LockCursorManager.HasFocusAndLockCursor)
			{
				_uxView.Show();
			}
		}
		else
		{
			_uxView.Show();
		}
	}
}
