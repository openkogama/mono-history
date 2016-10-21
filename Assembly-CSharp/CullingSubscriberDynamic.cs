using UnityEngine;

public class CullingSubscriberDynamic : ICullingSubscriber, IUpdatecontrollerSubscriber
{
	private int cullingBandIndex;

	private int overrideDistanceBandIndex = -1;

	private GameObject root;

	private Transform rootTransform;

	private GameObject[] children;

	public int CullingIndex { get; set; }

	public CullingSubscriberDynamic(float radius, int cullingBandIndex, int overrideDistanceBandIndex, GameObject root, GameObject[] children = null)
	{
		this.cullingBandIndex = cullingBandIndex;
		this.root = root;
		rootTransform = root.transform;
		this.children = children;
		CullingApiWrapper.Subscribe(this);
		CullingApiWrapper.spheres[CullingIndex].position = rootTransform.position;
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
		root.SetActive(active);
		if (children != null)
		{
			GameObject[] array = children;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(active);
			}
		}
	}

	public void UpdateControllerUpdate()
	{
		CullingApiWrapper.spheres[CullingIndex].position = rootTransform.position;
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
