using UnityEngine;
using UnityEngine.UI;

public class VersionTextSetter : MonoBehaviour
{
	[SerializeField]
	private Text textObject;

	protected void Start()
	{
		SetText();
	}

	private void SetText()
	{
		textObject.text = "v. " + MVGameControllerBase.KoGaMaSettings.VersionStringNoBuild;
	}
}
