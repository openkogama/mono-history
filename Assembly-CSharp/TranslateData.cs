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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		this.wo = wo;
		ungridifiedPosition = wo.WorldPosition;
		gridifiedPosition = wo.GetClosestGridPoint(gridSize, ungridifiedPosition);
		localDirCamToObject = Vector3.zero;
		prevGridifiedPosition = gridifiedPosition;
	}
}
