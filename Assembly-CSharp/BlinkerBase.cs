using System.Collections.Generic;
using UnityEngine;

public class BlinkerBase : MonoBehaviour
{
	[SerializeField]
	protected Material blinkMaterial;

	protected int layerMask;

	protected bool visible;

	protected MeshFilter[] meshFilters;

	protected Dictionary<BlinkType, Blinker> blinkers;

	protected Camera targetCamera;

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	public MeshFilter[] MeshFilters
	{
		get
		{
			return meshFilters;
		}
		set
		{
			meshFilters = value;
		}
	}

	private void Awake()
	{
		targetCamera = Camera.main;
	}

	public void StartBlinking(BlinkType type, float duration = float.PositiveInfinity)
	{
		blinkers[type].Start(duration);
	}

	public void StopBlinking(BlinkType type)
	{
		blinkers[type].Stop();
	}

	private void LateUpdate()
	{
		if (!visible || blinkers == null || blinkers.Values == null)
		{
			return;
		}
		foreach (Blinker value in blinkers.Values)
		{
			if (value.IsExpired)
			{
				continue;
			}
			BeforeDraw();
			if (meshFilters == null)
			{
				continue;
			}
			MeshFilter[] array = meshFilters;
			foreach (MeshFilter meshFilter in array)
			{
				if (meshFilter.gameObject.activeInHierarchy)
				{
					Transform tfm = meshFilter.transform;
					value.Draw(meshFilter.sharedMesh, tfm, targetCamera, layerMask);
				}
			}
		}
	}

	protected virtual void BeforeDraw()
	{
	}

	private void OnDestroy()
	{
		foreach (Blinker value in blinkers.Values)
		{
			value.DestroyBlinkerMaterial();
		}
	}
}
