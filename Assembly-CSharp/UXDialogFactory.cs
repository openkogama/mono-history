using System;
using System.Collections.Generic;
using Localize;
using UnityEngine;

public class UXDialogFactory : MonoBehaviour
{
	public float depth;

	public Transform DialogRoot;

	public Vector2 MinimumDialogBoxSize = new Vector2(25f, 12f);

	public Vector2 EdgeBuffer = Vector2.op_Implicit(new Vector3(5f, 2f));

	public float ButtonsOffset = 2.5f;

	public float MinButtonWidth = 5f;

	public float ButtonMargin = 0.5f;

	private float ButtonHeight = 2.2f;

	public UXDialogBox SimpleDialogPrefab;

	public UXDialogBox TextFieldDialogPrefab;

	public UXDialogBox ToggleDialogPrefab;

	public UXDialogBox ComboBoxDialogPrefab;

	public UXTextButton ButtonPrefab;

	private Stack<UXDialogBox> dialogs = new Stack<UXDialogBox>();

	private List<UXTextButton> currentDialogButtons = new List<UXTextButton>();

	private bool allowNoButtons;

	private GameObject go;

	private BoxCollider boxCollider;

	private UXScreen screen;

	private bool screenLock;

	public bool DialogOpen => dialogs.Count > 0;

	public UXDialogBox CurrentDialogBox
	{
		get
		{
			if (dialogs.Count == 0)
			{
				return null;
			}
			return dialogs.Peek();
		}
	}

	public UXDialogBox CurrentlyBuildingDialogBox { get; private set; }

	public UXDialogFactory()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Awake()
	{
		screen = UXUtils.FindGUIObjectOfType<UXScreen>();
		UXScreen uXScreen = screen;
		uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(UpdatePlacement));
		if ((Object)(object)go == (Object)null)
		{
			InitializeCollider();
		}
	}

	private void InitializeCollider()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		go = new GameObject("Collider");
		boxCollider = go.AddComponent<BoxCollider>();
		UXMouseClickObject uXMouseClickObject = go.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true;
		uXMouseClickObject.OnClick = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
		};
		go.layer = LayerMask.NameToLayer("UXElement");
		go.transform.parent = ((Component)this).transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.active = false;
	}

	public void UpdatePlacement()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		go.transform.position = screen.GetPosition(UXHorizontal.Center, UXVertical.Middle, depth - 2f);
		Vector3 val = screen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - screen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		boxCollider.size = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), 10f);
	}

	public UXDialogFactory CreateDialog(TextSlotIndex messageIndex, TextSlotIndex headerIndex = TextSlotIndex.Empty, UXDialogType dialogType = UXDialogType.Simple, bool noButtons = false, bool stackDialog = false, bool canClose = true, ValueInsert values = null)
	{
		string textWithValues = Localization.Instance.GetTextWithValues(messageIndex, values);
		string textWithValues2 = Localization.Instance.GetTextWithValues(headerIndex, values);
		return BuildDialog(textWithValues, textWithValues2, dialogType, noButtons, stackDialog, canClose, values);
	}

	public UXDialogFactory CreateDevelopmentDialog(string message, string header, UXDialogType dialogType = UXDialogType.Simple, bool noButtons = false, bool stackDialog = false, bool canClose = true, ValueInsert values = null)
	{
		if (!Debug.isDebugBuild)
		{
			header = "Localize Error : " + header;
		}
		return BuildDialog(message, header, dialogType, noButtons, stackDialog, canClose, values);
	}

	public UXDialogFactory BuildDialog(string text, string header, UXDialogType dialogType = UXDialogType.Simple, bool noButtons = false, bool stackDialog = false, bool canClose = true, ValueInsert values = null)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CurrentDialogBox != (Object)null && !stackDialog)
		{
			return null;
		}
		DestroyBuildingDialog();
		CurrentlyBuildingDialogBox = AddDialog(dialogType);
		((Component)CurrentlyBuildingDialogBox).transform.parent = DialogRoot;
		((Component)CurrentlyBuildingDialogBox).transform.localPosition = Vector3.zero;
		((Component)CurrentlyBuildingDialogBox).transform.localScale = Vector3.one;
		UXText component = UXUtils.FindChild(((Component)CurrentlyBuildingDialogBox).gameObject, "Text").GetComponent<UXText>();
		component.Text = text;
		UXWindow component2 = UXUtils.FindChild(((Component)CurrentlyBuildingDialogBox).gameObject, "Window").GetComponent<UXWindow>();
		component2.SetHeaderText(header);
		allowNoButtons = noButtons;
		((Component)CurrentlyBuildingDialogBox).gameObject.SetActiveRecursively(false);
		currentDialogButtons.Clear();
		if (canClose)
		{
			component2.HasExitButton = true;
			component2.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(component2.OnExitButtonClick, (UXWindow.OnExitButtonClickDelegate)(() =>
			{
				FireResult(CurrentDialogBox.OnNegativeClose);
			}));
		}
		return this;
	}

	public UXDialogFactory CreateCustomDialog(string prefabPath, TextSlotIndex headerIndex = TextSlotIndex.Empty, bool noButtons = false, bool stackDialog = false, bool canClose = true)
	{
		return BuildCustomDialog(prefabPath, Localization.Instance.GetText(headerIndex), noButtons, stackDialog, canClose);
	}

	public UXDialogFactory CreateCustomDevelopmentDialog(string prefabPath, string header, bool noButtons = false, bool stackDialog = false, bool canClose = true)
	{
		if (!Debug.isDebugBuild)
		{
			header = "Localize Error : " + header;
		}
		return BuildCustomDialog(prefabPath, header, noButtons, stackDialog, canClose);
	}

	private UXDialogFactory BuildCustomDialog(string prefabPath, string header, bool noButtons = false, bool stackDialog = false, bool canClose = true)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CurrentDialogBox != (Object)null && !stackDialog)
		{
			return null;
		}
		DestroyBuildingDialog();
		CurrentlyBuildingDialogBox = AddDialog(prefabPath);
		((Component)CurrentlyBuildingDialogBox).transform.parent = DialogRoot;
		((Component)CurrentlyBuildingDialogBox).transform.localPosition = Vector3.zero;
		((Component)CurrentlyBuildingDialogBox).transform.localScale = Vector3.one;
		UXWindow component = UXUtils.FindChild(((Component)CurrentlyBuildingDialogBox).gameObject, "Window").GetComponent<UXWindow>();
		component.SetHeaderText(header);
		allowNoButtons = noButtons;
		((Component)CurrentlyBuildingDialogBox).gameObject.SetActiveRecursively(false);
		currentDialogButtons.Clear();
		if (canClose)
		{
			component.HasExitButton = true;
			component.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(component.OnExitButtonClick, (UXWindow.OnExitButtonClickDelegate)(() =>
			{
				FireResult(CurrentDialogBox.OnNegativeClose);
			}));
		}
		return this;
	}

	public UXDialogFactory AddPositiveButton(TextSlotIndex index, float buttonWidth = 0f)
	{
		string text = Localization.Instance.GetText(index);
		AddButton(text, buttonWidth, CurrentlyBuildingDialogBox.OnPositiveClose);
		return this;
	}

	public UXDialogFactory AddNegativeButton(TextSlotIndex index, float buttonWidth = 0f)
	{
		string text = Localization.Instance.GetText(index);
		AddButton(text, buttonWidth, CurrentlyBuildingDialogBox.OnNegativeClose);
		return this;
	}

	public UXDialogFactory SetOnResultCallback(UXDialogBox.OnDialogResult resultCallBack)
	{
		if (resultCallBack != null)
		{
			CurrentlyBuildingDialogBox.OnResult = resultCallBack;
		}
		return this;
	}

	public UXDialogFactory SetOnIntermediateResultCallback(UXDialogBox.OnDialogResult resultCallBack)
	{
		if (resultCallBack != null)
		{
			CurrentlyBuildingDialogBox.OnIntermediateResult = resultCallBack;
		}
		return this;
	}

	public UXDialogFactory SetValues(Dictionary<string, DialogData> elementData)
	{
		foreach (KeyValuePair<string, DialogData> elementDatum in elementData)
		{
			string key = elementDatum.Key;
			GameObject val = UXUtils.FindChild(((Component)CurrentlyBuildingDialogBox).gameObject, key);
			if ((Object)(object)val != (Object)null)
			{
				elementDatum.Value.ApplyDataToElement(val);
			}
		}
		return this;
	}

	public void DestroyBuildingDialog()
	{
		if ((Object)(object)CurrentlyBuildingDialogBox != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)CurrentlyBuildingDialogBox).gameObject);
			CurrentlyBuildingDialogBox = null;
			currentDialogButtons.Clear();
		}
	}

	public void Show()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)CurrentDialogBox != (Object)null)
		{
			((Component)CurrentDialogBox).gameObject.SetActiveRecursively(false);
		}
		if (currentDialogButtons.Count == 0 && !allowNoButtons)
		{
			AddPositiveButton(TextSlotIndex.Ok);
		}
		dialogs.Push(CurrentlyBuildingDialogBox);
		CurrentlyBuildingDialogBox = null;
		((Component)CurrentDialogBox).gameObject.SetActiveRecursively(true);
		Vector2 size = CurrentDialogBox.GetSize();
		if (currentDialogButtons.Count > 0)
		{
			GameObject val = UXUtils.FindChild(((Component)CurrentDialogBox).gameObject, "Buttons");
			val.transform.localPosition = new Vector3(0f - GetTotalButtonWidth() / 2f, (0f - size.y) / 2f + ButtonsOffset, -0.01f);
		}
		CurrentDialogBox.OnShowDialog();
		go.active = true;
		screenLock = Screen.lockCursor;
		Screen.lockCursor = false;
	}

	public void CloseDialog()
	{
		UXDialogBox currentDialogBox = CurrentDialogBox;
		dialogs.Pop();
		if (dialogs.Count > 0)
		{
			((Component)CurrentDialogBox).gameObject.SetActiveRecursively(true);
			CurrentDialogBox.OnShowDialog();
		}
		else
		{
			go.active = false;
			Screen.lockCursor = screenLock;
		}
		currentDialogBox.OnCloseDialog();
		Object.Destroy((Object)(object)((Component)currentDialogBox).gameObject);
	}

	private void AddButton(string buttonText, float buttonWidth, UXBaseButton.OnClickDelegate onClick)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = UXUtils.FindChild(((Component)CurrentlyBuildingDialogBox).gameObject, "Buttons").transform;
		UXTextButton uXTextButton = Object.Instantiate((Object)(object)ButtonPrefab) as UXTextButton;
		uXTextButton.FitButtonSizeToText = true;
		uXTextButton.Text = buttonText;
		float minButtonWidth = Mathf.Max(MinButtonWidth, buttonWidth);
		uXTextButton.FitToText(minButtonWidth, ButtonHeight);
		((Component)uXTextButton).transform.parent = transform;
		((Component)uXTextButton).transform.localScale = Vector3.one;
		((Component)uXTextButton).transform.localPosition = new Vector3(GetTotalButtonWidth() + ButtonMargin + uXTextButton.Width / 2f, 0f, 0f);
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			FireResult(onClick);
		}));
		currentDialogButtons.Add(uXTextButton);
	}

	private float GetTotalButtonWidth()
	{
		float num = 0f;
		foreach (UXTextButton currentDialogButton in currentDialogButtons)
		{
			num += currentDialogButton.Width;
		}
		return num + (float)(currentDialogButtons.Count - 1) * ButtonMargin;
	}

	private void FireResult(UXBaseButton.OnClickDelegate onClick)
	{
		onClick();
		CloseDialog();
	}

	private Vector2 GetRequiredDialogSize(Vector2 textBounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = textBounds;
		val = val + EdgeBuffer + new Vector2(0f, ButtonHeight);
		val.x = Mathf.Max(MinimumDialogBoxSize.x, val.x);
		val.y = Mathf.Max(MinimumDialogBoxSize.y, val.y);
		return val;
	}

	private UXDialogBox AddDialog(UXDialogType dialogType)
	{
		UXDialogBox result = null;
		switch (dialogType)
		{
		case UXDialogType.Simple:
			result = Object.Instantiate((Object)(object)SimpleDialogPrefab) as UXDialogBox;
			break;
		case UXDialogType.TextField:
			result = Object.Instantiate((Object)(object)TextFieldDialogPrefab) as UXDialogBox;
			break;
		case UXDialogType.Toggle:
			result = Object.Instantiate((Object)(object)ToggleDialogPrefab) as UXDialogBox;
			break;
		case UXDialogType.ComboBox:
			result = Object.Instantiate((Object)(object)ComboBoxDialogPrefab) as UXDialogBox;
			break;
		}
		return result;
	}

	private UXDialogBox AddDialog(string prefabPath)
	{
		UXDialogBox uXDialogBox = null;
		Object val = Object.Instantiate(Resources.Load(prefabPath));
		return ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXDialogBox>();
	}
}
