using UnityEngine;

public class DynamicCullingHandler
{
	private CullingSubscriberDynamic cullingSubscriberDynamic;

	private float cullingRadius = 2f;

	public DynamicCullingHandler(float cullingRadius)
	{
		this.cullingRadius = cullingRadius;
	}

	public void ActivateCulling(GameObject cullingObject)
	{
		Debug.Log("Activating Culling");
		cullingSubscriberDynamic = new CullingSubscriberDynamic(cullingRadius, 3, cullingObject);
	}

	public void DeActivateCulling()
	{
		if (cullingSubscriberDynamic != null)
		{
			cullingSubscriberDynamic.Destroy();
			cullingSubscriberDynamic = null;
		}
	}

	public void UpdateCullingRadius(MVWorldObjectClient objArg, ScaleChangedEventArgs scaleArg)
	{
		CullingSubscriberDynamic cullingSubscriberDynamic = this.cullingSubscriberDynamic;
		float num = cullingRadius;
		Vector3 newScale = scaleArg.NewScale;
		cullingSubscriberDynamic.SetCullingRadius(num * newScale.y);
	}
}
