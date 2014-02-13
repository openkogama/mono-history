using System;
using Localize;
using UnityEngine;

public class UXWindow : UXGUIElement, IUXContainer
{
	public delegate void OnExitButtonClickDelegate();

	public OnExitButtonClickDelegate OnExitButtonClick;

	public UXPresetWindowSize PresetWindowSize;

	public string headerText;

	private UXText uiHeaderText;

	public TextSlotIndex index = TextSlotIndex.Empty;

	public Color headerTextColor;

	private Vector3 headerPos;

	[SerializeField]
	private bool _hasExitButton = true;

	[SerializeField]
	private UXIconButton _exitButtonPrefab;

	[SerializeField]
	private float _exitButtonZOffset;

	private bool isInitialized;

	public bool HasExitButton
	{
		get
		{
			return _hasExitButton;
		}
		set
		{
			_hasExitButton = value;
			if (isInitialized)
			{
				UXIconButton exitButton = GetExitButton();
				if (_hasExitButton && (Object)(object)exitButton == (Object)null)
				{
					AddExitButton();
				}
				else if (!_hasExitButton && (Object)(object)exitButton != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)exitButton).gameObject);
				}
			}
		}
	}

	public override void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.Awake();
		if (PresetWindowSize != UXPresetWindowSize.None)
		{
			SetSize(GetPresetWindowSize());
		}
	}

	public virtual void Start()
	{
		MeshFilter val = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val.mesh = BuildMesh();
		BuildHeaderText();
		if (_hasExitButton)
		{
			AddExitButton();
		}
		isInitialized = true;
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		MeshFilter component = ((Component)this).gameObject.GetComponent<MeshFilter>();
		if ((Object)(object)component != (Object)null)
		{
			component.mesh = BuildMesh();
		}
		if (_hasExitButton)
		{
			PlaceExitButton();
		}
	}

	private void BuildHeaderText()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (index != TextSlotIndex.Empty)
		{
			headerText = Localization.Instance.GetText(index);
		}
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		uiHeaderText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Object)((Component)uiHeaderText).gameObject).name = "HeaderText";
		((Component)uiHeaderText).transform.parent = ((Component)this).transform;
		((Component)uiHeaderText).transform.localScale = Vector3.one;
		((Component)uiHeaderText).transform.localPosition = ((!(headerPos != Vector3.zero)) ? GetPresetHeaderPosition() : headerPos);
		uiHeaderText.verticalAlign = UXVertical.Top;
		uiHeaderText.horizontalAlign = UXHorizontal.Center;
		uiHeaderText.TextSize = UXTextSize.Large;
		uiHeaderText.Text = headerText;
		uiHeaderText.Color = headerTextColor;
	}

	public void SetHeaderText(string headerText)
	{
		this.headerText = headerText;
		if ((Object)(object)uiHeaderText != (Object)null)
		{
			uiHeaderText.Text = this.headerText;
		}
	}

	public void MoveHeader(Vector3 headerPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		this.headerPos = headerPos;
		if ((Object)(object)uiHeaderText != (Object)null)
		{
			((Component)uiHeaderText).transform.localPosition = this.headerPos;
		}
	}

	public void AddExitButton()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		UXIconButton uXIconButton = Object.Instantiate((Object)(object)_exitButtonPrefab) as UXIconButton;
		((Object)uXIconButton).name = "ExitButton";
		((Component)uXIconButton).transform.parent = ((Component)this).transform;
		((Component)uXIconButton).transform.localScale = Vector3.one;
		PlaceExitButton();
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			if (OnExitButtonClick != null)
			{
				OnExitButtonClick();
			}
		}));
	}

	private void PlaceExitButton()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		UXIconButton exitButton = GetExitButton();
		if ((Object)(object)exitButton != (Object)null)
		{
			((Component)exitButton).transform.localPosition = new Vector3(Width / 2f, Height / 2f, -0.1f) - new Vector3(2.5f, 2f, _exitButtonZOffset);
		}
	}

	public UXIconButton GetExitButton()
	{
		Transform val = ((Component)this).transform.FindChild("ExitButton");
		if ((Object)(object)val != (Object)null)
		{
			return ((Component)val).GetComponent<UXIconButton>();
		}
		return null;
	}

	private Vector2 GetPresetWindowSize()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		return PresetWindowSize switch
		{
			UXPresetWindowSize.Small => new Vector2(35f, 15f), 
			UXPresetWindowSize.Medium => new Vector2(35f, 22f), 
			UXPresetWindowSize.MediumWide => new Vector2(45f, 22f), 
			UXPresetWindowSize.Large => new Vector2(50f, 25f), 
			_ => new Vector2(1f, 1f), 
		};
	}

	private Vector3 GetPresetHeaderPosition()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		return PresetWindowSize switch
		{
			UXPresetWindowSize.Large => new Vector3((0f - Width) / 2f, Height / 2f, -0.1f) + new Vector3(10f, -1f, 0f), 
			_ => new Vector3(0f, Height / 2f - 1f, -0.1f), 
		};
	}
}
