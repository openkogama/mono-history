using System.Collections;
using UnityEngine;

public class SentryGunBeam : MonoBehaviour
{
	private const float timeout = 1f;

	public LineRenderer lineRenderer;

	private float deleteTimer = 1f;

	public bool Active { get; set; }

	public Vector3 StartPosition { get; private set; }

	public Vector3 EndPosition { get; private set; }

	public static SentryGunBeam Create(SentryGunBeam prefab, SentryGunBeamType beamType, MVSentryGun owner)
	{
		SentryGunBeam sentryGunBeam = Object.Instantiate(prefab, owner.WorldPosition, owner.WorldRotation) as SentryGunBeam;
		sentryGunBeam.lineRenderer.sharedMaterial = sentryGunBeam.lineRenderer.sharedMaterials[(uint)beamType];
		return sentryGunBeam;
	}

	protected virtual void OnUpdate()
	{
	}

	public void RefreshTime()
	{
		deleteTimer = 1f;
	}

	public void SetBeamPositions(Vector3 start, Vector3 end)
	{
		transform.rotation = Quaternion.LookRotation((end - start).normalized, Vector3.up);
		StartPosition = start;
		EndPosition = end;
		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, end);
	}

	private IEnumerator Start()
	{
		while (deleteTimer > 0f)
		{
			OnUpdate();
			deleteTimer -= Time.deltaTime;
			yield return null;
		}
		Object.Destroy(gameObject);
	}
}
