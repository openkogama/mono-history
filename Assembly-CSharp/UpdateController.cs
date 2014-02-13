using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class UpdateController
{
	private List<PriorityData> UpdateList = new List<PriorityData>();

	private List<PriorityData> FixedUpdateList = new List<PriorityData>();

	public void AddUpdateObject(IUpdatecontrollerSubscriber obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityData item = new PriorityData
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		UpdateList.Add(item);
		UpdateList.Sort((PriorityData a, PriorityData b) => a.priority.CompareTo(b.priority));
	}

	public void AddFixedUpdateObject(IUpdatecontrollerSubscriber obj, UpdatePriority priority, int conditionInp = 1)
	{
		PriorityData item = new PriorityData
		{
			obj = obj,
			priority = priority,
			condition = conditionInp
		};
		FixedUpdateList.Add(item);
		FixedUpdateList.Sort((PriorityData a, PriorityData b) => a.priority.CompareTo(b.priority));
	}

	public void RemoveObject(IUpdatecontrollerSubscriber obj)
	{
		RemoveUpdateObject(obj);
		RemoveFixedUpdateObject(obj);
	}

	public void RemoveUpdateObject(IUpdatecontrollerSubscriber obj)
	{
		UpdateList.RemoveAll((PriorityData x) => x.obj == obj);
	}

	public void RemoveFixedUpdateObject(IUpdatecontrollerSubscriber obj)
	{
		FixedUpdateList.RemoveAll((PriorityData x) => x.obj == obj);
	}

	public void Update()
	{
		int presentState = GetPresentState();
		UpdateList.RemoveAll((PriorityData pd) => pd.obj == null);
		for (int num = 0; num < UpdateList.Count; num++)
		{
			if (UpdateList[num].obj == null || (presentState & UpdateList[num].condition) <= 0)
			{
				continue;
			}
			try
			{
				UpdateList[num].obj.UpdateControllerUpdate();
			}
			catch (Exception ex)
			{
				if (Application.isEditor)
				{
					throw ex;
				}
				Debug.LogError((object)("Exception in UpdateControllerUpdate: " + ex.ToString()));
			}
		}
	}

	public void FixedUpdate()
	{
		int presentState = GetPresentState();
		FixedUpdateList.RemoveAll((PriorityData pd) => pd.obj == null);
		for (int num = 0; num < FixedUpdateList.Count; num++)
		{
			if ((presentState & FixedUpdateList[num].condition) <= 0)
			{
				continue;
			}
			try
			{
				FixedUpdateList[num].obj.UpdateControllerFixedUpdate();
			}
			catch (Exception ex)
			{
				if (Application.isEditor)
				{
					throw ex;
				}
				Debug.LogError((object)("Exception in UpdateControllerFixedUpdate: " + ex.ToString()));
			}
		}
	}

	private int GetPresentState()
	{
		int num = 0;
		num++;
		if (MVGameController.Instance.IngameController == null)
		{
			return num;
		}
		if (MVGameController.Instance.GameMode == MVGameMode.Edit)
		{
			num += 4;
		}
		if (MVGameController.Instance.GameMode == MVGameMode.Edit && (MVGameController.Instance.IngameController as AEditController).PlayInEditor)
		{
			num += 8;
		}
		if (MVGameController.Instance.GameMode == MVGameMode.Play)
		{
			num += 2;
		}
		return num;
	}
}
