using System.Collections.Generic;
using MV.WorldObject;

public static class KogamaDataHandler
{
	public delegate void DataCallBack(Dictionary<object, object> data, KogamaDataType dataType);

	private static int serializeVersion = 11;

	private static int maxDeserializeTimeBeforeService = 1000;

	private static int timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);

	public static int GetKoGaMaData(BytePacker bp, DataCallBack callBack, bool readRuntimeData)
	{
		timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);
		GetPrototypeData(bp, callBack);
		int worldObjectData = GetWorldObjectData(bp, callBack, readRuntimeData);
		GetLinks(bp, callBack, readRuntimeData);
		GetObjectLinks(bp, callBack, readRuntimeData);
		return worldObjectData;
	}

	private static void GetPrototypeData(BytePacker bp, DataCallBack callBack)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.Prototypes, readRuntimeData: false);
			callBack(dataParameters, KogamaDataType.Prototypes);
			HandleService();
		}
	}

	private static void HandleService()
	{
		if (WaitForTicksLocal.Diff(timeSinceService) > maxDeserializeTimeBeforeService)
		{
			MVGameControllerBase.Game.Service();
			timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);
		}
	}

	private static int GetWorldObjectData(BytePacker bp, DataCallBack callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		int num2 = -1;
		for (int i = 0; i < num; i++)
		{
			Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.WorldObjects, readRuntimeData);
			if (num2 == -1)
			{
				num2 = (int)dataParameters[WorldObjectDataParameters.Id];
			}
			callBack(dataParameters, KogamaDataType.WorldObjects);
			HandleService();
		}
		return num2;
	}

	private static void GetLinks(BytePacker bp, DataCallBack callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.Links, readRuntimeData);
			callBack(dataParameters, KogamaDataType.Links);
			HandleService();
		}
	}

	private static void GetObjectLinks(BytePacker bp, DataCallBack callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.ObjectLinks, readRuntimeData);
			callBack(dataParameters, KogamaDataType.ObjectLinks);
			HandleService();
		}
	}
}
