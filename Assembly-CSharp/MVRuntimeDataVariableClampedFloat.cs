using System.Collections;
using UnityEngine;

public class MVRuntimeDataVariableClampedFloat : MVRuntimeDataVariable<float>
{
	private float min;

	private float max;

	public new float Value
	{
		get
		{
			return base.Value;
		}
		set
		{
			base.Value = Mathf.Clamp(value, min, max);
		}
	}

	public MVRuntimeDataVariableClampedFloat(string variableId, float sendInterval, Hashtable initialRuntimeData, bool writeThrough, float min, float max)
		: base(variableId, sendInterval, initialRuntimeData, writeThrough)
	{
		this.min = min;
		this.max = max;
	}
}
