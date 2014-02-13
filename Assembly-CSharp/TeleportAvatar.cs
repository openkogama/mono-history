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
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.IsMovementLocked = true;
			rigidBody.Reset();
		}
		MVGameController.Instance.AudioManager.Play("Teleport avatar", leaveClip, avatar.WorldPosition, 0.8f, SoundRangeDistance.Short);
		yield return ((MonoBehaviour)this).StartCoroutine(DoForSeconds(teleportTime, (float t) =>
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			avatar.Scale = Vector3.one * (1f - BlockStep(t, 5f));
		}));
		avatar.WorldPosition = targetPosition;
		avatar.SyncPos = targetPosition;
		((Component)this).transform.position = targetPosition;
		MVGameController.Instance.AudioManager.Play("Teleport avatar", arriveClip, avatar.WorldPosition, 0.8f, SoundRangeDistance.Short);
		yield return ((MonoBehaviour)this).StartCoroutine(DoForSeconds(teleportTime, (float t) =>
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			avatar.Scale = Vector3.one * BlockStep(t, 5f);
		}));
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.IsMovementLocked = false;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
