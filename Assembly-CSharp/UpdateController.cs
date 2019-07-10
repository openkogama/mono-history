using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public static class UpdateController
{
	private static readonly List<PriorityDataUpdate>[] updateBuckets = new List<PriorityDataUpdate>[5]
	{
		new List<PriorityDataUpdate>(),
		new List<PriorityDataUpdate>(),
		new List<PriorityDataUpdate>(),
		new List<PriorityDataUpdate>(),
		new List<PriorityDataUpdate>()
	};

	private static readonly List<PriorityDataFixedUpdate>[] fixedUpdateBuckets = new List<PriorityDataFixedUpdate>[5]
	{
		new List<PriorityDataFixedUpdate>(),
		new List<PriorityDataFixedUpdate>(),
		new List<PriorityDataFixedUpdate>(),
		new List<PriorityDataFixedUpdate>(),
		new List<PriorityDataFixedUpdate>()
	};

	private static readonly List<PriorityDataLateUpdate>[] lateUpdateBuckets = new List<PriorityDataLateUpdate>[5]
	{
		new List<PriorityDataLateUpdate>(),
		new List<PriorityDataLateUpdate>(),
		new List<PriorityDataLateUpdate>(),
		new List<PriorityDataLateUpdate>(),
		new List<PriorityDataLateUpdate>()
	};

	public static void AddUpdateObject(IUpdatecontrollerSubscriberUpdate obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityDataUpdate item = new PriorityDataUpdate
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		updateBuckets[(int)priority].Add(item);
	}

	public static void AddFixedUpdateObject(IUpdatecontrollerSubscriberFixedUpdate obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityDataFixedUpdate item = new PriorityDataFixedUpdate
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		fixedUpdateBuckets[(int)priority].Add(item);
	}

	public static void AddLateUpdateObject(IUpdatecontrollerSubscriberLateUpdate obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityDataLateUpdate item = new PriorityDataLateUpdate
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		lateUpdateBuckets[(int)priority].Add(item);
	}

	public static void RemoveUpdateObject(IUpdatecontrollerSubscriberUpdate obj)
	{
		for (int i = 0; i < updateBuckets.Length; i++)
		{
			updateBuckets[i].RemoveAll((PriorityDataUpdate x) => x.obj == obj);
		}
	}

	public static void RemoveFixedUpdateObject(IUpdatecontrollerSubscriberFixedUpdate obj)
	{
		for (int i = 0; i < fixedUpdateBuckets.Length; i++)
		{
			fixedUpdateBuckets[i].RemoveAll((PriorityDataFixedUpdate x) => x.obj == obj);
		}
	}

	public static void RemoveLateUpdateObject(IUpdatecontrollerSubscriberLateUpdate obj)
	{
		for (int i = 0; i < lateUpdateBuckets.Length; i++)
		{
			lateUpdateBuckets[i].RemoveAll((PriorityDataLateUpdate x) => x.obj == obj);
		}
	}

	public static void Update()
	{
		int presentState = GetPresentState();
		for (int i = 0; i < updateBuckets.Length; i++)
		{
			UpdateList(presentState, updateBuckets[i]);
		}
	}

	private static void UpdateList(int state, List<PriorityDataUpdate> priorityDatas)
	{
		for (int i = 0; i < priorityDatas.Count; i++)
		{
			if (priorityDatas[i].obj == null || (state & priorityDatas[i].condition) <= 0)
			{
				continue;
			}
			try
			{
				priorityDatas[i].obj.UpdateControllerUpdate();
			}
			catch (Exception ex)
			{
				if (Application.isEditor)
				{
					throw;
				}
				Debug.LogError("Exception in UpdateControllerUpdate: " + ex.ToString());
			}
		}
	}

	public static void FixedUpdate()
	{
		int presentState = GetPresentState();
		for (int i = 0; i < fixedUpdateBuckets.Length; i++)
		{
			FixedUpdateList(presentState, fixedUpdateBuckets[i]);
		}
	}

	private static void FixedUpdateList(int state, List<PriorityDataFixedUpdate> priorityDatas)
	{
		for (int i = 0; i < priorityDatas.Count; i++)
		{
			if ((state & priorityDatas[i].condition) <= 0)
			{
				continue;
			}
			try
			{
				priorityDatas[i].obj.UpdateControllerFixedUpdate();
			}
			catch (Exception ex)
			{
				if (Application.isEditor)
				{
					throw ex;
				}
				Debug.LogError("Exception in UpdateControllerFixedUpdate: " + ex.ToString());
			}
		}
	}

	public static void LateUpdate()
	{
		int presentState = GetPresentState();
		for (int i = 0; i < lateUpdateBuckets.Length; i++)
		{
			LateUpdateList(presentState, lateUpdateBuckets[i]);
		}
	}

	private static void LateUpdateList(int state, List<PriorityDataLateUpdate> priorityDatas)
	{
		for (int i = 0; i < priorityDatas.Count; i++)
		{
			if ((state & priorityDatas[i].condition) <= 0)
			{
				continue;
			}
			try
			{
				priorityDatas[i].obj.UpdateControllerLateUpdate();
			}
			catch (Exception ex)
			{
				if (Application.isEditor)
				{
					throw ex;
				}
				Debug.LogError("Exception in UpdateControllerLateUpdate: " + ex.ToString());
			}
		}
	}

	public static void Clear()
	{
		for (int i = 0; i < updateBuckets.Length; i++)
		{
			updateBuckets[i].Clear();
		}
		for (int j = 0; j < fixedUpdateBuckets.Length; j++)
		{
			fixedUpdateBuckets[j].Clear();
		}
		for (int k = 0; k < lateUpdateBuckets.Length; k++)
		{
			lateUpdateBuckets[k].Clear();
		}
	}

	private static int GetPresentState()
	{
		int num = 1;
		if (!MVGameControllerBase.IsInitialized)
		{
			return num;
		}
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			num += 4;
		}
		if (MVGameControllerBase.GameMode == MVGameMode.Edit && MVGameControllerBase.Game.IsPlaying)
		{
			num += 8;
		}
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			num += 2;
		}
		return num;
	}
}
