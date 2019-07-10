using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour, ILaserPointer
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

	public Vector3 offset = new Vector3(0.43f, -0.36f, 0.5f);

	public Transform cube;

	public LineRenderer lineRenderer;

	public Material insertingMaterial;

	public Material deleteMaterial;

	public Material transformingMaterial;

	public Color beamObjectColor = new Color(0f, 1f, 0f, 0.8f);

	public Color beamDeleteColor = new Color(1f, 0f, 0f, 0.8f);

	public Color beamEditColor = new Color(0f, 0f, 1f, 0.8f);

	private float lastSyncTime;

	private Dictionary<object, object> syncBuffer = new Dictionary<object, object>();

	private Material currentCubeMaterial;

	private byte currentCubeMaterialId;

	private Color beamColor = new Color(1f, 0f, 0f, 0.8f);

	private LaserPointerState state;

	private Vector3 relativeTargetPosition = default;

	private Vector3 relativeCurrentTargetPosition = default;

	private bool isFiring;

	private float activeDuration;

	private float currentLaserAlpha;

	private const float syncInverval = 0.4f;

	[SerializeField]
	private Renderer cubeRenderer;

	[SerializeField]
	private MeshFilter cubeMeshFilter;

	private bool isActive;

	private bool isLocal;

	private MVRuntimeDataVariable currentItem;

	public Renderer CubeRenderer => cubeRenderer;

	public MeshFilter CubeMeshFilter => cubeMeshFilter;

	public void SetLaserActiveState(bool isActive)
	{
		this.isActive = isActive;
	}

	public void Initialize(bool isLocal, MVRuntimeDataVariable currentItem, Transform parent)
	{
		this.isLocal = isLocal;
		this.currentItem = currentItem;
		transform.parent = parent;
		if (!isLocal)
		{
			currentItem.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(currentItem.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnChange));
		}
	}

	private void OnChange(object newvalue)
	{
		OnStateChanged((Dictionary<object, object>)newvalue);
	}

	public void SetCurrentCubeMaterial(byte cubeMaterial)
	{
		currentCubeMaterialId = cubeMaterial;
		SyncState(new Dictionary<object, object> { { "cm", currentCubeMaterialId } });
		ApplyMaterialForState();
	}

	public void SetLaserCubeVisible(bool visible)
	{
		CubeRenderer.enabled = visible;
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
		isActive = true;
		activeDuration = Mathf.Min(activeDuration + duration, 0.2f);
		StopCoroutine("DoDeactivateLaserAfterDuration");
		StartCoroutine("DoDeactivateLaserAfterDuration");
	}

	public void OnStateChanged(Dictionary<object, object> newState)
	{
		if (!isLocal)
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
				isActive = (bool)newState["fire"];
			}
			if (newState.ContainsKey("cm"))
			{
				currentCubeMaterialId = (byte)newState["cm"];
				ApplyMaterialForState();
			}
		}
	}

	public void OnEquip()
	{
		if (isLocal)
		{
			cube.parent = Camera.main.transform;
			cube.localPosition = offset;
		}
		enabled = true;
		gameObject.SetActive(value: true);
		cube.gameObject.SetActive(value: true);
	}

	public void SubscribeToCommands()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.OnCubeMaterialChanged += SetCurrentCubeMaterial;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.OnActivateLaserForDuration += ActivateLaserForDuration;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.OnChangeState += ChangeState;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.OnLaserActiveChanged += SetLaserActiveState;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.OnUpdatePosition += UpdatePosition;
	}

	private void OnEnable()
	{
		cube.gameObject.SetActive(value: true);
	}

	private void OnDisable()
	{
		if (cube != null)
		{
			cube.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		lineRenderer.positionCount = 2;
		ChangeState(state);
	}

	private void LateUpdate()
	{
		if (isLocal && isFiring != isActive)
		{
			isFiring = isActive;
			SyncState(new Dictionary<object, object> { { "fire", isFiring } });
		}
		if (!isLocal)
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
			if (isActive)
			{
				lineRenderer.SetPosition(1, cube.position + relativeCurrentTargetPosition);
			}
		}
		float num;
		if (isActive)
		{
			num = ((!isLocal) ? 1f : 0.8f);
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
		if (isLocal && !lineRenderer.enabled)
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
		while (isActive && t < activeDuration)
		{
			t += Time.deltaTime;
			yield return 0;
		}
		activeDuration = 0f;
		isActive = false;
	}

	private void ApplyMaterialForState()
	{
		currentCubeMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(currentCubeMaterialId);
		switch (state)
		{
		case LaserPointerState.Idle:
			CubeRenderer.sharedMaterial = currentCubeMaterial;
			CubeMeshFilter.sharedMesh = material.Mesh;
			break;
		case LaserPointerState.Inserting:
			CubeRenderer.sharedMaterial = insertingMaterial;
			beamColor = beamObjectColor;
			break;
		case LaserPointerState.EditingCube:
			CubeRenderer.sharedMaterial = currentCubeMaterial;
			CubeMeshFilter.sharedMesh = material.Mesh;
			beamColor = beamEditColor;
			break;
		case LaserPointerState.Transforming:
			CubeRenderer.sharedMaterial = transformingMaterial;
			beamColor = beamObjectColor;
			break;
		case LaserPointerState.DeletingCubes:
			CubeRenderer.sharedMaterial = deleteMaterial;
			beamColor = beamDeleteColor;
			break;
		case LaserPointerState.PaintCubes:
			CubeRenderer.sharedMaterial = currentCubeMaterial;
			CubeMeshFilter.sharedMesh = material.Mesh;
			beamColor = beamEditColor;
			break;
		case LaserPointerState.SprayCubes:
			CubeRenderer.sharedMaterial = currentCubeMaterial;
			CubeMeshFilter.sharedMesh = material.Mesh;
			beamColor = beamEditColor;
			break;
		}
	}

	protected void SyncState(Dictionary<object, object> newState)
	{
		if (!newState.ContainsKey("type"))
		{
			newState.Add("type", 0);
		}
		currentItem.Value = newState;
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
