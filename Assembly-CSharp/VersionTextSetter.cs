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
		textObject.text = MVGameControllerBase.KoGaMaSettings.ReleaseName + "\nv. " + MVGameControllerBase.KoGaMaSettings.VersionStringNoBuild;
	}
}
