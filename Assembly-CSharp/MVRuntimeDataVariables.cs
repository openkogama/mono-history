using System;
using System.Collections.Generic;

public class MVRuntimeDataVariables
{
	private MVWorldObjectClient owner;

	private List<MVRuntimeDataVariable> variables;

	public MVRuntimeDataVariables(MVWorldObjectClient owner)
	{
		this.owner = owner;
		variables = new List<MVRuntimeDataVariable>();
	}

	public MVRuntimeDataVariable New(string variableId, float sendInterval, bool writeThrough)
	{
		MVRuntimeDataVariable mVRuntimeDataVariable = new MVRuntimeDataVariable(variableId, sendInterval, owner.RunTimeData, writeThrough);
		mVRuntimeDataVariable.OnWriteThrough = (MVRuntimeDataVariable.OnWriteThroughDelegate)Delegate.Combine(mVRuntimeDataVariable.OnWriteThrough, new MVRuntimeDataVariable.OnWriteThroughDelegate(OnWriteThrough));
		variables.Add(mVRuntimeDataVariable);
		return mVRuntimeDataVariable;
	}

	public MVRuntimeDataVariable<T> New<T>(string variableId, float sendInterval, bool writeThrough)
	{
		MVRuntimeDataVariable<T> mVRuntimeDataVariable = new MVRuntimeDataVariable<T>(variableId, sendInterval, owner.RunTimeData, writeThrough);
		mVRuntimeDataVariable.OnWriteThrough = (MVRuntimeDataVariable.OnWriteThroughDelegate)Delegate.Combine(mVRuntimeDataVariable.OnWriteThrough, new MVRuntimeDataVariable.OnWriteThroughDelegate(OnWriteThrough));
		variables.Add(mVRuntimeDataVariable);
		return mVRuntimeDataVariable;
	}

	public MVRuntimeDataVariableClampedFloat NewClampedFloat(string variableId, float sendInterval, bool writeThrough, float minValue, float maxValue)
	{
		MVRuntimeDataVariableClampedFloat mVRuntimeDataVariableClampedFloat = new MVRuntimeDataVariableClampedFloat(variableId, sendInterval, owner.RunTimeData, writeThrough, minValue, maxValue);
		mVRuntimeDataVariableClampedFloat.OnWriteThrough = (MVRuntimeDataVariable.OnWriteThroughDelegate)Delegate.Combine(mVRuntimeDataVariableClampedFloat.OnWriteThrough, new MVRuntimeDataVariable.OnWriteThroughDelegate(OnWriteThrough));
		variables.Add(mVRuntimeDataVariableClampedFloat);
		return mVRuntimeDataVariableClampedFloat;
	}

	public void Receive(Dictionary<object, object> runtimeData)
	{
		foreach (MVRuntimeDataVariable variable in variables)
		{
			variable.Receive(runtimeData);
		}
	}

	public Dictionary<object, object> Send()
	{
		Dictionary<object, object> runtimeDataDelta = new Dictionary<object, object>();
		foreach (MVRuntimeDataVariable variable in variables)
		{
			variable.Send(ref runtimeDataDelta);
		}
		return (Dictionary<object, object>)ObscuredTypesConverter.CreateUnObscuredValue(runtimeDataDelta);
	}

	public void OnWriteThrough(object value)
	{
		if (owner.NetworkObject != null && owner.NetworkObject is MVNetworkReporter)
		{
			(owner.NetworkObject as MVNetworkReporter).SyncRunTimeDataVariables(MVGameController.Game);
		}
	}
}
