using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSelectionAnimator : MonoBehaviour
{
	private static AvatarSelectionAnimator _instance;

	public List<MVBody> Bodies = new List<MVBody>();

	public MVSpawnPointRed bodySpawnPoint;

	public Vector3 displayPos;

	public Vector3 hidePos;

	public Quaternion displayRotation;

	private int currentIndex = -1;

	private int targetIndex = -1;

	private float time;

	private float baseTimeMultiplier = 2f;

	private Vector3 distance = Vector3.right * 8f;

	private bool forward = true;

	private float timeSlowThreshold = 0.8f;

	private float endmultiplier = 0.01f;

	private float addition;

	private float SuperspeedFactor = 1f;

	public static AvatarSelectionAnimator Instance
	{
		get
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected Obj, but got Unknown
			if ((Object)(object)_instance == (Object)null)
			{
				_instance = Object.FindObjectOfType(typeof(AvatarSelectionAnimator)) as AvatarSelectionAnimator;
			}
			if ((Object)(object)_instance == (Object)null)
			{
				GameObject val = new GameObject("AvatarSelectionAnimator");
				_instance = val.AddComponent(typeof(AvatarSelectionAnimator)) as AvatarSelectionAnimator;
			}
			return _instance;
		}
	}

	public AvatarSelectionAnimator()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		addition = (1f - endmultiplier * timeSlowThreshold) / (1f - timeSlowThreshold);
	}

	private void Update()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		if (targetIndex == -1 || currentIndex == -1)
		{
			return;
		}
		int num = 0;
		float num2 = 1f;
		SuperspeedFactor = 1f + (float)(Math.Abs(targetIndex - currentIndex) - 1);
		float num3 = baseTimeMultiplier * SuperspeedFactor;
		num = GetNextIndex();
		if (!forward)
		{
			num2 = -1f;
		}
		if (time > timeSlowThreshold && num == targetIndex)
		{
			num3 = baseTimeMultiplier * ((endmultiplier - addition) * time + addition);
		}
		time += Time.deltaTime * num3;
		Bodies[currentIndex].WorldPosition = displayPos + num2 * distance * time;
		Bodies[currentIndex].Scale = Vector3.one * (1f - time);
		Bodies[num].WorldPosition = displayPos + num2 * distance * time - num2 * distance;
		Bodies[num].Scale = Vector3.one * time;
		if (time > 1f)
		{
			Bodies[currentIndex].WorldPosition = hidePos;
			Bodies[currentIndex].Scale = Vector3.one;
			time = 0f;
			if (num == targetIndex)
			{
				Bodies[num].WorldPosition = displayPos;
				Bodies[num].Scale = Vector3.one;
				currentIndex = -1;
				targetIndex = -1;
				SuperspeedFactor = 1f;
			}
			else
			{
				currentIndex = num;
			}
		}
	}

	public void SetTargetIndex(int currentIndexInp, int TargetIndexInp)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (currentIndexInp == TargetIndexInp && targetIndex == -1)
		{
			Bodies[currentIndexInp].WorldPosition = displayPos;
			Bodies[currentIndexInp].Scale = Vector3.one;
			return;
		}
		targetIndex = TargetIndexInp;
		if (currentIndex == -1)
		{
			currentIndex = currentIndexInp;
		}
		bool flag = ((targetIndex <= currentIndex) ? (Bodies.Count - 1 - currentIndex + targetIndex) : (targetIndex - currentIndex)) < ((targetIndex >= currentIndex) ? (Bodies.Count - 1 - targetIndex + currentIndex) : (currentIndex - targetIndex));
		if (forward != flag)
		{
			currentIndex = GetNextIndex();
			forward = flag;
			time = 1f - time;
		}
	}

	public void SetTargetIndexNoAnim(int oldindex, int TargetIndexInp)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (currentIndex != -1)
		{
			Bodies[currentIndex].WorldPosition = hidePos;
			Bodies[currentIndex].Scale = Vector3.one;
		}
		Bodies[oldindex].WorldPosition = hidePos;
		Bodies[oldindex].Scale = Vector3.one;
		Bodies[TargetIndexInp].WorldPosition = displayPos;
		Bodies[TargetIndexInp].Scale = Vector3.one;
		currentIndex = -1;
		targetIndex = -1;
		time = 0f;
	}

	private int GetNextIndex()
	{
		int num = 0;
		if (!forward)
		{
			return (currentIndex != 0) ? (currentIndex - 1) : (Bodies.Count - 1);
		}
		return (currentIndex != Bodies.Count - 1) ? (currentIndex + 1) : 0;
	}
}
