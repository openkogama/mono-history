using UnityEngine;

public class CollectibleEffects : MonoBehaviour
{
	private float rotationSpeed = 0.6f;

	private float reshowingStartTime;

	private MVCollectible.CollectibleClientState state;

	public void SetState(MVCollectible.CollectibleClientState collectibleClientState)
	{
		state = collectibleClientState;
		if (state == MVCollectible.CollectibleClientState.ReShowing)
		{
			reshowingStartTime = Time.realtimeSinceStartup;
		}
	}

	private void Update()
	{
		if (state == MVCollectible.CollectibleClientState.Visible || state == MVCollectible.CollectibleClientState.Invisible)
		{
			float num = (0.35f + Mathf.Sin(Time.realtimeSinceStartup * 3f) * 0.05f) * 2f;
			transform.localScale = new Vector3(num, num, num);
			transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * 57.29578f, Space.Self);
		}
		else if (state == MVCollectible.CollectibleClientState.PickedUp)
		{
			float num2 = 0f;
			if (transform.localScale.x > 0.001f)
			{
				transform.localScale = new Vector3(num2, num2, num2);
			}
		}
		else if (state == MVCollectible.CollectibleClientState.ReShowing)
		{
			float num3 = 0.6f * (Time.realtimeSinceStartup - reshowingStartTime) * 2f;
			transform.localScale = new Vector3(num3, num3, num3);
		}
	}
}
