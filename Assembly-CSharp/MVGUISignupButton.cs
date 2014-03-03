using Localize;
using UnityEngine;

public class MVGUISignupButton : MonoBehaviour
{
	[SerializeField]
	private UXBaseButton _baseButton;

	[SerializeField]
	private string _externalCallFunction = "gotoRegisterForm";

	[SerializeField]
	private string _externalCallArg = "play";

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
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateCustomDialog("Prefabs/GUI/Dialogs/PublishDialog", TextSlotIndex.PublishPlanet).AddPositiveButton(TextSlotIndex.Confirm)
			.AddNegativeButton(TextSlotIndex.Reject)
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
		Application.ExternalCall(_externalCallFunction, new object[1] { _externalCallArg });
	}
}
