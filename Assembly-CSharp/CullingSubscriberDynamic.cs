using UnityEngine;

public class CullingSubscriberDynamic : ICullingSubscriber, IUpdatecontrollerSubscriber
{
	private int cullingBandIndex;

	private int overrideDistanceBandIndex = -1;

	private GameObject gameObject;

	private Transform transform;

	public int CullingIndex { get; set; }

	public CullingSubscriberDynamic(float radius, int cullingBandIndex, int overrideDistanceBandIndex, GameObject gameObject)
	{
		this.cullingBandIndex = cullingBandIndex;
		this.gameObject = gameObject;
		transform = gameObject.transform;
		CullingApiWrapper.Subscribe(this);
		CullingApiWrapper.spheres[CullingIndex].position = transform.position;
		CullingApiWrapper.spheres[CullingIndex].radius = radius;
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingGroupEvent, cullingBandIndex);
		if (overrideDistanceBandIndex != -1 && cullingGroupEvent.currentDistance <= overrideDistanceBandIndex)
		{
			active = true;
		}
		gameObject.SetActive(active);
	}

	public void UpdateControllerUpdate()
	{
		CullingApiWrapper.spheres[CullingIndex].position = transform.position;
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	public void Destroy()
	{
		CullingApiWrapper.UnSubscribe(this);
		UpdateController.RemoveUpdateObject(this);
	}
}
