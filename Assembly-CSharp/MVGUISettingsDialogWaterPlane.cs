using System.Collections.Generic;
using MV.WorldObject;

public class MVGUISettingsDialogWaterPlane : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogWaterPlane()
	{
		UXDialogFactory uXDialogFactory = dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/WaterPlaneSettingsDialog", TM._("Water Plane")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData());
		(uXDialogFactory.CurrentlyBuildingDialogBox as MVGUIWaterPlaneSettingsBox).IsPreset = wo.WorldObjectType == WorldObjectType.WaterPlanePreset;
		uXDialogFactory.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		float[] array = wo.Data["waterColor"] as float[];
		dictionary.Add("RedSlider", new SliderData
		{
			sliderValue = array[0]
		});
		dictionary.Add("GreenSlider", new SliderData
		{
			sliderValue = array[1]
		});
		dictionary.Add("BlueSlider", new SliderData
		{
			sliderValue = array[2]
		});
		if (!wo.Data.ContainsKey("avatarModifierPackageType"))
		{
			wo.Data.Add("avatarModifierPackageType", 0);
		}
		AvatarModifierPackageType avatarModifierPackageType = (AvatarModifierPackageType)(int)wo.Data["avatarModifierPackageType"];
		dictionary.Add("ModifierText", new TextData
		{
			text = avatarModifierPackageType.ToString()
		});
		return dictionary;
	}
}
