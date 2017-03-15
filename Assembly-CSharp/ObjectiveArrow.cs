using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
	[SerializeField]
	private float animationSpeed;

	[SerializeField]
	private Vector3 arrowOffset;

	[SerializeField]
	private AnimationCurve bobbleCurve;

	private float distanceScale = 0.015f;

	private Vector3 initialScale;

	private Transform targetPickup;

	private Transform targetDropOff;

	private float initialHeight;

	private float animationTimer;

	private float farPlane;

	public void Initialize(Transform dropOff, Transform pickup)
	{
		initialScale = transform.localScale;
		targetDropOff = dropOff;
		targetPickup = pickup;
		transform.position = targetDropOff.position + arrowOffset;
		initialHeight = arrowOffset.y;
		farPlane = MVGameControllerBase.CameraController.MainCamera.farClipPlane;
		TranslateArrow();
	}

	public void OnPositionChanged(MVWorldObjectClient wo, PositionChangedEventArgs args)
	{
		transform.position = args.NewPos + arrowOffset;
	}

	private void Update()
	{
		animationTimer += animationSpeed * Time.deltaTime;
		TranslateArrow();
	}

	private void TranslateArrow()
	{
		Vector3 dir = targetDropOff.position - targetPickup.position;
		float num = Mathf.Min(dir.magnitude, farPlane - 100f);
		TranslateTowardsDirection(dir, num);
		SetScaleFromDistance(num);
	}

	private void TranslateTowardsDirection(Vector3 dir, float dist)
	{
		transform.position = targetPickup.position + dir.normalized * dist + arrowOffset;
		Vector3 localPosition = transform.localPosition;
		arrowOffset.y = bobbleCurve.Evaluate(animationTimer) * Mathf.Max(distanceScale * dist, 1f) + initialHeight;
		transform.localPosition = localPosition;
	}

	private void SetScaleFromDistance(float dist)
	{
		transform.localScale = initialScale + initialScale * distanceScale * dist;
	}
}
