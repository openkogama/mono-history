using UnityEngine;
using UnityEngine.UI;

public class HelpPopup : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	[SerializeField]
	private GameObject pleaseWaitOverlay;

	public void Initialize(Texture2D texture)
	{
		image.texture = texture;
		pleaseWaitOverlay.SetActive(value: false);
	}
}
