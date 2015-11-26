using System.Collections.Generic;
using UnityEngine;

public class UseInteratorVisualization : MonoBehaviour
{
	private List<UseRequirement> useRequirements = new List<UseRequirement>();

	private readonly Vector3 pivot = new Vector3(0f, 2.5f, 0f);

	private float rotation = 100f;

	private float baseDist = 1.3f;

	private void Update()
	{
		if (useRequirements.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (UseRequirement useRequirement in useRequirements)
		{
			if (useRequirement.IsActive())
			{
				num++;
			}
		}
		if (num == 0)
		{
			return;
		}
		float num2 = 360 / num;
		float num3 = 0f;
		float distanceFromPivot = baseDist;
		if (num == 1)
		{
			distanceFromPivot = 0.01f;
		}
		foreach (UseRequirement useRequirement2 in useRequirements)
		{
			if (useRequirement2.IsActive())
			{
				useRequirement2.CalculatePosAroundPivot(pivot, num3 + rotation, distanceFromPivot);
				num3 += num2;
			}
		}
		rotation += Time.deltaTime * 20f;
	}

	public void UpdateData(Dictionary<object, object> data, int ownerID)
	{
		foreach (UseRequirement useRequirement in useRequirements)
		{
			useRequirement.OnDataUpdate(data, ownerID);
		}
	}

	public void AddUseRequirement(UseRequirement useRequirement)
	{
		useRequirements.Add(useRequirement);
	}

	public UseGUIResult EvaluateUsability()
	{
		if (useRequirements.Count == 0)
		{
			return UseGUIResult.NoCost;
		}
		UseGUIResult useGUIResult = (UseGUIResult)0;
		foreach (UseRequirement useRequirement in useRequirements)
		{
			useGUIResult |= useRequirement.GetCanUseGUIResult();
		}
		return useGUIResult;
	}

	public ShowUseOption GetShowOptions()
	{
		if (useRequirements.Count == 0)
		{
			return ShowUseOption.Normal;
		}
		ShowUseOption showUseOption = ShowUseOption.Normal;
		foreach (UseRequirement useRequirement in useRequirements)
		{
			showUseOption |= useRequirement.GetShowOption();
		}
		return showUseOption;
	}

	public void PayUseCost()
	{
		foreach (UseRequirement useRequirement in useRequirements)
		{
			useRequirement.PayUseCost();
		}
	}

	public void DestroyRequirementObjects(Dictionary<object, object> data)
	{
		foreach (UseRequirement useRequirement in useRequirements)
		{
			useRequirement.DestroyRequirement(data);
		}
	}
}
