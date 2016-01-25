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
		for (int i = 0; i < useRequirements.Count; i++)
		{
			if (useRequirements[i].IsActive())
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
		for (int j = 0; j < useRequirements.Count; j++)
		{
			if (useRequirements[j].IsActive())
			{
				useRequirements[j].CalculatePosAroundPivot(pivot, num3 + rotation, distanceFromPivot);
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
		for (int i = 0; i < useRequirements.Count; i++)
		{
			useGUIResult |= useRequirements[i].GetCanUseGUIResult();
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
		for (int i = 0; i < useRequirements.Count; i++)
		{
			showUseOption |= useRequirements[i].GetShowOption();
		}
		return showUseOption;
	}

	public void PayUseCost()
	{
		for (int i = 0; i < useRequirements.Count; i++)
		{
			useRequirements[i].PayUseCost();
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
