using System.Collections;
using MV.WorldObject;

public static class KogamaDataHandler
{
	public delegate void DataCallBack(Hashtable data, KogamaDataType dataType);

	private static int serializeVersion = 11;

	public static int GetKoGaMaData(BytePacker bp, DataCallBack callBack, bool readRuntimeData)
	{
		GetPrototypeData(bp, callBack);
		int worldObjectData = GetWorldObjectData(bp, callBack, readRuntimeData);
		GetLinks(bp, callBack);
		GetObjectLinks(bp, callBack);
		return worldObjectData;
	}

	private static void GetPrototypeData(BytePacker bp, DataCallBack callBack)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Hashtable dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.Prototypes, readRuntimeData: false);
			callBack(dataParameters, KogamaDataType.Prototypes);
		}
	}

	private static int GetWorldObjectData(BytePacker bp, DataCallBack callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		int num2 = -1;
		for (int i = 0; i < num; i++)
		{
			Hashtable dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.WorldObjects, readRuntimeData);
			if (num2 == -1)
			{
				num2 = (int)dataParameters[WorldObjectDataParameters.Id];
			}
			callBack(dataParameters, KogamaDataType.WorldObjects);
		}
		return num2;
	}

	private static void GetLinks(BytePacker bp, DataCallBack callBack)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Hashtable dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.Links, readRuntimeData: false);
			callBack(dataParameters, KogamaDataType.Links);
		}
	}

	private static void GetObjectLinks(BytePacker bp, DataCallBack callBack)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Hashtable dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.ObjectLinks, readRuntimeData: false);
			callBack(dataParameters, KogamaDataType.ObjectLinks);
		}
	}
}
