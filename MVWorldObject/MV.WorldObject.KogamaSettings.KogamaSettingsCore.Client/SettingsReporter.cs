using System;
using System.Collections.Generic;
using MV.Common;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.Client;

public class SettingsReporter
{
	public readonly MVWorldObject worldObject;

	protected Dictionary<object, object> DeltaData;

	protected Dictionary<object, object> DeltaRemovalData;

	private event Action<int, Dictionary<object, object>> partialDataUpdate;

	private event Action<int, Dictionary<object, object>> partialDataRemove;

	public event Action<Dictionary<object, object>> OnValueRemovedLocal;

	public event Action<Dictionary<object, object>> OnValueChangedLocal;

	public SettingsReporter(MVWorldObject worldObject, Action<int, Dictionary<object, object>> partialDataUpdate, Action<int, Dictionary<object, object>> partialDataRemove)
	{
		this.worldObject = worldObject;
		this.partialDataUpdate = partialDataUpdate;
		this.partialDataRemove = partialDataRemove;
	}

	public void Submit()
	{
		if (DeltaData != null)
		{
			partialDataUpdate(worldObject.Id, DeltaData);
			DeltaData = null;
		}
		if (DeltaRemovalData != null)
		{
			partialDataRemove(worldObject.Id, DeltaRemovalData);
			DeltaRemovalData = null;
		}
	}

	public void OnValueRemoved(Dictionary<object, object> deltaChange)
	{
		if (DeltaRemovalData == null)
		{
			DeltaRemovalData = new Dictionary<object, object>();
		}
		CommonUtils.PartialUpdateHashtable(DeltaRemovalData, deltaChange);
		CommonUtils.PartialRemoveFromHashtable(worldObject.Data, DeltaRemovalData);
		if (OnValueRemovedLocal != null)
		{
			OnValueRemovedLocal(DeltaRemovalData);
		}
	}

	public void OnValueChange(Dictionary<object, object> deltaChange)
	{
		if (DeltaData == null)
		{
			DeltaData = new Dictionary<object, object>();
		}
		CommonUtils.PartialUpdateHashtable(DeltaData, deltaChange);
		CommonUtils.PartialUpdateHashtable(worldObject.Data, deltaChange);
		if (OnValueChangedLocal != null)
		{
			OnValueChangedLocal(deltaChange);
		}
	}
}
