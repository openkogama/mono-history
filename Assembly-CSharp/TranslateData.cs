using UnityEngine;

public class TranslateData
{
	private readonly WorldObjectClientRef worldObjectClientRef;

	public Vector3 ungridifiedPosition;

	public Vector3 gridifiedPosition;

	public Vector3 prevGridifiedPosition;

	public Vector3 localDirCamToObject;

	public MVWorldObjectClient Wo => worldObjectClientRef.WorldObjectClient;

	public TranslateData(MVWorldObjectClient wo, float gridSize)
	{
		worldObjectClientRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(wo.Id);
		ungridifiedPosition = wo.WorldPosition;
		gridifiedPosition = wo.GetClosestGridPoint(gridSize, ungridifiedPosition);
		localDirCamToObject = Vector3.zero;
		prevGridifiedPosition = gridifiedPosition;
	}
}
