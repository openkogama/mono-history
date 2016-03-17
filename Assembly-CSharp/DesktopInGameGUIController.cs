using MV.Common;
using UnityEngine;

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
