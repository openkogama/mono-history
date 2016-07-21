using UnityEngine;
using UnityEngine.Events;

public class CullingSubscriberBase : ICullingSubscriber
{
	private UnityAction<CullingGroupEvent> callback;

	public int DistanceBandIndex { get; set; }

	public int CullingIndex { get; set; }

	public Vector3 Position
	{
		get
		{
			return CullingApiWrapper.spheres[CullingIndex].position;
		}
		set
		{
			CullingApiWrapper.spheres[CullingIndex].position = value;
		}
	}

	public float Radius
	{
		get
		{
			return CullingApiWrapper.spheres[CullingIndex].radius;
		}
		set
		{
			CullingApiWrapper.spheres[CullingIndex].radius = value;
		}
	}

	public CullingSubscriberBase()
	{
		CullingApiWrapper.Subscribe(this);
	}

	public CullingSubscriberBase(UnityAction<CullingGroupEvent> callback)
		: this()
	{
		this.callback = callback;
	}

	public CullingSubscriberBase(float radius, Vector3 position, UnityAction<CullingGroupEvent> callback)
		: this(callback)
	{
		Setup(radius, position);
	}

	public void Setup(float radius, Vector3 position)
	{
		Position = position;
		Radius = radius;
		DistanceBandIndex = CullingApiWrapper.GetDistanceBand(radius);
	}

	public void Destroy()
	{
		CullingApiWrapper.UnSubscribe(this);
		callback = null;
	}

	public virtual void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		callback(cullingGroupEvent);
	}
}
