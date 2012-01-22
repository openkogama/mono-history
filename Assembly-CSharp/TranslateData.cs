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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		this.wo = wo;
		ungridifiedPosition = wo.GameObject.transform.position;
		gridifiedPosition = wo.GetClosestGridPoint(gridSize, ungridifiedPosition);
		localDirCamToObject = Vector3.zero;
		prevGridifiedPosition = gridifiedPosition;
	}
}
