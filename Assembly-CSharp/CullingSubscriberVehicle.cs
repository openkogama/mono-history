using UnityEngine;

public class CullingSubscriberVehicle : CullingSubscriberBase
{
	private GameObject visualRoot;

	private const int vehicleDistanceBandIndex = 2;

	public CullingSubscriberVehicle(GameObject visualRoot)
	{
		this.visualRoot = visualRoot;
		CullingIndex = 2;
		Radius = 10f;
	}

	public override void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool flag = CullingApiWrapper.Visible(cullingGroupEvent, CullingIndex);
		if (visualRoot.activeSelf != flag)
		{
			visualRoot.SetActive(flag);
		}
	}

	public void PositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		Debug.Log("positionChanged");
		Position = arg0.Position;
	}
}
