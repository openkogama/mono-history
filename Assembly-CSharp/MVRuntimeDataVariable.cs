using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class MVRuntimeDataVariable
{
	public delegate void OnChangeDelegate(object newValue);

	public delegate void OnWriteThroughDelegate(object newValue);

	private ObscuredString variableId;

	private object value;

	private object sendValue;

	private float sendInterval;

	private float lastSendTime = float.NegativeInfinity;

	private bool writeThrough;

	public OnChangeDelegate OnChange;

	public OnWriteThroughDelegate OnWriteThrough;

	public bool WriteThrough => writeThrough;

	public object Value
	{
		get
		{
			return ObscuredTypesConverter.CreateUnObscuredValue(value);
		}
		set
		{
			object obj = ObscuredTypesConverter.CreateObscuredValue(value);
			bool flag = this.value != obj;
			this.value = obj;
			if (flag)
			{
				NotifyChange();
			}
		}
	}

	public MVRuntimeDataVariable(string variableId, float sendInterval, Dictionary<object, object> initialRuntimeData, bool writeThrough)
	{
		this.variableId = variableId;
		this.sendInterval = sendInterval;
		this.writeThrough = writeThrough;
		if (!initialRuntimeData.ContainsObscuredKey(this.variableId))
		{
			Debug.LogError("Initial runtime data does not contain " + this.variableId);
		}
		value = initialRuntimeData[this.variableId];
		sendValue = value;
	}

	public void Receive(Dictionary<object, object> runtimeDataDelta)
	{
		string key = variableId.ToString();
		if (runtimeDataDelta.ContainsKey(key))
		{
			Value = runtimeDataDelta[key];
		}
	}

	public void Send(ref Dictionary<object, object> runtimeDataDelta, bool immediateSend)
	{
		if (!value.Equals(sendValue))
		{
			float time = Time.time;
			if (time > lastSendTime + sendInterval || immediateSend)
			{
				runtimeDataDelta[variableId] = value;
				sendValue = value;
				lastSendTime = time;
			}
		}
	}

	private void NotifyChange()
	{
		if (OnChange != null)
		{
			OnChange(Value);
		}
		if (writeThrough && OnWriteThrough != null)
		{
			OnWriteThrough(Value);
		}
	}
}
public class MVRuntimeDataVariable<T> : MVRuntimeDataVariable
{
	public new T Value
	{
		get
		{
			return (T)base.Value;
		}
		set
		{
			base.Value = value;
		}
	}

	public MVRuntimeDataVariable(string variableId, float sendInterval, Dictionary<object, object> initialRuntimeData, bool writeThrough)
		: base(variableId, sendInterval, initialRuntimeData, writeThrough)
	{
	}
}
