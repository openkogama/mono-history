using System.Collections.Generic;
using UnityEngine;

public class UseInteratorVisualization : MonoBehaviour
{
	private List<UseRequirement> useRequirements = new List<UseRequirement>();

	private Vector3 pivot = new Vector3(0f, 2.5f, 0f);

	private float rotation = 100f;

	private readonly float baseDist = 1.3f;

	private float dist = 0.01f;

	private float spacing = 120f;

	private int active;

	public void Initialize(float yOffset)
	{
		pivot.y = yOffset;
		CalculateSpacing();
	}

	private void CalculateSpacing()
	{
		active = 0;
		dist = baseDist;
		for (int i = 0; i < useRequirements.Count; i++)
		{
			if (useRequirements[i].IsActive())
			{
				active++;
			}
		}
		if (active == 1)
		{
			dist = 0.01f;
		}
		if (active != 0)
		{
			spacing = 360 / active;
		}
		enabled = active != 0;
	}

	private void Update()
	{
		float num = 0f;
		for (int i = 0; i < useRequirements.Count; i++)
		{
			if (useRequirements[i].IsActive())
			{
				useRequirements[i].CalculatePosAroundPivot(pivot, num + rotation, dist);
				num += spacing;
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
		CalculateSpacing();
	}

	public void AddUseRequirement(UseRequirement useRequirement)
	{
		useRequirements.Add(useRequirement);
		CalculateSpacing();
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
