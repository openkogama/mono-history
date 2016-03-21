using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class DesktopInGameGUIController : MonoBehaviour
{
	private bool showingEquipableUI;

	[SerializeField]
	private ShowUse2D use2DPrefab;

	[SerializeField]
	private ShowUse3D use3DPrefab;

	private ShowUse use;

	[SerializeField]
	private CrossHair crossHair;

	[SerializeField]
	private CrossHair crossHair2D;

	[SerializeField]
	private GameObject touristLogo;

	[SerializeField]
	private Image logo;

	private Dictionary<LoadLogoType, string> logoToPathMap = new Dictionary<LoadLogoType, string> { 
	{
		LoadLogoType.Poki,
		"Logos/Logo_Poki.png"
	} };

	public void Initialize()
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			use = Object.Instantiate(use2DPrefab);
		}
		else
		{
			use = Object.Instantiate(use3DPrefab);
		}
		use.transform.SetParent(transform, worldPositionStays: false);
		if (MVGameControllerBase.IsTouristSession)
		{
			touristLogo.SetActive(value: true);
			LoadLogoType loadLogoType = MVGameControllerBase.GameSessionData.LoadLogoType;
			if (loadLogoType != LoadLogoType.None)
			{
				string path = Urls.StreamingAssets + logoToPathMap[loadLogoType];
				AsyncWWWManager.WWWRequest(new CachedGetRequest(path, StreamingAssetCallback));
			}
		}
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogWarning("Streaming asset callback failed for referral logo: " + www.error);
				return;
			}
			logo.gameObject.SetActive(value: true);
			Texture2D texture2D = new Texture2D(www.texture.width, www.texture.height, www.texture.format, mipmap: false);
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.SetPixels32(www.texture.GetPixels32());
			texture2D.Apply();
			logo.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
		}
	}

	private void Update()
	{
		if (PickupGUI.ShowEquipableUI != showingEquipableUI)
		{
			showingEquipableUI = PickupGUI.ShowEquipableUI;
		}
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
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			return crossHair2D;
		}
		return crossHair;
	}
}
