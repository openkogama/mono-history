using System;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUIGameTypeButton : MonoBehaviour
{
	public delegate void OnGameTypeSelectDelegate(MVGUIGameTypeButton gameTypeButton, MVGameType gameType);

	public OnGameTypeSelectDelegate OnGameTypeSelect;

	public UXText gameTypeName;

	public UXPlane colorSquare;

	public float colorSquareInsert = 1f;

	public float ButtonWidth = 15f;

	public Color RTFColor;

	public Color DMColor;

	public Color TTColor;

	public Color CHATColor;

	public Color ADVColor;

	public Color CTFColor;

	public Color COOPColor;

	private MVGameType gameType;

	private UXPlane plane;

	private Color color;

	public Color ColorSquareSelected;

	public void BuildButton(MVGameType gameType)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		this.gameType = gameType;
		color = GameTypeToColor();
		plane = ((Component)this).gameObject.GetComponent<UXPlane>();
		plane.SetSize(ButtonWidth, 2f);
		plane.SetColor(color, string.Empty);
		((Component)colorSquare).transform.localPosition = new Vector3(colorSquareInsert, 0f, -0.1f);
		UpdateBGPlane(toggled: false);
		gameTypeName.Text = PrettyGameType();
		BoxCollider val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		val.size = new Vector3(ButtonWidth, 2f, 1f);
		val.center = new Vector3(ButtonWidth / 2f, 0f, 0.5f);
		UXMouseClickObject component = ((Component)this).gameObject.GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePos) =>
		{
			if (OnGameTypeSelect != null)
			{
				OnGameTypeSelect(this, this.gameType);
			}
		}));
	}

	public void SetToggled(bool toggled)
	{
		UpdateBGPlane(toggled);
	}

	private void UpdateBGPlane(bool toggled)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((Component)plane).renderer.enabled = toggled;
		colorSquare.SetColor((!toggled) ? color : ColorSquareSelected, string.Empty);
	}

	private string PrettyGameType()
	{
		return gameType switch
		{
			MVGameType.ADV => Localization.Instance.GetText(TextSlotIndex.GameType_Adventure), 
			MVGameType.CHAT => Localization.Instance.GetText(TextSlotIndex.GameType_Chat), 
			MVGameType.COOP => Localization.Instance.GetText(TextSlotIndex.GameType_Coop), 
			MVGameType.CTF => Localization.Instance.GetText(TextSlotIndex.GameType_CaptureTheFlag), 
			MVGameType.DM => Localization.Instance.GetText(TextSlotIndex.GameType_Deathmatch), 
			MVGameType.RTF => Localization.Instance.GetText(TextSlotIndex.GameType_ReachTheFlag), 
			MVGameType.TT => Localization.Instance.GetText(TextSlotIndex.GameType_TimeTrial), 
			_ => gameType.ToString(), 
		};
	}

	private Color GameTypeToColor()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		return gameType switch
		{
			MVGameType.ADV => ADVColor, 
			MVGameType.CHAT => CHATColor, 
			MVGameType.COOP => COOPColor, 
			MVGameType.CTF => CTFColor, 
			MVGameType.DM => DMColor, 
			MVGameType.RTF => RTFColor, 
			MVGameType.TT => TTColor, 
			_ => Color.gray, 
		};
	}
}
