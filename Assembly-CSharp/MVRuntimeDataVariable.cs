using System.Collections;
using UnityEngine;

public class MVRuntimeDataVariable
{
	public delegate void OnChangeDelegate(object newValue);

	public delegate void OnWriteThroughDelegate(object newValue);

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(MVRuntimeDataVariable));

	private string variableId;

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
			return value;
		}
		set
		{
			bool flag = this.value != value;
			this.value = value;
			if (flag)
			{
				NotifyChange();
			}
		}
	}

	public MVRuntimeDataVariable(string variableId, float sendInterval, Hashtable initialRuntimeData, bool writeThrough)
	{
		this.variableId = variableId;
		this.sendInterval = sendInterval;
		this.writeThrough = writeThrough;
		if (!initialRuntimeData.ContainsKey(variableId))
		{
			Debug.LogError((object)("Initial runtime data does not contain " + variableId));
		}
		value = initialRuntimeData[variableId];
		sendValue = value;
	}

	public void Receive(Hashtable runtimeDataDelta)
	{
		if (runtimeDataDelta.ContainsKey(variableId))
		{
			object obj = runtimeDataDelta[variableId];
			Value = obj;
		}
	}

	public void Send(Hashtable runtimeDataDelta)
	{
		if (!value.Equals(sendValue))
		{
			float time = Time.time;
			if (time > lastSendTime + sendInterval)
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
			OnChange(value);
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

	public MVRuntimeDataVariable(string variableId, float sendInterval, Hashtable initialRuntimeData, bool writeThrough)
		: base(variableId, sendInterval, initialRuntimeData, writeThrough)
	{
	}
}
