using System.Collections.Generic;
using UnityEngine;

public class BlinkerBase : MonoBehaviour
{
	public Material blinkMaterial;

	private Material blinkMaterialInstance;

	protected bool visible;

	protected MeshFilter[] meshFilters;

	protected Dictionary<BlinkType, Blinker> blinkers;

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

	public void StartBlinking(BlinkType type, float duration)
	{
		blinkers[type].Start(duration);
	}

	public void StopBlinking(BlinkType type)
	{
		blinkers[type].Stop();
	}

	private void LateUpdate()
	{
		if (!visible || meshFilters == null)
		{
			return;
		}
		foreach (Blinker value in blinkers.Values)
		{
			if (!value.IsExpired)
			{
				MeshFilter[] array = meshFilters;
				foreach (MeshFilter val in array)
				{
					Transform transform = ((Component)val).transform;
					value.Draw(val.mesh, transform);
				}
			}
		}
	}
}
