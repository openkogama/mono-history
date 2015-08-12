using UnityEngine;

public class MVGUISignupButton : MonoBehaviour
{
	[SerializeField]
	private UXBaseButton _baseButton;

	[SerializeField]
	private bool confirmationPopup;

	private void Start()
	{
		_baseButton.OnClick = () =>
		{
			if (confirmationPopup)
			{
				DoConfirmationPopup();
			}
			else
			{
				DoExternalCall();
			}
		};
	}

	private void DoConfirmationPopup()
	{
		UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/PublishDialog", TM._("Publish Your Game")).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
			.SetOnResultCallback(OnButtonOnClick)
			.Show();
	}

	private void OnButtonOnClick(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			DoExternalCall();
		}
	}

	private void DoExternalCall()
	{
		BrowserComm.ToJavaScript.ExternalCall("gotoSignup");
		BrowserComm.ExecuteBrowserRequest(MVGameController.GameSessionData.signupURL);
	}
}
