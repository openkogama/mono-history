using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThemeMenuButton : MonoBehaviour
{
	public interface IClickHandler : IEventSystemHandler
	{
		void OpenThemesMenu();
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private ToolTip toolTip;

	private static readonly string toolTipStr_ButtonDisabled = TM._("Themes can only be changed by the game owner");

	private static readonly string toolTipStr_ButtonEnabled = TM._("Theme options");

	protected void Awake()
	{
		if (ThemeRepository.Instance.ThemesEnabled)
		{
			SetButtonAvailability(MVGameControllerBase.Game.LocalPlayer.PlanetOwnership);
		}
		else
		{
			button.gameObject.SetActive(value: false);
		}
	}

	private void SetButtonAvailability(MVLocalPlayer.PlanetOwnershipType planetOwnership)
	{
		if (planetOwnership == MVLocalPlayer.PlanetOwnershipType.Owner)
		{
			button.interactable = true;
			toolTip.SetText(toolTipStr_ButtonEnabled);
		}
		else
		{
			button.interactable = false;
			toolTip.SetText(toolTipStr_ButtonDisabled);
		}
	}

	public void OnClick()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IClickHandler handler, BaseEventData data) =>
		{
			handler.OpenThemesMenu();
		});
	}
}
