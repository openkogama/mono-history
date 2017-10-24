using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;

public static class KoGaMaDataHandler
{
	private class AsyncBookkeeping
	{
		private static float workTime = 1f;

		private float workStartTime;

		private readonly UnityAction<int> doneCallback;

		public int deserializedPrototypes;

		public int deserializedWorldObjects;

		public int deserializedLinks;

		public int deserializedObjectLinks;

		public int numberOfPrototypes = -1;

		public int numberOfWorldObjects = -1;

		public int numberOfLinks = -1;

		public int numberOfObjectLinks = -1;

		public int rootId = -1;

		public readonly BytePacker bp;

		public readonly UnityAction<Dictionary<object, object>, KogamaDataType> callBack;

		public readonly bool readRuntimeData;

		public bool waitOneFrameBeforeDoneCallback = true;

		public bool WaitFrame
		{
			get
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (realtimeSinceStartup - workStartTime > workTime)
				{
					workStartTime = realtimeSinceStartup;
					return true;
				}
				return false;
			}
		}

		public AsyncBookkeeping(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData, UnityAction<int> doneCallback)
		{
			this.bp = bp;
			this.callBack = callBack;
			this.readRuntimeData = readRuntimeData;
			this.doneCallback = doneCallback;
			workStartTime = Time.realtimeSinceStartup;
		}

		public void Done()
		{
			doneCallback(rootId);
		}
	}

	private static AsyncBookkeeping asyncBookkeeping = null;

	private static int serializeVersion = 11;

	private static int maxDeserializeTimeBeforeService = 1000;

	private static int timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);

	public static int GetKoGaMaData(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);
		GetPrototypeData(bp, callBack);
		int worldObjectData = GetWorldObjectData(bp, callBack, readRuntimeData);
		GetLinks(bp, callBack, readRuntimeData);
		GetObjectLinks(bp, callBack, readRuntimeData);
		return worldObjectData;
	}

	private static void GetPrototypeData(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			DeserializePrototype(bp, callBack);
		}
	}

	private static int GetWorldObjectData(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		int num2 = -1;
		for (int i = 0; i < num; i++)
		{
			int num3 = DeserializeWorldObject(bp, callBack, readRuntimeData);
			if (num2 == -1)
			{
				num2 = num3;
			}
		}
		return num2;
	}

	private static void GetLinks(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			DeserializeLink(bp, callBack, readRuntimeData);
		}
	}

	private static void GetObjectLinks(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			DeserializeObjectLink(bp, callBack, readRuntimeData);
		}
	}

	public static void GetKoGaMaDataAsync(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData, UnityAction<int> doneCallback)
	{
		if (asyncBookkeeping != null)
		{
			Debug.LogError("Already doing async download.");
			return;
		}
		timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);
		asyncBookkeeping = new AsyncBookkeeping(bp, callBack, readRuntimeData, doneCallback);
		Execute();
	}

	private static bool GetPrototypeDataAsync()
	{
		if (asyncBookkeeping.numberOfPrototypes == -1)
		{
			asyncBookkeeping.numberOfPrototypes = asyncBookkeeping.bp.ReadInt32();
		}
		for (int i = asyncBookkeeping.deserializedPrototypes; i < asyncBookkeeping.numberOfPrototypes; i++)
		{
			DeserializePrototype(asyncBookkeeping.bp, asyncBookkeeping.callBack);
			asyncBookkeeping.deserializedPrototypes++;
			if (asyncBookkeeping.WaitFrame)
			{
				return false;
			}
		}
		return true;
	}

	private static bool GetWorldObjectDataAsync()
	{
		if (asyncBookkeeping.numberOfWorldObjects == -1)
		{
			asyncBookkeeping.numberOfWorldObjects = asyncBookkeeping.bp.ReadInt32();
		}
		for (int i = asyncBookkeeping.deserializedWorldObjects; i < asyncBookkeeping.numberOfWorldObjects; i++)
		{
			int rootId = DeserializeWorldObject(asyncBookkeeping.bp, asyncBookkeeping.callBack, asyncBookkeeping.readRuntimeData);
			if (asyncBookkeeping.rootId == -1)
			{
				asyncBookkeeping.rootId = rootId;
			}
			asyncBookkeeping.deserializedWorldObjects++;
			if (asyncBookkeeping.WaitFrame)
			{
				return false;
			}
		}
		return true;
	}

	private static bool GetLinksAsync()
	{
		if (asyncBookkeeping.numberOfLinks == -1)
		{
			asyncBookkeeping.numberOfLinks = asyncBookkeeping.bp.ReadInt32();
		}
		for (int i = asyncBookkeeping.deserializedLinks; i < asyncBookkeeping.numberOfLinks; i++)
		{
			DeserializeLink(asyncBookkeeping.bp, asyncBookkeeping.callBack, asyncBookkeeping.readRuntimeData);
			asyncBookkeeping.deserializedLinks++;
			if (asyncBookkeeping.WaitFrame)
			{
				return false;
			}
		}
		return true;
	}

	private static bool GetObjectLinksAsync()
	{
		if (asyncBookkeeping.numberOfObjectLinks == -1)
		{
			asyncBookkeeping.numberOfObjectLinks = asyncBookkeeping.bp.ReadInt32();
		}
		for (int i = asyncBookkeeping.deserializedObjectLinks; i < asyncBookkeeping.numberOfObjectLinks; i++)
		{
			DeserializeObjectLink(asyncBookkeeping.bp, asyncBookkeeping.callBack, asyncBookkeeping.readRuntimeData);
			asyncBookkeeping.deserializedObjectLinks++;
			if (asyncBookkeeping.WaitFrame)
			{
				return false;
			}
		}
		return true;
	}

	private static void Execute()
	{
		if (!GetPrototypeDataAsync())
		{
			Coroutines.Start(WaitForFrames.Frames(1, Execute));
		}
		else if (!GetWorldObjectDataAsync())
		{
			Coroutines.Start(WaitForFrames.Frames(1, Execute));
		}
		else if (!GetLinksAsync())
		{
			Coroutines.Start(WaitForFrames.Frames(1, Execute));
		}
		else if (!GetObjectLinksAsync())
		{
			Coroutines.Start(WaitForFrames.Frames(1, Execute));
		}
		else if (asyncBookkeeping.waitOneFrameBeforeDoneCallback)
		{
			asyncBookkeeping.waitOneFrameBeforeDoneCallback = false;
			Coroutines.Start(WaitForFrames.Frames(1, Execute));
		}
		else
		{
			asyncBookkeeping.Done();
			asyncBookkeeping = null;
		}
	}

	private static void DeserializePrototype(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack)
	{
		Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.Prototypes, readRuntimeData: false);
		callBack(dataParameters, KogamaDataType.Prototypes);
		HandleService();
	}

	private static int DeserializeWorldObject(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.WorldObjects, readRuntimeData);
		int result = (int)dataParameters[WorldObjectDataParameters.Id];
		callBack(dataParameters, KogamaDataType.WorldObjects);
		HandleService();
		return result;
	}

	private static void DeserializeLink(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.Links, readRuntimeData);
		callBack(dataParameters, KogamaDataType.Links);
		HandleService();
	}

	private static void DeserializeObjectLink(BytePacker bp, UnityAction<Dictionary<object, object>, KogamaDataType> callBack, bool readRuntimeData)
	{
		Dictionary<object, object> dataParameters = KogamaDataHandlerWrapper.GetDataParameters(serializeVersion, bp, KogamaDataType.ObjectLinks, readRuntimeData);
		callBack(dataParameters, KogamaDataType.ObjectLinks);
		HandleService();
	}

	private static void HandleService()
	{
		if (WaitForTicksLocal.Diff(timeSinceService) > maxDeserializeTimeBeforeService)
		{
			MVGameControllerBase.Game.Service();
			timeSinceService = WaitForTicksLocal.GetEnvironmentTick(0);
		}
	}
}
