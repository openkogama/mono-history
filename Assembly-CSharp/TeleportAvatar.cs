using System.Collections;
using UnityEngine;

public class TeleportAvatar : MonoBehaviour
{
	private delegate void ActionDelegate(float time);

	public float teleportTime = 2f;

	public AudioClip leaveClip;

	public AudioClip arriveClip;

	public Vector3 targetPosition;

	public Vector3 originPosition;

	public MVAvatar avatar;

	private IEnumerator DoForSeconds(float duration, ActionDelegate body)
	{
		float t = 0f;
		while (t < duration)
		{
			body(t / duration);
			t += Time.deltaTime;
			yield return 0;
		}
		body(1f);
	}

	private float BlockStep(float t, float steps)
	{
		return Mathf.Round(t * steps) / steps;
	}

	private IEnumerator Start()
	{
		avatar.WorldPosition = originPosition;
		MVRigidBody rigidBody = avatar.GameObject.GetComponent<MVRigidBody>();
		if (rigidBody != null)
		{
			rigidBody.IsMovementLocked = true;
			rigidBody.Reset();
		}
		MVGameController.AudioManager.Play("Teleport avatar", leaveClip, avatar.WorldPosition, 0.4f, SoundRangeDistance.Short);
		yield return StartCoroutine(DoForSeconds(teleportTime, (float t) =>
		{
			avatar.Scale = Vector3.one * (1f - BlockStep(t, 5f));
		}));
		avatar.WorldPosition = targetPosition;
		avatar.SyncPos = targetPosition;
		transform.position = targetPosition;
		rigidBody.Reset();
		MVGameController.AudioManager.Play("Teleport avatar", arriveClip, avatar.WorldPosition, 0.4f, SoundRangeDistance.Short);
		yield return StartCoroutine(DoForSeconds(teleportTime, (float t) =>
		{
			avatar.Scale = Vector3.one * BlockStep(t, 5f);
		}));
		if (rigidBody != null)
		{
			rigidBody.IsMovementLocked = false;
		}
		Object.Destroy(gameObject);
	}
}
