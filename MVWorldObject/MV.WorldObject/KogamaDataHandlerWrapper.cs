using System.Collections.Generic;
using MV.WorldObject.BytePackerFunctions;

namespace MV.WorldObject;

public static class KogamaDataHandlerWrapper
{
	public static Dictionary<object, object> GetDataParameters(int serializeVersion, BytePacker bp, KogamaDataType kogamaDataType, bool readRuntimeData)
	{
		if (serializeVersion == 11)
		{
			return BytePackerFunctionsVersion11.GetDataParameters(bp, kogamaDataType, readRuntimeData);
		}
		return null;
	}

	public static BytePacker GetBytePackerFromKogamaDataPackage(int serializeVersion, KogamaDataPackage kogamaDataPackage, bool writeRuntimeData)
	{
		if (serializeVersion == 11)
		{
			return BytePackerFunctionsVersion11.GetBytePackerFromKogamaDataPackage(kogamaDataPackage, writeRuntimeData);
		}
		return null;
	}
}
