using MV.WorldObject;
using UnityEngine;

public class MVWorldObjectController : MonoBehaviour
{
	public MVWorldObjectClient worldObject;

	private void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown((KeyCode)119))
		{
			Transform transform = worldObject.GameObject.transform;
			transform.position += worldObject.GameObject.transform.forward;
			worldObject.State = MVWorldObjectState.Dirty;
		}
		if (Input.GetKeyDown((KeyCode)115))
		{
			Transform transform2 = worldObject.GameObject.transform;
			transform2.position -= worldObject.GameObject.transform.forward;
			worldObject.State = MVWorldObjectState.Dirty;
		}
	}
}
