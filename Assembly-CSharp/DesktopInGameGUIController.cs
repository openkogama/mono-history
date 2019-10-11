using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DesktopInGameGUIController : MonoBehaviour
{
	[SerializeField]
	private GameObject winningConditionLayoutGroup;

	[SerializeField]
	private ShowUse3D use3DPrefab;

	private ShowUse use;

	[SerializeField]
	private CrossHair crossHair;

	[SerializeField]
	private GameObject touristLogo;

	[SerializeField]
	private Image logo;

	[SerializeField]
	private LevelBadge levelBadge;

	private Dictionary<LoadLogoType, string> logoToPathMap = new Dictionary<LoadLogoType, string> { 
	{
		LoadLogoType.Poki,
		"Logos/Logo_Poki.png"
	} };

	public void Initialize()
	{
		use = Object.Instantiate(use3DPrefab);
		use.transform.SetParent(transform, worldPositionStays: false);
		use.transform.SetAsFirstSibling();
		if (MVGameControllerBase.IsTouristSession)
		{
			touristLogo.SetActive(value: true);
			LoadLogoType loadLogoType = MVGameControllerBase.GameSessionData.LoadLogoType;
			if (loadLogoType != LoadLogoType.None)
			{
				logo.gameObject.SetActive(value: true);
				string path = Urls.StreamingAssets + logoToPathMap[loadLogoType];
				AsyncWWWManager.WWWRequest(new CachedGetRequest(path, StreamingAssetCallback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
			}
		}
		levelBadge = Object.Instantiate(levelBadge);
		levelBadge.transform.SetParent(winningConditionLayoutGroup.transform, worldPositionStays: false);
		levelBadge.transform.SetAsFirstSibling();
	}

	private void StreamingAssetCallback(UnityWebRequest www)
	{
		if (www == null || www.isNetworkError || www.isHttpError)
		{
			Debug.LogWarning("Streaming asset callback failed for referral logo: " + www.error);
			return;
		}
		byte[] data = www.downloadHandler.data;
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		logo.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
		Debug.Log("referrer logo set from callback with string: " + MVGameControllerBase.GameSessionData.referrer);
	}

	public void ShowEUseIcon(ShowUseOption option, int woID = 0)
	{
		use.gameObject.SetActive(value: true);
		use.CalculateUseGraphics(option, woID);
	}

	public void HideEUseIcon()
	{
		use.gameObject.SetActive(value: false);
	}

	public IGUICrossHair GetCrossHair()
	{
		return crossHair;
	}

	private void OnDestroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(StreamingAssetCallback);
	}
}
