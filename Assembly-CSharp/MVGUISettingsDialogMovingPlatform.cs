using System.Collections.Generic;
using Localize;
using UnityEngine;

public class MVGUISettingsDialogMovingPlatform : MVGUISettingsDialog
{
	public MVGUISettingsDialogMovingPlatform()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/MovableSpeedSettingsDialog", TextSlotIndex.MovingPlatform).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private void OnDialogResult(UXDialogBox dialog)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			float num = (float)dialog.GetResult();
			MVMovingPlatformGroup mVMovingPlatformGroup = wo as MVMovingPlatformGroup;
			MVMovingPlatform platform = mVMovingPlatformGroup.Platform;
			Vector3 velocity = platform.Velocity;
			Vector3 normalized = velocity.normalized;
			float num2 = num;
			Vector3 vec = normalized * num2;
			string keyPath = "BlueprintData\\Velocity";
			MVGameController.Instance.Game.UpdateWorldObjectDataPartial(platform.Id, keyPath, vec.ToSerializeString());
		}
		MVGameController.Instance.EditorController.EditorStateMachine.DeSelectAll();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		MVMovingPlatformGroup mVMovingPlatformGroup = wo as MVMovingPlatformGroup;
		MVMovingPlatform platform = mVMovingPlatformGroup.Platform;
		SliderData sliderData = new SliderData();
		Vector3 velocity = platform.Velocity;
		sliderData.sliderValue = velocity.magnitude;
		sliderData.setMinMaxValue = true;
		sliderData.minValue = 0.3f;
		sliderData.maxValue = 3f;
		dictionary.Add("SpeedSlider", sliderData);
		return dictionary;
	}
}
