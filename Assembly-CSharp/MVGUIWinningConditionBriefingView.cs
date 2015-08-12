using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIWinningConditionBriefingView : UXViewScript
{
	private const string _winningConditionPrefab = "Prefabs/GUI/WinnerScreen/WinningConditionBriefings/WinningCondition";

	private const string _winningConditionMaterialsFolder = "Materials/WinningConditionMaterials/";

	private List<MVGUIWinningCondition> winningConditions = new List<MVGUIWinningCondition>();

	private float fadeTime = 0.3f;

	private float stayTime = 3f;

	private float initialWaitTime = 1f;

	[SerializeField]
	private UXGroup group;

	[SerializeField]
	private Transform offset;

	private void Update()
	{
		Tests();
	}

	public void AddBriefing(string material, int limit)
	{
		AddBriefing(material, limit, showLimit: true);
	}

	public void AddBriefing(string material)
	{
		AddBriefing(material, 0, showLimit: false);
	}

	public override void OnShow()
	{
		if (winningConditions.Count != 0)
		{
			CalculateXPositions();
			group.SetVisible(visible: true);
			StopAllCoroutines();
			StartCoroutine(ShowBriefingCoroutine());
		}
	}

	public void Clear()
	{
		StopAllCoroutines();
		foreach (MVGUIWinningCondition winningCondition in winningConditions)
		{
			Object.Destroy(winningCondition.transform.gameObject);
		}
		winningConditions.Clear();
		group.SetVisible(visible: false);
	}

	private IEnumerator ShowBriefingCoroutine()
	{
		group.SetAlpha(0f, string.Empty);
		yield return StartCoroutine(Wait(initialWaitTime));
		yield return StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			group.SetAlpha(t, string.Empty);
		}));
		yield return StartCoroutine(Wait(stayTime));
		yield return StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			group.SetAlpha(t, string.Empty);
			if (t == 0f)
			{
				Clear();
			}
		}));
	}

	private IEnumerator Wait(float wait)
	{
		yield return new WaitForSeconds(wait);
	}

	private void AddBriefing(string materialName, int limit, bool showLimit)
	{
		MVGUIWinningCondition winningConditionBriefing = GetWinningConditionBriefing();
		Material material = (Material)Object.Instantiate(Resources.Load("Materials/WinningConditionMaterials/" + materialName));
		winningConditionBriefing.SetMaterial(material);
		if (showLimit)
		{
			winningConditionBriefing.SetLimit(limit);
		}
		else
		{
			winningConditionBriefing.HideLimit();
		}
		winningConditionBriefing.transform.parent = offset;
		winningConditionBriefing.transform.localPosition = new Vector3(0f, 0f, 0f);
		winningConditionBriefing.transform.localScale = Vector3.one;
		winningConditions.Add(winningConditionBriefing);
	}

	private static MVGUIWinningCondition GetWinningConditionBriefing()
	{
		return (Object.Instantiate(Resources.Load("Prefabs/GUI/WinnerScreen/WinningConditionBriefings/WinningCondition")) as GameObject).GetComponent<MVGUIWinningCondition>();
	}

	private void CalculateXPositions()
	{
		float num = 0f;
		float num2 = -0.2f;
		int num3 = 0;
		foreach (MVGUIWinningCondition winningCondition in winningConditions)
		{
			float x = winningCondition.Size.x;
			Vector3 localPosition = winningCondition.transform.localPosition;
			localPosition.x = num;
			winningCondition.transform.localPosition = localPosition;
			num += x + num2;
			num3++;
		}
		float num4 = num / (float)num3;
		num /= 2f;
		num -= num4 / 2f;
		foreach (MVGUIWinningCondition winningCondition2 in winningConditions)
		{
			Vector3 localPosition2 = winningCondition2.transform.localPosition;
			localPosition2.x -= num;
			winningCondition2.transform.localPosition = localPosition2;
		}
	}

	private void Tests()
	{
		if (MVInputWrapper.DebugGetKeyUp(KeyCode.O) && Application.isEditor)
		{
			AddBriefing("Kill", 10);
		}
		if (MVInputWrapper.DebugGetKeyUp(KeyCode.P) && Application.isEditor)
		{
			View.Show();
		}
		else if (MVInputWrapper.DebugGetKeyUp(KeyCode.I) && Application.isEditor)
		{
			Clear();
		}
	}
}
