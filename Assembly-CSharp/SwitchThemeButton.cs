using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.Networking;
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
	private GameObject memberUI;

	[SerializeField]
	private Button button;

	private Texture2D levelRequirementTextureAsset;

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
			memberUI.SetActive(value: false);
			priceTag.SetActive(value: true);
		}
		else if (MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.FreeBuildingGameObjects))
		{
			this.levelReq.SetActive(value: false);
			priceTag.SetActive(value: false);
			memberUI.SetActive(value: true);
		}
		else
		{
			GoldRequirement = goldReq;
			this.levelReq.SetActive(value: false);
			priceTag.SetActive(value: true);
			memberUI.SetActive(value: false);
		}
	}

	private void OnLevelTextureReceived(UnityWebRequest www)
	{
		if (string.IsNullOrEmpty(www.error))
		{
			levelRequirementTextureAsset = DownloadHandlerTexture.GetContent(www);
			levelReqImage.texture = levelRequirementTextureAsset;
		}
	}

	protected void OnDestroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnLevelTextureReceived);
		Object.Destroy(levelRequirementTextureAsset);
	}

	protected void Reset()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
	}
}
