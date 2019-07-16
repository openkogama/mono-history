using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.Client;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings;
using UnityEngine;

public class MVGameBoosterDataObject : MVWorldObjectClient
{
	private readonly SettingsReporter setttingsReporter;

	public GameBoosterSettingsManager GameBoosterSettingsManager => new GameBoosterSettingsManager(Data, setttingsReporter);

	public MVGameBoosterDataObject(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		Debug.Log(HashtableFunctions.PrettyString(Data));
		setttingsReporter = new SettingsReporter(this, PartialDataUpdate, PartialDataRemove);
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

	public override void Initialize()
	{
		Debug.LogWarning("TEST CODE");
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
