using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
	[SerializeField]
	private float animationSpeed;

	[SerializeField]
	private Vector3 arrowOffset;

	[SerializeField]
	private AnimationCurve bobbleCurve;

	[SerializeField]
	private AnimationCurve animationSpawnCurve;

	[SerializeField]
	private float animationLerpTime = 2f;

	[SerializeField]
	private float distanceScale = 0.015f;

	private Vector3 initialScale;

	private Vector3 startPos;

	private Transform targetPickup;

	private Transform targetDropOff;

	private float initialHeight;

	private float farPlane;

	private float animationTimer;

	public void Initialize(Vector3 startPos, Transform dropOff, Transform pickup)
	{
		this.startPos = startPos;
		initialScale = transform.localScale;
		targetDropOff = dropOff;
		targetPickup = pickup;
		initialHeight = arrowOffset.y;
		farPlane = MVGameControllerBase.CameraController.MainCamera.farClipPlane;
		float arrowBobbing = Mathf.Min((targetDropOff.position - targetPickup.position).magnitude, farPlane - 100f);
		SetArrowBobbing(arrowBobbing);
		transform.position = startPos + arrowOffset;
	}

	public void OnPositionChanged(MVWorldObjectClient wo, PositionChangedEventArgs args)
	{
		transform.position = args.NewPos + arrowOffset;
	}

	private void Update()
	{
		animationTimer += Time.deltaTime;
		if (animationTimer <= animationLerpTime)
		{
			UpdateLerpAnimation();
		}
		else
		{
			UpdateArrowTransform();
		}
	}

	private void UpdateLerpAnimation()
	{
		float num = animationSpawnCurve.Evaluate(animationTimer / animationLerpTime);
		Vector3 vector = Vector3.Lerp(startPos, targetDropOff.position, num);
		float num2 = Mathf.Min((targetDropOff.position - targetPickup.position).magnitude, farPlane - 100f) * num;
		SetArrowBobbing(num2);
		transform.position = vector + arrowOffset;
		SetScaleFromDistance(num2);
	}

	private void UpdateArrowTransform()
	{
		Vector3 dir = targetDropOff.position - targetPickup.position;
		float num = Mathf.Min(dir.magnitude, farPlane - 100f);
		MoveInDirection(dir, num);
		SetScaleFromDistance(num);
	}

	private void MoveInDirection(Vector3 dir, float dist)
	{
		transform.position = targetPickup.position + dir.normalized * dist + arrowOffset;
		SetArrowBobbing(dist);
	}

	private void SetArrowBobbing(float dist)
	{
		Vector3 localPosition = transform.localPosition;
		arrowOffset.y = bobbleCurve.Evaluate(animationTimer * animationSpeed) * Mathf.Max(distanceScale * dist, 1f) + initialHeight;
		transform.localPosition = localPosition;
	}

	private void SetScaleFromDistance(float dist)
	{
		transform.localScale = initialScale + initialScale * distanceScale * dist;
	}
}
