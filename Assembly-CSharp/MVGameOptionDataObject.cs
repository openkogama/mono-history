using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.Client;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameOptions;
using UnityEngine;

public class MVGameOptionDataObject : MVWorldObjectClient
{
	private readonly SettingsReporter setttingsReporter;

	private readonly SettingsManager settingsManager;

	public GameBoosterSettingsManager GameBoosterSettingsManager => new GameBoosterSettingsManager(Data);

	public GameOptionSettingsManager GameOptionSettingsManager => new GameOptionSettingsManager(Data);

	public MVGameOptionDataObject(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		Debug.Log(HashtableFunctions.PrettyString(Data));
		setttingsReporter = new SettingsReporter(this, PartialDataUpdate, PartialDataRemove);
		settingsManager = new SettingsManager(setttingsReporter);
	}

	public void UpdateSetting(KogamaSettingWrapperBase obj)
	{
		settingsManager.UpdateSetting(obj);
	}

	public void RemoveSetting(KogamaSettingWrapperBase obj)
	{
		settingsManager.RemoveSetting(obj);
	}

	public void Submit()
	{
		settingsManager.Submit();
		CommonUtils.PruneEmptyDictionaries(Data);
	}

	public override void PartialUpdateWOData(Dictionary<object, object> woData)
	{
		Debug.Log("woData\n" + HashtableFunctions.PrettyString(woData));
		Debug.Log("data before\n" + HashtableFunctions.PrettyString(Data));
		base.PartialUpdateWOData(woData);
		Debug.Log("data after\n" + HashtableFunctions.PrettyString(Data));
	}

	public override void PartialRemoveFromWOData(Dictionary<object, object> entriesToRemove)
	{
		Debug.Log("entriesToRemove\n" + HashtableFunctions.PrettyString(entriesToRemove));
		Debug.Log("data before\n" + HashtableFunctions.PrettyString(Data));
		CommonUtils.PartialRemoveFromHashtable(Data, entriesToRemove, acceptMissingValuesInTarget: true);
		OnDataUpdate();
		Debug.Log("data after\n" + HashtableFunctions.PrettyString(Data));
	}

	private void PartialDataRemove(int arg1, Dictionary<object, object> arg2)
	{
		MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(arg1, arg2);
	}

	private void PartialDataUpdate(int arg1, Dictionary<object, object> arg2)
	{
		MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(arg1, arg2);
	}
}
