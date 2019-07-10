using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject.GamePassSystem.GamePassProgressionDataObject;
using Newtonsoft.Json;
using UnityEngine;

public class MVGamePassProgressionDataObject : MVWorldObjectClient
{
	public const string gamePassProgressionDataObjectKey = "gamePassProgressionDataObject";

	public const string gamePassProgressionDataObjectValidationKey = "gamePassProgressionDataObjectValidation";

	public const string gamePassProgressionEnabledKey = "gamePassProgressionEnabled";

	public GamePassProgressionDataObjectShared GamePassProgressionDataObjectShared
	{
		get
		{
			return JsonConvert.DeserializeObject<GamePassProgressionDataObjectShared>((string)Data["gamePassProgressionDataObject"]);
		}
		set
		{
			string value2 = JsonConvert.SerializeObject(value);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("gamePassProgressionDataObject", value2);
			Dictionary<object, object> dictionary2 = dictionary;
			PartialUpdateWOData(dictionary2);
			MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(Id, dictionary2);
		}
	}

	public GamePassProgressionDataObjectSharedValidator GamePassProgressionDataObjectSharedValidator
	{
		get
		{
			if (!RunTimeData.ContainsObscuredKey("gamePassProgressionDataObjectValidation"))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<GamePassProgressionDataObjectSharedValidator>((ObscuredString)RunTimeData.GetObscuredType("gamePassProgressionDataObjectValidation"));
		}
	}

	public bool EnableProgression => (bool)Data["gamePassProgressionEnabled"];

	public MVGamePassProgressionDataObject(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		Debug.Log(this);
		if (GamePassProgressionController.OnGamePassesProgressionUpdate != null)
		{
			GamePassProgressionController.OnGamePassesProgressionUpdate();
		}
	}
}
