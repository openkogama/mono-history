using System;
using UnityEngine;

public class UXWindow : UXGUIElement, IUXContainer
{
	public delegate void OnExitButtonClickDelegate();

	public OnExitButtonClickDelegate OnExitButtonClick;

	public UXPresetWindowSize PresetWindowSize;

	public string headerText;

	private UXText uiHeaderText;

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
				if (_hasExitButton && exitButton == null)
				{
					AddExitButton();
				}
				else if (!_hasExitButton && exitButton != null)
				{
					UnityEngine.Object.Destroy(exitButton.gameObject);
				}
			}
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (PresetWindowSize != UXPresetWindowSize.None)
		{
			SetSize(GetPresetWindowSize());
		}
	}

	public virtual void Start()
	{
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		BuildMesh(meshFilter.mesh);
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
		MeshFilter component = gameObject.GetComponent<MeshFilter>();
		if (component != null)
		{
			BuildMesh(component.mesh);
		}
		if (_hasExitButton)
		{
			PlaceExitButton();
		}
	}

	private void BuildHeaderText()
	{
		headerText = TM._(headerText);
		uiHeaderText = UnityEngine.Object.Instantiate(PrefabPool.Instance.UXTextObject).GetComponent<UXText>();
		uiHeaderText.gameObject.name = "HeaderText";
		uiHeaderText.transform.parent = transform;
		uiHeaderText.transform.localScale = Vector3.one;
		uiHeaderText.transform.localPosition = ((!(headerPos != Vector3.zero)) ? GetPresetHeaderPosition() : headerPos);
		uiHeaderText.verticalAlign = UXVertical.Top;
		uiHeaderText.horizontalAlign = UXHorizontal.Center;
		uiHeaderText.TextSize = UXTextSize.Large;
		uiHeaderText.Text = headerText;
		uiHeaderText.Color = headerTextColor;
	}

	public void SetHeaderText(string headerText)
	{
		this.headerText = headerText;
		if (uiHeaderText != null)
		{
			uiHeaderText.Text = this.headerText;
		}
	}

	public void MoveHeader(Vector3 headerPos)
	{
		this.headerPos = headerPos;
		if (uiHeaderText != null)
		{
			uiHeaderText.transform.localPosition = this.headerPos;
		}
	}

	public void AddExitButton()
	{
		UXIconButton uXIconButton = UnityEngine.Object.Instantiate(_exitButtonPrefab);
		uXIconButton.name = "ExitButton";
		uXIconButton.transform.parent = transform;
		uXIconButton.transform.localScale = Vector3.one;
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
		UXIconButton exitButton = GetExitButton();
		if (exitButton != null)
		{
			exitButton.transform.localPosition = new Vector3(Width / 2f, Height / 2f, -0.1f) - new Vector3(2.5f, 2f, _exitButtonZOffset);
		}
	}

	public UXIconButton GetExitButton()
	{
		Transform transform = base.transform.FindChild("ExitButton");
		if (transform != null)
		{
			return transform.GetComponent<UXIconButton>();
		}
		return null;
	}

	private Vector2 GetPresetWindowSize()
	{
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
		return PresetWindowSize switch
		{
			UXPresetWindowSize.Large => new Vector3((0f - Width) / 2f, Height / 2f, -0.1f) + new Vector3(10f, -1f, 0f), 
			_ => new Vector3(0f, Height / 2f - 1f, -0.1f), 
		};
	}
}
