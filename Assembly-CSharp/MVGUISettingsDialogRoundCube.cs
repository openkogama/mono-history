using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUISettingsDialogRoundCube : MVGUIDynamicSettingsDialog
{
	public static List<GameStatCounterLocalized> availableGameStatCounters = new List<GameStatCounterLocalized>
	{
		new GameStatCounterLocalized(GameStatCounterType.None, TM._("None")),
		new GameStatCounterLocalized(GameStatCounterType.YDown, TM._("Reach Lowest Altitude")),
		new GameStatCounterLocalized(GameStatCounterType.YUp, TM._("Reach Highest Altitude"))
	};

	public MVGUISettingsDialogRoundCube()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/RoundCubeSettingsDialog", TM._("Round Cube")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("intervalSlider", new SliderData
		{
			sliderValue = (int)wo.Data["interval"]
		});
		string[] array = new string[availableGameStatCounters.Count];
		for (int i = 0; i < availableGameStatCounters.Count; i++)
		{
			array[i] = availableGameStatCounters[i].txt;
		}
		Debug.Log(wo.Data["winningCondition"]);
		int currentlySelectedIndex = 0;
		try
		{
			currentlySelectedIndex = GetCurrentSelectedIndex((int)wo.Data["winningCondition"]);
		}
		catch (Exception)
		{
			Debug.LogWarning("Ignoring out of bounds index");
		}
		dictionary.Add("winningConditionComboBox", new TextComboBoxData
		{
			items = array,
			currentlySelectedIndex = currentlySelectedIndex
		});
		return dictionary;
	}

	private int GetCurrentSelectedIndex(int gameStatCounterID)
	{
		for (int i = 0; i < availableGameStatCounters.Count; i++)
		{
			if (gameStatCounterID == (int)availableGameStatCounters[i].gameStatCounterType)
			{
				return i;
			}
		}
		throw new Exception("Did not find index");
	}
}
