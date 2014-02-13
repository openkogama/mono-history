using System.Collections;
using MV.Common;
using UnityEngine;

public class LaserPointer : PickupItem, ILaserPointer
{
	private enum NetworkStateKey : byte
	{
		State = 10,
		TargetX,
		TargetY,
		TargetZ,
		IsFiring,
		CubeMaterial
	}

	private const float syncInverval = 0.1f;

	public Vector3 offset = new Vector3(0.43f, -0.36f, 0.5f);

	public Transform cube;

	public LineRenderer lineRenderer;

	public Material insertingMaterial;

	public Material deleteMaterial;

	public Material transformingMaterial;

	public Color beamObjectColor = new Color(0f, 1f, 0f, 0.8f);

	public Color beamDeleteColor = new Color(1f, 0f, 0f, 0.8f);

	public Color beamEditColor = new Color(0f, 0f, 1f, 0.8f);

	private Material currentCubeMaterial;

	private byte currentCubeMaterialId;

	private Color beamColor = new Color(1f, 0f, 0f, 0.8f);

	private LaserPointerState state;

	private Vector3 relativeTargetPosition = default;

	private Vector3 relativeCurrentTargetPosition = default;

	private bool isFiring;

	private float activeDuration;

	private float currentLaserAlpha;

	private float lastSyncTime;

	private Hashtable syncBuffer = new Hashtable();

	public bool LaserActive { get; set; }

	public byte CurrentCubeMaterial
	{
		get
		{
			return currentCubeMaterialId;
		}
		set
		{
			currentCubeMaterialId = value;
			SyncState(new Hashtable { { "cm", currentCubeMaterialId } });
			ApplyMaterialForState();
		}
	}

	public override AvatarItemType Type => AvatarItemType.LaserPointer;

	public override bool ActivateGunModeOnEquip => false;

	public LaserPointer()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
	}

	public void SetLaserCubeVisible(bool visible)
	{
		((Component)cube).renderer.enabled = visible;
	}

	public void ChangeState(LaserPointerState newState)
	{
		state = newState;
		ApplyMaterialForState();
		SyncState(new Hashtable { { "st", state } });
	}

	public void UpdatePosition(Vector3 to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		relativeTargetPosition = to - cube.position;
		IntervalSyncState(new Hashtable
		{
			{ "tx", relativeTargetPosition.x },
			{ "ty", relativeTargetPosition.y },
			{ "tz", relativeTargetPosition.z }
		}, 0.1f);
	}

	public void ActivateLaserForDuration(float duration)
	{
		LaserActive = true;
		activeDuration = Mathf.Min(activeDuration + duration, 0.2f);
		((MonoBehaviour)this).StopCoroutine("DoDeactivateLaserAfterDuration");
		((MonoBehaviour)this).StartCoroutine("DoDeactivateLaserAfterDuration");
	}

	public override void OnStateChanged(Hashtable newState)
	{
		if (!owner.IsLocal)
		{
			if (newState.ContainsKey("tx"))
			{
				relativeTargetPosition.x = (float)newState["tx"];
			}
			if (newState.ContainsKey("ty"))
			{
				relativeTargetPosition.y = (float)newState["ty"];
			}
			if (newState.ContainsKey("tz"))
			{
				relativeTargetPosition.z = (float)newState["tz"];
			}
			if (newState.ContainsKey("st"))
			{
				state = (LaserPointerState)(int)newState["st"];
				ApplyMaterialForState();
			}
			if (newState.ContainsKey("fire"))
			{
				LaserActive = (bool)newState["fire"];
			}
			if (newState.ContainsKey("cm"))
			{
				currentCubeMaterialId = (byte)newState["cm"];
				ApplyMaterialForState();
			}
		}
	}

	public override void OnEquip()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (owner.IsLocal)
		{
			cube.parent = ((Component)Camera.main).transform;
			cube.localPosition = offset;
		}
		((Behaviour)this).enabled = true;
		((Component)this).gameObject.SetActiveRecursively(true);
		((Component)cube).gameObject.SetActiveRecursively(true);
	}

	public override void OnUnequip()
	{
		cube.parent = ((Component)this).transform;
		((Component)cube).gameObject.SetActiveRecursively(false);
		((Component)this).gameObject.SetActiveRecursively(false);
		((Behaviour)this).enabled = false;
		LaserActive = false;
	}

	private void Start()
	{
		lineRenderer.SetVertexCount(2);
		ChangeState(state);
	}

	private void LateUpdate()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		if (owner.IsLocal && isFiring != LaserActive)
		{
			isFiring = LaserActive;
			SyncState(new Hashtable { { "fire", isFiring } });
		}
		if (!owner.IsLocal)
		{
			Vector3 val = cube.position + relativeTargetPosition - ((Component)this).transform.parent.position;
			val.y = 0f;
			val.Normalize();
			cube.position = ((Component)this).transform.parent.position + Vector3.up + val * 1.25f;
		}
		cube.LookAt(cube.position + relativeTargetPosition, cube.parent.up);
		if (((Renderer)lineRenderer).enabled)
		{
			lineRenderer.SetPosition(0, cube.position);
			if (LaserActive)
			{
				lineRenderer.SetPosition(1, cube.position + relativeCurrentTargetPosition);
			}
		}
		float num;
		if (LaserActive)
		{
			num = ((!owner.IsLocal) ? 1f : 0.8f);
		}
		else
		{
			num = 0f;
		}
		if (currentLaserAlpha < num)
		{
			currentLaserAlpha = Mathf.Min(num, currentLaserAlpha + Time.deltaTime * 4f);
		}
		else if (currentLaserAlpha > num)
		{
			currentLaserAlpha = Mathf.Max(num, currentLaserAlpha - Time.deltaTime * 4f);
		}
		beamColor.a = currentLaserAlpha;
		((Renderer)lineRenderer).enabled = beamColor.a > float.Epsilon;
		((Renderer)lineRenderer).material.SetColor("_TintColor", beamColor);
		if (owner.IsLocal && !((Renderer)lineRenderer).enabled)
		{
			Vector3 val2 = cube.parent.position + cube.parent.forward * 10f - cube.position;
			Quaternion val3 = Quaternion.LookRotation(val2, cube.parent.up);
			Quaternion val4 = Quaternion.LookRotation(cube.forward, cube.parent.up);
			Quaternion val5 = Quaternion.Slerp(val4, val3, Time.deltaTime * 10f);
			relativeTargetPosition = val5 * Vector3.forward * Mathf.Lerp(relativeTargetPosition.magnitude, 10f, Time.deltaTime * 5f);
		}
		relativeCurrentTargetPosition = Vector3.Lerp(relativeCurrentTargetPosition, relativeTargetPosition, Time.deltaTime * 20f);
	}

	private IEnumerator DoDeactivateLaserAfterDuration()
	{
		float t = 0f;
		while (LaserActive && t < activeDuration)
		{
			t += Time.deltaTime;
			yield return 0;
		}
		activeDuration = 0f;
		LaserActive = false;
	}

	private void ApplyMaterialForState()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		currentCubeMaterial = MVGameController.Instance.Game.MaterialRepository.GetMaterial(currentCubeMaterialId).material;
		switch (state)
		{
		case LaserPointerState.Idle:
			((Component)cube).renderer.sharedMaterial = currentCubeMaterial;
			break;
		case LaserPointerState.Inserting:
			((Component)cube).renderer.sharedMaterial = insertingMaterial;
			beamColor = beamObjectColor;
			break;
		case LaserPointerState.EditingCube:
			((Component)cube).renderer.sharedMaterial = currentCubeMaterial;
			beamColor = beamEditColor;
			break;
		case LaserPointerState.Transforming:
			((Component)cube).renderer.sharedMaterial = transformingMaterial;
			beamColor = beamObjectColor;
			break;
		case LaserPointerState.DeletingCubes:
			((Component)cube).renderer.sharedMaterial = deleteMaterial;
			beamColor = beamDeleteColor;
			break;
		case LaserPointerState.PaintCubes:
			((Component)cube).renderer.sharedMaterial = currentCubeMaterial;
			beamColor = beamEditColor;
			break;
		case LaserPointerState.SprayCubes:
			((Component)cube).renderer.sharedMaterial = currentCubeMaterial;
			beamColor = beamEditColor;
			break;
		}
	}

	protected void SyncState(Hashtable newState)
	{
		if ((Object)(object)owner.CurrentItem == (Object)(object)this)
		{
			if (!newState.Contains("type"))
			{
				newState.Add("type", (int)Type);
			}
			((MVAvatar)owner.WorldObjectOwner).CurrentItem.Value = newState;
		}
		else
		{
			Debug.LogWarning((object)"Trying to sync non-equipped item!");
		}
	}

	protected void IntervalSyncState(Hashtable newState, float interval)
	{
		foreach (DictionaryEntry item in newState)
		{
			syncBuffer[item.Key] = item.Value;
		}
		if (lastSyncTime + interval < Time.time)
		{
			Hashtable newState2 = syncBuffer.Clone() as Hashtable;
			lastSyncTime = Time.time;
			SyncState(newState2);
			syncBuffer.Clear();
		}
	}
}
