using System;
using UnityEngine;
using UnityEngine.Events;

public class LaserSight : MonoBehaviour
{
	[SerializeField]
	private PickupItem itemAttachedTo;

	[SerializeField]
	private Transform muzzlePoint;

	[SerializeField]
	private Projector projector;

	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private float rayWidth;

	[SerializeField]
	private Color startColor;

	[SerializeField]
	private Color endColor;

	[SerializeField]
	private LineRenderer coreLineRenderer;

	[SerializeField]
	private float coreRayWidth;

	[SerializeField]
	private Color coreColorStart;

	[SerializeField]
	private Color coreColorEnd;

	[SerializeField]
	private AnimationCurve flashCurve;

	private float flashDuration;

	private float flashTimer = float.PositiveInfinity;

	private const float projectionDistFromImpactPoint = 10f;

	private float initialProjectorSize;

	private bool idle;

	private void OnValidate()
	{
		if (lineRenderer != null)
		{
			initialProjectorSize = projector.orthographicSize;
			lineRenderer.startColor = startColor;
			lineRenderer.endColor = endColor;
			coreLineRenderer.startColor = coreColorStart;
			coreLineRenderer.endColor = coreColorEnd;
			OnScaleChange();
		}
	}

	private void Update()
	{
		if (flashTimer < flashDuration)
		{
			UpdateFlash();
		}
		else if (!idle)
		{
			SetIdle();
		}
	}

	public void Initialize()
	{
		flashDuration = flashCurve.keys[flashCurve.length - 1].time;
		initialProjectorSize = projector.orthographicSize;
		SetIdle();
		OnScaleChange();
		if (itemAttachedTo.owner != null)
		{
			MVWorldObjectClient worldObjectOwner = itemAttachedTo.owner.WorldObjectOwner;
			worldObjectOwner.ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(worldObjectOwner.ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(OnScaleChange));
		}
	}

	private void SetIdle()
	{
		idle = true;
		lineRenderer.startColor = startColor;
		lineRenderer.endColor = endColor;
		coreLineRenderer.enabled = false;
	}

	private void UpdateFlash()
	{
		flashTimer += Time.deltaTime;
		float num = flashCurve.Evaluate(flashTimer);
		Color color = new Color(0f, 0f, 0f, 1f) * num;
		lineRenderer.startColor = startColor + color;
		lineRenderer.endColor = endColor + color;
		coreLineRenderer.startColor = coreColorStart * num;
		coreLineRenderer.endColor = coreColorEnd * num;
	}

	public void DisableLineRenderer()
	{
		lineRenderer.gameObject.SetActive(value: false);
		coreLineRenderer.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (itemAttachedTo.owner != null)
		{
			MVWorldObjectClient worldObjectOwner = itemAttachedTo.owner.WorldObjectOwner;
			worldObjectOwner.ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Remove(worldObjectOwner.ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(OnScaleChange));
		}
	}

	private void OnScaleChange(MVWorldObjectClient obj = null, ScaleChangedEventArgs args = null)
	{
		projector.orthographicSize = initialProjectorSize * transform.lossyScale.y;
		float num = rayWidth * transform.lossyScale.y;
		lineRenderer.startWidth = num;
		lineRenderer.endWidth = num;
		num = coreRayWidth * transform.lossyScale.y;
		coreLineRenderer.startWidth = num;
		coreLineRenderer.endWidth = num;
	}

	public void AimAt(Vector3 point)
	{
		Vector3 position = point;
		position -= Vector3.Normalize(point - muzzlePoint.position) * 10f;
		transform.position = position;
		transform.LookAt(point);
		lineRenderer.SetPosition(0, muzzlePoint.transform.position);
		lineRenderer.SetPosition(1, point);
		coreLineRenderer.SetPosition(0, muzzlePoint.transform.position);
		coreLineRenderer.SetPosition(1, point);
	}

	public void Flash()
	{
		idle = false;
		flashTimer = 0f;
		coreLineRenderer.enabled = true;
	}
}
