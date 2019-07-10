using System.Collections;
using MV.Common;
using UnityEngine;

public class TeleportAvatar : MonoBehaviour
{
	private delegate void ActionDelegate(float time);

	public float teleportTime = 2f;

	public Vector3 targetPosition;

	public Vector3 originPosition;

	public MVAvatarLocal avatar;

	private bool shouldCancelTeleportation;

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
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += OnAvatarStateChanged;
		yield return StartCoroutine(DoForSeconds(teleportTime, (float t) =>
		{
			avatar.SetTransparency = 1f - BlockStep(t, 10f);
		}));
		if (shouldCancelTeleportation)
		{
			avatar.SetTransparency = 1f;
		}
		else
		{
			avatar.WorldPosition = targetPosition;
			avatar.SyncPos = targetPosition;
			transform.position = targetPosition;
			rigidBody.Reset();
			yield return StartCoroutine(DoForSeconds(teleportTime, (float t) =>
			{
				avatar.SetTransparency = BlockStep(t, 10f);
			}));
		}
		EndTeleportation(rigidBody);
	}

	private void CancelTeleportation()
	{
		shouldCancelTeleportation = true;
	}

	private void OnAvatarStateChanged(SpawnRoleModeType mode)
	{
		CancelTeleportation();
	}

	private void EndTeleportation(MVRigidBody rigidBody)
	{
		if (rigidBody != null)
		{
			rigidBody.IsMovementLocked = false;
		}
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange -= OnAvatarStateChanged;
		Object.Destroy(gameObject);
	}
}
