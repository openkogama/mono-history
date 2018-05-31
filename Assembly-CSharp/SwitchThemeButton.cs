using UnityEngine;
using UnityEngine.UI;

public class SwitchThemeButton : MonoBehaviour
{
	[SerializeField]
	private GameObject levelReq;

	[SerializeField]
	private RawImage levelReqImage;

	[SerializeField]
	private Button button;

	public Button Button => button;

	public int LevelRequirement
	{
		set
		{
			if (value > 0)
			{
				levelReq.SetActive(value: true);
				BadgeManager.GetBadgeTexture(value, OnLevelTextureReceived);
			}
			else
			{
				levelReq.SetActive(value: false);
			}
		}
	}

	private void OnLevelTextureReceived(WWW www)
	{
		if (string.IsNullOrEmpty(www.error))
		{
			levelReqImage.texture = www.texture;
		}
	}

	protected void Destroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnLevelTextureReceived);
	}

	protected void Reset()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
	}
}
