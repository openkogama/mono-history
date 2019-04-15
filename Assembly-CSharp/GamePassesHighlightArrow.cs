using UnityEngine;

public class GamePassesHighlightArrow : MonoBehaviour
{
	[SerializeField]
	private RectTransform transformToMove;

	[SerializeField]
	private AnimationCurve moveCurve;

	[SerializeField]
	private float moveAmount;

	[SerializeField]
	private float directionX;

	[SerializeField]
	private float directionY;

	private float moveStartTime;

	private Vector3 startPosition;

	private void Start()
	{
		startPosition = transformToMove.localPosition;
		moveStartTime = Time.time;
	}

	private void OnEnable()
	{
		moveStartTime = Time.time;
	}

	private void Update()
	{
		Vector3 localPosition = transformToMove.localPosition;
		localPosition.y = startPosition.y + moveAmount * moveCurve.Evaluate(Time.time - moveStartTime) * directionX;
		localPosition.x = startPosition.x + moveAmount * moveCurve.Evaluate(Time.time - moveStartTime) * directionY;
		transformToMove.localPosition = localPosition;
	}
}
