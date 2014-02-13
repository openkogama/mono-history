using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIControlRoomDialog : UXCustomDialogBox
{
	public Transform GameTypeSelectRoot;

	public MVGUIGameTypeButton GameTypeButtonPrefab;

	public UXSlider gravitySlider;

	public UXText gravityValueText;

	public UXSlider maxPlayersSlider;

	public UXText maxPlayersValueText;

	public UXSlider roundTimeSlider;

	public UXText roundTimeValueText;

	private Dictionary<MVGameType, MVGUIGameTypeButton> gameTypeButtons = new Dictionary<MVGameType, MVGUIGameTypeButton>();

	private MVGameType gameType;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		CreateGameTypeButton(MVGameType.RTF);
		CreateGameTypeButton(MVGameType.DM);
		CreateGameTypeButton(MVGameType.TT);
		CreateGameTypeButton(MVGameType.CHAT);
		CreateGameTypeButton(MVGameType.ADV);
		InitializeValues();
		InitializeListeners();
	}

	private void InitializeValues()
	{
	}

	private void InitializeListeners()
	{
		UXSlider uXSlider = gravitySlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			gravityValueText.Text = FormatGravity(s.Value);
		}));
		UXSlider uXSlider2 = gravitySlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			gravityValueText.Text = FormatGravity(v);
		}));
		UXSlider uXSlider3 = roundTimeSlider;
		uXSlider3.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider3.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			roundTimeValueText.Text = FormatRoundTime(s.Value);
		}));
		UXSlider uXSlider4 = roundTimeSlider;
		uXSlider4.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider4.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			roundTimeValueText.Text = FormatRoundTime(v);
		}));
		UXSlider uXSlider5 = maxPlayersSlider;
		uXSlider5.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider5.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			maxPlayersValueText.Text = s.Value + string.Empty;
		}));
		UXSlider uXSlider6 = maxPlayersSlider;
		uXSlider6.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider6.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			maxPlayersValueText.Text = v + string.Empty;
		}));
	}

	private void UpdateSettingsRange()
	{
		if (gameType == MVGameType.ADV || gameType == MVGameType.CHAT)
		{
			roundTimeSlider.Value = 0f;
			roundTimeValueText.Text = FormatRoundTime(roundTimeSlider.Value);
			((Component)roundTimeSlider).gameObject.SetActiveRecursively(false);
			return;
		}
		((Component)roundTimeSlider).gameObject.SetActiveRecursively(true);
		float num = ((gameType != MVGameType.DM) ? 0f : 60f);
		roundTimeSlider.MinValue = num;
		if (roundTimeSlider.Value < num)
		{
			roundTimeSlider.Value = num;
			roundTimeValueText.Text = FormatRoundTime(roundTimeSlider.Value);
		}
	}

	private void CreateGameTypeButton(MVGameType gameType)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		MVGUIGameTypeButton mVGUIGameTypeButton = Object.Instantiate((Object)(object)GameTypeButtonPrefab) as MVGUIGameTypeButton;
		mVGUIGameTypeButton.BuildButton(gameType);
		((Component)mVGUIGameTypeButton).transform.parent = GameTypeSelectRoot;
		((Component)mVGUIGameTypeButton).transform.localPosition = new Vector3(0f, (float)gameTypeButtons.Count * -2.5f, 0f);
		((Component)mVGUIGameTypeButton).transform.localScale = Vector3.one;
		mVGUIGameTypeButton.OnGameTypeSelect = (MVGUIGameTypeButton.OnGameTypeSelectDelegate)Delegate.Combine(mVGUIGameTypeButton.OnGameTypeSelect, new MVGUIGameTypeButton.OnGameTypeSelectDelegate(OnGameTypeButtonSelect));
		gameTypeButtons.Add(gameType, mVGUIGameTypeButton);
	}

	private void OnGameTypeButtonSelect(MVGUIGameTypeButton selectedGameTypeButton, MVGameType gameType)
	{
		foreach (MVGUIGameTypeButton value in gameTypeButtons.Values)
		{
			value.SetToggled(toggled: false);
		}
		selectedGameTypeButton.SetToggled(toggled: true);
		this.gameType = gameType;
		UpdateSettingsRange();
	}

	private string FormatGravity(float gravity)
	{
		return gravity.ToString();
	}

	private string FormatRoundTime(float time)
	{
		if (time == 0f)
		{
			return "Disabled";
		}
		return $"{time / 60f:00}:{time % 60f:00}";
	}

	public override object GetResult()
	{
		float value = gravitySlider.Value;
		int num = (int)roundTimeSlider.Value;
		if (gameType == MVGameType.ADV || gameType == MVGameType.CHAT)
		{
			num = 0;
		}
		int num2 = (int)maxPlayersSlider.Value;
		Hashtable hashtable = new Hashtable();
		hashtable.Add("gametype", gameType);
		hashtable.Add("gravity", value);
		hashtable.Add("roundTime", num);
		hashtable.Add("maxPlayers", num2);
		return hashtable;
	}
}
