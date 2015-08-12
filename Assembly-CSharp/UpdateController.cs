using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MV.Common;
using UnityEngine;

public static class UpdateController
{
	[NotNull]
	private static readonly List<PriorityData>[] updateBuckets = new List<PriorityData>[5]
	{
		new List<PriorityData>(),
		new List<PriorityData>(),
		new List<PriorityData>(),
		new List<PriorityData>(),
		new List<PriorityData>()
	};

	[NotNull]
	private static readonly List<PriorityData>[] fixedUpdateBuckets = new List<PriorityData>[5]
	{
		new List<PriorityData>(),
		new List<PriorityData>(),
		new List<PriorityData>(),
		new List<PriorityData>(),
		new List<PriorityData>()
	};

	public static void AddUpdateObject(IUpdatecontrollerSubscriber obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityData item = new PriorityData
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		updateBuckets[(int)priority].Add(item);
	}

	public static void AddFixedUpdateObject(IUpdatecontrollerSubscriber obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityData item = new PriorityData
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		fixedUpdateBuckets[(int)priority].Add(item);
	}

	public static void RemoveObject(IUpdatecontrollerSubscriber obj)
	{
		RemoveUpdateObject(obj);
		RemoveFixedUpdateObject(obj);
	}

	public static void RemoveUpdateObject(IUpdatecontrollerSubscriber obj)
	{
		for (int i = 0; i < updateBuckets.Length; i++)
		{
			updateBuckets[i].RemoveAll((PriorityData x) => x.obj == obj);
		}
	}

	public static void RemoveFixedUpdateObject(IUpdatecontrollerSubscriber obj)
	{
		for (int i = 0; i < fixedUpdateBuckets.Length; i++)
		{
			fixedUpdateBuckets[i].RemoveAll((PriorityData x) => x.obj == obj);
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

	private static void UpdateList(int state, List<PriorityData> priorityDatas)
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

	private static void FixedUpdateList(int state, List<PriorityData> priorityDatas)
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
	}

	private static int GetPresentState()
	{
		int num = 0;
		num++;
		if (MVGameController.IngameController == null)
		{
			return num;
		}
		if (MVGameController.GameMode == MVGameMode.Edit)
		{
			num += 4;
		}
		if (MVGameController.GameMode == MVGameMode.Edit && (MVGameController.IngameController as EditorController).PlayInEditor)
		{
			num += 8;
		}
		if (MVGameController.GameMode == MVGameMode.Play)
		{
			num += 2;
		}
		return num;
	}
}
