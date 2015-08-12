using UnityEngine;

public class WorldObjectEnableController : MonoBehaviour
{
	private EnableState enableState = EnableState.Enable;

	[SerializeField]
	private GreyOutObjectScript greyOutObjectScript;

	public EnableState EnableState
	{
		get
		{
			return enableState;
		}
		set
		{
			if (enableState == value)
			{
				return;
			}
			enableState = value;
			if (!(greyOutObjectScript == null))
			{
				switch (value)
				{
				case EnableState.DisableAndHide:
					greyOutObjectScript.Hide();
					break;
				case EnableState.DisableAndGreyOut:
					greyOutObjectScript.GreyOut();
					break;
				case EnableState.Enable:
					greyOutObjectScript.GreyIn();
					break;
				}
			}
		}
	}
}
