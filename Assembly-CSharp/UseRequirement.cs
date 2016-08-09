using System.Collections.Generic;
using UnityEngine;

public abstract class UseRequirement
{
	public abstract GameObject GameObject { get; }

	public abstract UseGUIResult GetCanUseGUIResult();

	public abstract void PayUseCost();

	public abstract void DestroyRequirement(Dictionary<object, object> data);

	public abstract void OnDataUpdate(Dictionary<object, object> data, int ownerID);

	public abstract ShowUseOption GetShowOption();

	public abstract int GetRequirementValue();

	public abstract UseRequirementType GetRequirementType();

	public abstract bool IsActive();

	public abstract void CalculatePosAroundPivot(Vector3 pivot, float spacingAngle, float distanceFromPivot);

	public abstract void SetScale(Vector3 scale);
}
