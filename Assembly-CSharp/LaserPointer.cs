using System.Collections;
using System.Collections.Generic;
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

	private const float syncInverval = 0.4f;

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

	private Dictionary<object, object> syncBuffer = new Dictionary<object, object>();

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
			SyncState(new Dictionary<object, object> { { "cm", currentCubeMaterialId } });
			ApplyMaterialForState();
		}
	}

	public override AvatarItemType Type => AvatarItemType.LaserPointer;

	public override bool ActivateGunModeOnEquip => false;

	public void SetLaserCubeVisible(bool visible)
	{
		cube.GetComponent<Renderer>().enabled = visible;
	}

	public void ChangeState(LaserPointerState newState)
	{
		state = newState;
		ApplyMaterialForState();
		SyncState(new Dictionary<object, object> { 
		{
			"st",
			(byte)state
		} });
	}

	public void UpdatePosition(Vector3 to)
	{
		relativeTargetPosition = to - cube.position;
		IntervalSyncState(new Dictionary<object, object>
		{
			{ "tx", relativeTargetPosition.x },
			{ "ty", relativeTargetPosition.y },
			{ "tz", relativeTargetPosition.z }
		}, 0.4f);
	}

	public void ActivateLaserForDuration(float duration)
	{
		LaserActive = true;
		activeDuration = Mathf.Min(activeDuration + duration, 0.2f);
		StopCoroutine("DoDeactivateLaserAfterDuration");
		StartCoroutine("DoDeactivateLaserAfterDuration");
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
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
				state = (LaserPointerState)(byte)newState["st"];
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
		if (owner.IsLocal)
		{
			cube.parent = Camera.main.transform;
			cube.localPosition = offset;
		}
		enabled = true;
		gameObject.SetActive(value: true);
		cube.gameObject.SetActive(value: true);
	}

	public override void OnUnequip()
	{
		cube.parent = transform;
		cube.gameObject.SetActive(value: false);
		gameObject.SetActive(value: false);
		enabled = false;
		LaserActive = false;
	}

	private void Start()
	{
		lineRenderer.SetVertexCount(2);
		ChangeState(state);
	}

	private void LateUpdate()
	{
		if (owner.IsLocal && isFiring != LaserActive)
		{
			isFiring = LaserActive;
			SyncState(new Dictionary<object, object> { { "fire", isFiring } });
		}
		if (!owner.IsLocal)
		{
			Vector3 vector = cube.position + relativeTargetPosition - transform.parent.position;
			vector.y = 0f;
			vector.Normalize();
			cube.position = transform.parent.position + Vector3.up + vector * 1.25f;
		}
		cube.LookAt(cube.position + relativeTargetPosition, cube.parent.up);
		if (lineRenderer.enabled)
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
		lineRenderer.enabled = beamColor.a > Mathf.Epsilon;
		lineRenderer.material.SetColor("_TintColor", beamColor);
		if (owner.IsLocal && !lineRenderer.enabled)
		{
			Vector3 forward = cube.parent.position + cube.parent.forward * 10f - cube.position;
			Quaternion b = Quaternion.LookRotation(forward, cube.parent.up);
			Quaternion a = Quaternion.LookRotation(cube.forward, cube.parent.up);
			Quaternion quaternion = Quaternion.Slerp(a, b, Time.deltaTime * 10f);
			relativeTargetPosition = quaternion * Vector3.forward * Mathf.Lerp(relativeTargetPosition.magnitude, 10f, Time.deltaTime * 5f);
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
		currentCubeMaterial = MVGameController.Game.MaterialRepository.GetMaterial(currentCubeMaterialId).material;
		switch (state)
		{
		case LaserPointerState.Idle:
			cube.GetComponent<Renderer>().sharedMaterial = currentCubeMaterial;
			break;
		case LaserPointerState.Inserting:
			cube.GetComponent<Renderer>().sharedMaterial = insertingMaterial;
			beamColor = beamObjectColor;
			break;
		case LaserPointerState.EditingCube:
			cube.GetComponent<Renderer>().sharedMaterial = currentCubeMaterial;
			beamColor = beamEditColor;
			break;
		case LaserPointerState.Transforming:
			cube.GetComponent<Renderer>().sharedMaterial = transformingMaterial;
			beamColor = beamObjectColor;
			break;
		case LaserPointerState.DeletingCubes:
			cube.GetComponent<Renderer>().sharedMaterial = deleteMaterial;
			beamColor = beamDeleteColor;
			break;
		case LaserPointerState.PaintCubes:
			cube.GetComponent<Renderer>().sharedMaterial = currentCubeMaterial;
			beamColor = beamEditColor;
			break;
		case LaserPointerState.SprayCubes:
			cube.GetComponent<Renderer>().sharedMaterial = currentCubeMaterial;
			beamColor = beamEditColor;
			break;
		}
	}

	protected void SyncState(Dictionary<object, object> newState)
	{
		if (owner.CurrentItem == this)
		{
			if (!newState.ContainsKey("type"))
			{
				newState.Add("type", (int)Type);
			}
			((MVAvatar)owner.WorldObjectOwner).CurrentItem.Value = newState;
		}
		else
		{
			Debug.LogWarning("Trying to sync non-equipped item!");
		}
	}

	protected void IntervalSyncState(Dictionary<object, object> newState, float interval)
	{
		foreach (KeyValuePair<object, object> item in newState)
		{
			syncBuffer[item.Key] = item.Value;
		}
		if (lastSyncTime + interval < Time.time)
		{
			Dictionary<object, object> newState2 = new Dictionary<object, object>(syncBuffer);
			lastSyncTime = Time.time;
			SyncState(newState2);
			syncBuffer.Clear();
		}
	}
}
