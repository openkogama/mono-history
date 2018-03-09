using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

namespace MV.WorldObject.BytePackerFunctions;

public static class BytePackerFunctionsVersion11
{
	public static BytePacker GetBytePackerFromKogamaDataPackage(KogamaDataPackage kogamaDataPackage, bool writeRuntimeData)
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(kogamaDataPackage.prototypes.Count);
		foreach (MVPrototype value in kogamaDataPackage.prototypes.Values)
		{
			bytePacker.Write(value.ID);
			bytePacker.Write(value.Scale);
			bytePacker.Write(value.InsertedInWorldByProfileID);
			byte[] prototypeData = MVPrototype.GetPrototypeData((Dictionary<IntVector, byte[]>)value.Data[(byte)48]);
			bytePacker.Write(prototypeData.Length);
			bytePacker.Write(prototypeData);
		}
		List<MVWorldObject> woList = new List<MVWorldObject>();
		MVWorldObject mVWorldObject = kogamaDataPackage.worldObjects[kogamaDataPackage.rootIdWo];
		int numWO = 0;
		MVWorldObject.CallBackDelegate callBack = (MVWorldObject worldObject) =>
		{
			if (writeRuntimeData || IsSerializable(worldObject.Id, kogamaDataPackage.worldObjects))
			{
				woList.Add(worldObject);
				numWO++;
			}
		};
		mVWorldObject.TraverseRecursiveTail(callBack, kogamaDataPackage.worldObjects);
		bytePacker.Write(numWO);
		foreach (MVWorldObject item in woList)
		{
			bytePacker.Write(item.Id);
			bytePacker.Write(item.GroupId);
			bytePacker.Write(item.ItemId);
			bytePacker.Write((int)item.WorldObjectType);
			bytePacker.Write(item.Position.x);
			bytePacker.Write(item.Position.y);
			bytePacker.Write(item.Position.z);
			bytePacker.Write(item.Rotation.x);
			bytePacker.Write(item.Rotation.y);
			bytePacker.Write(item.Rotation.z);
			bytePacker.Write(item.Rotation.w);
			bytePacker.Write(item.Scale.x);
			bytePacker.Write(item.Scale.y);
			bytePacker.Write(item.Scale.z);
			WriteDataToBytePacker(bytePacker, item.Data);
			if (writeRuntimeData)
			{
				WriteWorldObjectOwnerState(bytePacker, item);
				WriteDataToBytePacker(bytePacker, item.RunTimeData);
			}
		}
		bytePacker.Write(kogamaDataPackage.links.Count);
		foreach (Link value2 in kogamaDataPackage.links.Values)
		{
			bytePacker.Write(value2.id);
			bytePacker.Write(value2.outputWOID);
			bytePacker.Write(value2.inputWOID);
		}
		bytePacker.Write(kogamaDataPackage.objectLinks.Count);
		foreach (ObjectLink value3 in kogamaDataPackage.objectLinks.Values)
		{
			bytePacker.Write(value3.id);
			bytePacker.Write(value3.objectConnectorWOID);
			bytePacker.Write(value3.objectWOID);
		}
		return bytePacker;
	}

	private static bool IsSerializable(int id, Dictionary<int, MVWorldObject> worldObjects)
	{
		if (worldObjects[id].OwnerActorNr != 0 || worldObjects[id].PreviewOwnerProfileId != 0)
		{
			return false;
		}
		if (worldObjects[id].GroupId != -1)
		{
			return IsSerializable(worldObjects[id].GroupId, worldObjects);
		}
		return true;
	}

	private static void WriteWorldObjectOwnerState(BytePacker bp, MVWorldObject wo)
	{
		byte b = 0;
		bp.Write(b);
		int position = bp.Position - 1;
		if (wo.OwnerActorNr != 0)
		{
			b |= 1;
			bp.Write(wo.OwnerActorNr);
		}
		if (wo.PreviewOwnerProfileId != 0)
		{
			b |= 2;
			bp.Write(wo.PreviewOwnerProfileId);
		}
		bp.Position = position;
		bp.Write(b);
		bp.Position = bp.Length;
	}

	private static void WriteDataToBytePacker(BytePacker bp, Dictionary<object, object> data)
	{
		bp.Write(data.Count);
		foreach (string key in data.Keys)
		{
			bp.Write(key);
			if (data[key] is int)
			{
				bp.Write((byte)0);
				bp.Write((int)data[key]);
				continue;
			}
			if (data[key] is int[])
			{
				bp.Write((byte)1);
				bp.Write(((int[])data[key]).Length);
				for (int i = 0; i < ((int[])data[key]).Length; i++)
				{
					bp.Write(((int[])data[key])[i]);
				}
				continue;
			}
			if (data[key] is float)
			{
				bp.Write((byte)2);
				bp.Write((float)data[key]);
				continue;
			}
			if (data[key] is float[])
			{
				bp.Write((byte)3);
				bp.Write(((float[])data[key]).Length);
				for (int j = 0; j < ((float[])data[key]).Length; j++)
				{
					bp.Write(((float[])data[key])[j]);
				}
				continue;
			}
			if (data[key] is Dictionary<object, object>)
			{
				bp.Write((byte)8);
				WriteDataToBytePacker(bp, (Dictionary<object, object>)data[key]);
				continue;
			}
			if (data[key] is bool)
			{
				bp.Write((byte)5);
				bp.Write((bool)data[key]);
				continue;
			}
			if (data[key] is bool[])
			{
				bp.Write((byte)6);
				bp.Write(((bool[])data[key]).Length);
				for (int k = 0; k < ((bool[])data[key]).Length; k++)
				{
					bp.Write(((bool[])data[key])[k]);
				}
				continue;
			}
			if (data[key] is string)
			{
				bp.Write((byte)7);
				bp.Write((string)data[key]);
				continue;
			}
			if (data[key] is byte)
			{
				bp.Write((byte)9);
				bp.Write((byte)data[key]);
				continue;
			}
			if (data[key] is long)
			{
				bp.Write((byte)10);
				bp.Write((long)data[key]);
				continue;
			}
			if (data[key] is long[])
			{
				bp.Write((byte)11);
				long[] array = (long[])data[key];
				bp.Write(array.Length);
				for (int l = 0; l < array.Length; l++)
				{
					bp.Write(array[l]);
				}
				continue;
			}
			string arg = ((data[key] != null) ? data[key].GetType().Name : "");
			throw new Exception($"Trying to write '{data[key]}' of unsupported type '{arg}' in WriteDataToBytePacker");
		}
	}

	public static Dictionary<object, object> GetDataParameters(BytePacker bp, KogamaDataType kogamaDataType, bool readRuntimeData)
	{
		return kogamaDataType switch
		{
			KogamaDataType.Prototypes => GetPrototypeDataParameters(bp), 
			KogamaDataType.WorldObjects => GetWorldObjectDataParameters(bp, readRuntimeData), 
			KogamaDataType.Links => GetLinkDataParameters(bp), 
			KogamaDataType.ObjectLinks => GetObjectLinkDataParameters(bp), 
			_ => null, 
		};
	}

	private static Dictionary<object, object> GetPrototypeDataParameters(BytePacker bp)
	{
		int num = bp.ReadInt32();
		float num2 = bp.ReadSingle();
		int num3 = bp.ReadInt32();
		int count = bp.ReadInt32();
		byte[] value = bp.ReadBytes(count);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(PrototypeDataParameters.Id, num);
		dictionary.Add(PrototypeDataParameters.Scale, num2);
		dictionary.Add(PrototypeDataParameters.Data, value);
		dictionary.Add(PrototypeDataParameters.AuthorProfileId, num3);
		return dictionary;
	}

	private static Dictionary<object, object> GetWorldObjectDataParameters(BytePacker bp, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		int num2 = bp.ReadInt32();
		int num3 = bp.ReadInt32();
		WorldObjectType worldObjectType = (WorldObjectType)bp.ReadInt32();
		Vector3 vector = new Vector3(bp.ReadSingle(), bp.ReadSingle(), bp.ReadSingle());
		Quaternion quaternion = new Quaternion(bp.ReadSingle(), bp.ReadSingle(), bp.ReadSingle(), bp.ReadSingle());
		Vector3 vector2 = new Vector3(bp.ReadSingle(), bp.ReadSingle(), bp.ReadSingle());
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		int numNameValuePairs = bp.ReadInt32();
		GetHashTableFromBytePacker(bp, dictionary, numNameValuePairs);
		Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
		dictionary2.Add(WorldObjectDataParameters.Id, num);
		dictionary2.Add(WorldObjectDataParameters.GroudId, num2);
		dictionary2.Add(WorldObjectDataParameters.ItemId, num3);
		dictionary2.Add(WorldObjectDataParameters.WorldObjectType, worldObjectType);
		dictionary2.Add(WorldObjectDataParameters.Position, vector);
		dictionary2.Add(WorldObjectDataParameters.Rotation, quaternion);
		dictionary2.Add(WorldObjectDataParameters.Scale, vector2);
		dictionary2.Add(WorldObjectDataParameters.Data, dictionary);
		Dictionary<object, object> dictionary3 = dictionary2;
		if (readRuntimeData)
		{
			ReadWorldObjectOwnerState(bp, dictionary3);
			int numNameValuePairs2 = bp.ReadInt32();
			Dictionary<object, object> dictionary4 = new Dictionary<object, object>();
			GetHashTableFromBytePacker(bp, dictionary4, numNameValuePairs2);
			dictionary3.Add(WorldObjectDataParameters.RuntimeData, dictionary4);
		}
		return dictionary3;
	}

	private static void ReadWorldObjectOwnerState(BytePacker bp, Dictionary<object, object> worldObjectData)
	{
		byte b = bp.ReadByte();
		if ((b & 1) != 0)
		{
			int num = bp.ReadInt32();
			worldObjectData.Add(WorldObjectDataParameters.OwnerActorNumber, num);
		}
		if ((b & 2) != 0)
		{
			int num2 = bp.ReadInt32();
			worldObjectData.Add(WorldObjectDataParameters.PreviewOwnerProfileId, num2);
		}
	}

	private static Dictionary<object, object> GetLinkDataParameters(BytePacker bp)
	{
		int num = bp.ReadInt32();
		int num2 = bp.ReadInt32();
		int num3 = bp.ReadInt32();
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(LinkDataParameter.Id, num);
		dictionary.Add(LinkDataParameter.OutputWOID, num2);
		dictionary.Add(LinkDataParameter.InputWOID, num3);
		return dictionary;
	}

	private static Dictionary<object, object> GetObjectLinkDataParameters(BytePacker bp)
	{
		int num = bp.ReadInt32();
		int num2 = bp.ReadInt32();
		int num3 = bp.ReadInt32();
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(ObjectLinkDataParameter.Id, num);
		dictionary.Add(ObjectLinkDataParameter.ObjectLinkConnectorWOID, num2);
		dictionary.Add(ObjectLinkDataParameter.ObjectWOID, num3);
		return dictionary;
	}

	private static void GetHashTableFromBytePacker(BytePacker bp, Dictionary<object, object> data, int numNameValuePairs)
	{
		for (int i = 0; i < numNameValuePairs; i++)
		{
			string key = bp.ReadString();
			HashtableDataType hashtableDataType = (HashtableDataType)bp.ReadByte();
			switch (hashtableDataType)
			{
			case HashtableDataType.Int32:
				data.Add(key, bp.ReadInt32());
				break;
			case HashtableDataType.Int32Array:
			{
				int num4 = bp.ReadInt32();
				int[] array4 = new int[num4];
				for (int m = 0; m < num4; m++)
				{
					array4[m] = bp.ReadInt32();
				}
				data.Add(key, array4);
				break;
			}
			case HashtableDataType.Single:
				data.Add(key, bp.ReadSingle());
				break;
			case HashtableDataType.SingleArray:
			{
				int num3 = bp.ReadInt32();
				float[] array3 = new float[num3];
				for (int l = 0; l < num3; l++)
				{
					array3[l] = bp.ReadSingle();
				}
				data.Add(key, array3);
				break;
			}
			case HashtableDataType.Int32HashtableKeysOnly:
			{
				int num5 = bp.ReadInt32();
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				for (int n = 0; n < num5; n++)
				{
					dictionary2.Add(bp.ReadInt32(), (byte)0);
				}
				data.Add(key, dictionary2);
				break;
			}
			case HashtableDataType.Bool:
				data.Add(key, bp.ReadBoolean());
				break;
			case HashtableDataType.BoolArray:
			{
				int num2 = bp.ReadInt32();
				bool[] array2 = new bool[num2];
				for (int k = 0; k < num2; k++)
				{
					array2[k] = bp.ReadBoolean();
				}
				data.Add(key, array2);
				break;
			}
			case HashtableDataType.String:
				data.Add(key, bp.ReadString());
				break;
			case HashtableDataType.Hashtable:
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				data.Add(key, dictionary);
				int numNameValuePairs2 = bp.ReadInt32();
				GetHashTableFromBytePacker(bp, dictionary, numNameValuePairs2);
				break;
			}
			case HashtableDataType.Byte:
				data.Add(key, bp.ReadByte());
				break;
			case HashtableDataType.Int64:
				data.Add(key, bp.ReadInt64());
				break;
			case HashtableDataType.Int64Array:
			{
				int num = bp.ReadInt32();
				long[] array = new long[num];
				for (int j = 0; j < num; j++)
				{
					array[j] = bp.ReadInt64();
				}
				data.Add(key, array);
				break;
			}
			default:
				throw new Exception(string.Concat("Trying to read unsupported type ", hashtableDataType, " from WriteDataToBytePacker "));
			}
		}
	}
}
