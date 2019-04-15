using UnityEngine;
using UnityEngine.Events;

public class GamePointGainEffect : MonoBehaviour
{
	[SerializeField]
	private float targetSpeedAccelerationPerSec = 2f;

	[SerializeField]
	private float randomOffsetMaxSpeed = 10f;

	[SerializeField]
	private float randomOffsetMinSpeed = 2f;

	private float targetSpeed;

	private float offsetSpeed;

	private Vector3 offsetDirection = Vector3.zero;

	private Transform targetTransform;

	private UnityAction<int> onReachCallbackCallback;

	private int id;

	public int ID => id;

	public void Initialize(UnityAction<int> onReachCallbackCallback, int id)
	{
		this.onReachCallbackCallback = onReachCallbackCallback;
		this.id = id;
	}

	public void StartEffect(Transform targetTransform, float offsetDirectionXMin = -0.6f, float offsetDirectionXMax = 0.6f, float offsetDirectionYMin = -0.8f, float offsetDirectionYMax = 0.8f)
	{
		this.targetTransform = targetTransform;
		targetSpeed = 0f;
		offsetSpeed = Random.Range(randomOffsetMinSpeed, randomOffsetMaxSpeed);
		offsetDirection.x = Random.Range(offsetDirectionXMin, offsetDirectionXMax);
		if (offsetDirection.x > 0f)
		{
			offsetDirection.x += 0.2f;
		}
		else
		{
			offsetDirection.x -= 0.2f;
		}
		offsetDirection.y = Random.Range(offsetDirectionYMin, offsetDirectionYMax);
	}

	private void Update()
	{
		UpdateOffsetSpeed();
		UpdateTargetSpeed();
	}

	private void UpdateOffsetSpeed()
	{
		transform.position += offsetDirection * offsetSpeed * Time.deltaTime;
	}

	private void UpdateTargetSpeed()
	{
		Vector3 position = targetTransform.position;
		Vector3 vector = position - transform.position;
		Vector3 vector2 = vector.normalized * targetSpeed * Time.deltaTime;
		transform.position += vector2;
		targetSpeed += targetSpeedAccelerationPerSec * Time.deltaTime;
		if (vector.magnitude < vector2.magnitude)
		{
			onReachCallbackCallback(id);
			transform.position = position;
		}
	}
}
