using UnityEngine;

public class GodzillaCameraDesktop : GodzillaCamera
{
	protected override Vector2 GetMouseInput()
	{
		return new Vector2(0f - MVInputWrapper.GetAxis("Mouse Y"), MVInputWrapper.GetAxis("Mouse X"));
	}
}
