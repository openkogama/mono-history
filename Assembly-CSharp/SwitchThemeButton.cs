using UnityEngine;
using UnityEngine.UI;

public class SwitchThemeButton : MonoBehaviour
{
	[SerializeField]
	private GameObject levelReq;

	[SerializeField]
	private RawImage levelReqImage;

	[SerializeField]
	private GameObject priceTag;

	[SerializeField]
	private Text priceTagNumber;

	[SerializeField]
	private Button button;

	public Button Button => button;

	private int LevelRequirement
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

	private int GoldRequirement
	{
		set
		{
			priceTagNumber.text = value.ToString();
		}
	}

	public void Initialize(int levelReq, int goldReq)
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level < levelReq)
		{
			LevelRequirement = levelReq;
			this.levelReq.SetActive(value: true);
			priceTag.SetActive(value: false);
		}
		else
		{
			GoldRequirement = goldReq;
			this.levelReq.SetActive(value: false);
			priceTag.SetActive(value: true);
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
