using System.Collections.Generic;
using MV.Common;

public class MVGUISettingsDialogCameraSettings : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogCameraSettings()
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/CameraSettingsDialogClassic", TM._("Camera Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
				.SetOnResultCallback(OnDialogResult)
				.SetOnIntermediateResultCallback(OnIntermediateResult)
				.SetValues(BuildDialogData())
				.Show();
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/CameraSettingsDialogPlatformer", TM._("Camera Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
				.SetOnResultCallback(OnDialogResult)
				.SetOnIntermediateResultCallback(OnIntermediateResult)
				.SetValues(BuildDialogData())
				.Show();
		}
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			dictionary.Add("DistanceToAvatarSlider", new SliderData
			{
				sliderValue = (float)wo.Data["distanceToAvatar"]
			});
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			dictionary.Add("DistanceToAvatarSlider", new SliderData
			{
				sliderValue = (float)wo.Data["distanceToAvatar"]
			});
		}
		return dictionary;
	}
}
