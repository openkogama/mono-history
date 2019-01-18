using UnityEngine;

public class PendingFriendRequestAnimation : MonoBehaviour
{
	[SerializeField]
	private RectTransform maskTransform;

	[SerializeField]
	private RectTransform friendRequestImageTransform;

	[SerializeField]
	private float moveAmount;

	[SerializeField]
	private float dotAnimationCooldown;

	[SerializeField]
	private int dotAmount;

	private float lastDotAnimationTime;

	private int currentDot;

	private void Update()
	{
		if (Time.time > lastDotAnimationTime + dotAnimationCooldown)
		{
			if (currentDot == dotAmount)
			{
				MoveAnimation((0f - moveAmount) * (float)dotAmount);
				currentDot = 0;
			}
			else
			{
				MoveAnimation(moveAmount);
				currentDot++;
			}
			lastDotAnimationTime = Time.time;
		}
	}

	private void MoveAnimation(float amount)
	{
		Vector3 localPosition = maskTransform.localPosition;
		Vector3 localPosition2 = friendRequestImageTransform.localPosition;
		localPosition.x += amount;
		localPosition2.x -= amount;
		maskTransform.localPosition = localPosition;
		friendRequestImageTransform.localPosition = localPosition2;
	}
}
