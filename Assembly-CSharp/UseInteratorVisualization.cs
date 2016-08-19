using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UseInteratorVisualization : MonoBehaviour
{
	private const float scaleTime = 1f;

	private List<UseRequirement> useRequirements = new List<UseRequirement>();

	private Vector3 pivot = new Vector3(0f, 2.5f, 0f);

	private readonly float baseDist = 1.3f;

	private float dist = 0.01f;

	private float spacing = 120f;

	private int active;

	private CullingSubscriberBase cullingSubscriberBase;

	private bool hasUseRequirement;

	private int woId;

	private bool visible;

	public void Initialize(float yOffset, int woId)
	{
		this.woId = woId;
		pivot.y = yOffset;
		CalculateSpacing();
		enabled = false;
	}

	private void SetupCulling(int woId)
	{
		cullingSubscriberBase = new CullingSubscriberBase(OnStateChanged);
		cullingSubscriberBase.Radius = 2f;
		cullingSubscriberBase.DistanceBandIndex = 1;
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woId);
		worldObjectClient.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(worldObjectClient.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		UpdatePosition(worldObjectClient.WorldPosition);
	}

	private void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool flag = CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
		foreach (UseRequirement useRequirement in useRequirements)
		{
			if (useRequirement.IsActive())
			{
				useRequirement.GameObject.SetActive(flag);
			}
		}
		enabled = flag;
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		UpdatePosition(positionChangedEventArgs.NewPos);
	}

	private void UpdatePosition(Vector3 pos)
	{
		cullingSubscriberBase.Position = pos + pivot;
	}

	private void OnDestroy()
	{
		RemoveCulling();
	}

	private void RemoveCulling()
	{
		if (cullingSubscriberBase != null)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woId);
			worldObjectClient.PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Remove(worldObjectClient.PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
		enabled = false;
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
		hasUseRequirement = active != 0;
		float num = 0f;
		for (int j = 0; j < useRequirements.Count; j++)
		{
			if (useRequirements[j].IsActive())
			{
				useRequirements[j].CalculatePosAroundPivot(pivot, num, dist);
				num += spacing;
			}
		}
		CheckCullingSetup();
		if (!hasUseRequirement)
		{
			enabled = false;
		}
		else
		{
			enabled = true;
		}
	}

	private void Update()
	{
		transform.Rotate(Vector3.up, Time.deltaTime * 20f);
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			ChangeLOD((transform.position - MVGameControllerBase.WOCM.AvatarLocal.Transform.position).magnitude);
		}
	}

	private void ChangeLOD(float distance)
	{
		if (distance < 25f)
		{
			if (!visible)
			{
				Show();
			}
		}
		else if (visible)
		{
			Hide();
		}
	}

	public void Show()
	{
		StopAllCoroutines();
		StartCoroutine(pTween.To(1f, 0f, 1f, (float t) =>
		{
			Vector3 scale = new Vector3(t, t, t);
			for (int i = 0; i < useRequirements.Count; i++)
			{
				if (useRequirements[i].IsActive())
				{
					useRequirements[i].SetScale(scale);
				}
			}
		}));
		visible = true;
	}

	public void Hide()
	{
		StopAllCoroutines();
		StartCoroutine(pTween.To(1f, 1f, 0f, (float t) =>
		{
			Vector3 scale = new Vector3(t, t, t);
			for (int i = 0; i < useRequirements.Count; i++)
			{
				if (useRequirements[i].IsActive())
				{
					useRequirements[i].SetScale(scale);
				}
			}
		}));
		visible = false;
	}

	private void OnDisable()
	{
		Vector3 scale = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < useRequirements.Count; i++)
		{
			if (useRequirements[i].IsActive())
			{
				useRequirements[i].SetScale(scale);
			}
		}
	}

	private void OnEnable()
	{
		Vector3 scale = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < useRequirements.Count; i++)
		{
			if (useRequirements[i].IsActive())
			{
				useRequirements[i].SetScale(scale);
			}
		}
		Show();
	}

	public void UpdateData(Dictionary<object, object> data, int ownerID)
	{
		foreach (UseRequirement useRequirement in useRequirements)
		{
			useRequirement.OnDataUpdate(data, ownerID);
		}
		CalculateSpacing();
	}

	private void CheckCullingSetup()
	{
		if (hasUseRequirement && cullingSubscriberBase == null)
		{
			SetupCulling(woId);
		}
		else if (!hasUseRequirement && cullingSubscriberBase != null)
		{
			RemoveCulling();
		}
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
