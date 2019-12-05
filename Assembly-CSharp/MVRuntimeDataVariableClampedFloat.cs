using System.Collections.Generic;
using UnityEngine;

public class MVRuntimeDataVariableClampedFloat : MVRuntimeDataVariable<float>
{
	private float min;

	private float max;

	public override float Value
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

	public MVRuntimeDataVariableClampedFloat(string variableId, float sendInterval, Dictionary<object, object> initialRuntimeData, bool writeThrough, float min, float max)
		: base(variableId, sendInterval, initialRuntimeData, writeThrough)
	{
		this.min = min;
		this.max = max;
	}
}
