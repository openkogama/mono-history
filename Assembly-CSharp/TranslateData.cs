using UnityEngine;

public class TranslateData
{
	public MVWorldObjectClient wo;

	public Vector3 ungridifiedPosition;

	public Vector3 gridifiedPosition;

	public Vector3 prevGridifiedPosition;

	public Vector3 localDirCamToObject;

	public TranslateData(MVWorldObjectClient wo, float gridSize)
	{
		this.wo = wo;
		ungridifiedPosition = wo.WorldPosition;
		gridifiedPosition = wo.GetClosestGridPoint(gridSize, ungridifiedPosition);
		localDirCamToObject = Vector3.zero;
		prevGridifiedPosition = gridifiedPosition;
	}
}
