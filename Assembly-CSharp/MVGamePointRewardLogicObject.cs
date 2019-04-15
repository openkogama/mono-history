using System.Collections.Generic;

public abstract class MVGamePointRewardLogicObject : MVLogicObject
{
	protected virtual int GamePointRewardAmount => GetGamePointsRewardAmount(Data);

	public string GamePointString => "gamePointAmount";

	public MVGamePointRewardLogicObject(Dictionary<object, object> data, ObjectPrefab prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		GamePointAmountManager.UpdateRewardData(Id, GamePointRewardAmount);
	}

	public override void OnDataUpdate()
	{
		GamePointAmountManager.UpdateRewardData(Id, GamePointRewardAmount);
		base.OnDataUpdate();
	}

	public override void Destroy()
	{
		GamePointAmountManager.UpdateRewardData(Id, 0);
		base.Destroy();
	}

	protected bool HasGamePoints(Dictionary<object, object> dataToCheck)
	{
		return dataToCheck.ContainsKey(GamePointString);
	}

	protected int GetGamePointsRewardAmount(Dictionary<object, object> data)
	{
		if (!HasGamePoints(data))
		{
			return 0;
		}
		return (int)data[GamePointString];
	}
}
