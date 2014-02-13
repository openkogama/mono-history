using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SentryGunBeam : MonoBehaviour
{
	private const float timeout = 1f;

	public LineRenderer lineRenderer;

	private float deleteTimer = 1f;

	public bool Active { get; set; }

	public Vector3 StartPosition
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public Vector3 EndPosition
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public static SentryGunBeam Create(SentryGunBeam prefab, SentryGunBeamType beamType, MVSentryGun owner)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SentryGunBeam sentryGunBeam = Object.Instantiate((Object)(object)prefab, owner.WorldPosition, owner.WorldRotation) as SentryGunBeam;
		((Renderer)sentryGunBeam.lineRenderer).sharedMaterial = ((Renderer)sentryGunBeam.lineRenderer).sharedMaterials[(uint)beamType];
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).transform;
		Vector3 val = end - start;
		transform.rotation = Quaternion.LookRotation(val.normalized, Vector3.up);
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
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
