using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogRotator : MVGUISettingsDialog
{
	public MVGUISettingsDialogRotator()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/MovableSpeedSettingsDialog", TextSlotIndex.Rotator).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private void OnDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			float num = (float)dialog.GetResult();
			MVRotator mVRotator = wo as MVRotator;
			string keyPath = "BlueprintData\\AngularSpeed";
			MVGameController.Instance.Game.UpdateWorldObjectDataPartial(mVRotator.Id, keyPath, num);
		}
		MVGameController.Instance.EditorController.EditorStateMachine.DeSelectAll();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		MVRotator mVRotator = wo as MVRotator;
		dictionary.Add("SpeedSlider", new SliderData
		{
			sliderValue = mVRotator.AngularSpeed,
			setMinMaxValue = true,
			maxValue = 5f
		});
		return dictionary;
	}
}
