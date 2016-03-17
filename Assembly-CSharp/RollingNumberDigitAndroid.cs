using UnityEngine;

public class RollingNumberDigitAndroid : MonoBehaviour
{
	[SerializeField]
	private RectTransform rollingDigitTransform;

	private int targetNumber;

	private float firstNumPosY;

	private float digitSize = 6.8f;

	private float currPos;

	private float targetPosY;

	private float currNumPos;

	private float rollingSpeed = 6f;

	private float rollTimer;

	public int Number
	{
		get
		{
			return targetNumber;
		}
		set
		{
			rollTimer = 0f;
			if (targetNumber == 9 && value == 0)
			{
				currPos = firstNumPosY;
				Vector3 localPosition = rollingDigitTransform.localPosition;
				localPosition.y = currPos;
				rollingDigitTransform.localPosition = localPosition;
				targetNumber = -1;
			}
			currNumPos = (float)(targetNumber + 1) * digitSize - Mathf.Abs(firstNumPosY);
			Vector3 localPosition2 = rollingDigitTransform.localPosition;
			localPosition2.y = currNumPos;
			rollingDigitTransform.localPosition = localPosition2;
			targetNumber = Mathf.Clamp(value, 0, 9);
			targetPosY = (float)(targetNumber + 1) * digitSize - Mathf.Abs(firstNumPosY);
		}
	}

	private void Start()
	{
		firstNumPosY = rollingDigitTransform.localPosition.y;
		digitSize = rollingDigitTransform.rect.height / 11f;
		Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
		GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, digitSize);
		currPos = firstNumPosY + digitSize;
		targetPosY = currPos;
		currNumPos = currPos;
	}

	private void Update()
	{
		if (rollTimer >= 1f)
		{
			Vector3 localPosition = rollingDigitTransform.localPosition;
			localPosition.y = targetPosY;
			rollingDigitTransform.localPosition = localPosition;
		}
		else
		{
			rollTimer += rollingSpeed * Time.deltaTime;
			currPos = Mathf.Lerp(currNumPos, targetPosY, Mathf.Clamp01(rollTimer));
			Vector3 localPosition2 = rollingDigitTransform.localPosition;
			localPosition2.y = currPos;
			rollingDigitTransform.localPosition = localPosition2;
		}
	}
}
