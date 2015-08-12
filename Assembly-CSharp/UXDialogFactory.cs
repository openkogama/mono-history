using System;
using System.Collections.Generic;
using UnityEngine;

public class UXDialogFactory : MonoBehaviour
{
	public float depth;

	public Transform DialogRoot;

	public Vector2 MinimumDialogBoxSize = new Vector2(25f, 12f);

	public Vector2 EdgeBuffer = new Vector3(5f, 2f);

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

	public void Awake()
	{
		screen = UXUtils.UXScreen;
		UXScreen uXScreen = screen;
		uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(UpdatePlacement));
		if (go == null)
		{
			InitializeCollider();
		}
	}

	private void InitializeCollider()
	{
		go = new GameObject("Collider");
		boxCollider = go.AddComponent<BoxCollider>();
		UXMouseClickObject uXMouseClickObject = go.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true;
		uXMouseClickObject.OnClick = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
		};
		go.layer = LayerMask.NameToLayer("UXElement");
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.SetActive(value: false);
	}

	public void UpdatePlacement()
	{
		go.transform.position = screen.GetPosition(UXHorizontal.Center, UXVertical.Middle, depth - 2f);
		Vector3 vector = screen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - screen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		boxCollider.size = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), 10f);
	}

	public UXDialogFactory CreateDialog(string message, string header = "", UXDialogType dialogType = UXDialogType.Simple, bool noButtons = false, bool stackDialog = false, bool canClose = true, ValueInsert values = null)
	{
		return BuildDialog(message, header, dialogType, noButtons, stackDialog, canClose, values);
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
		if (CurrentDialogBox != null && !stackDialog)
		{
			return null;
		}
		DestroyBuildingDialog();
		CurrentlyBuildingDialogBox = AddDialog(dialogType);
		CurrentlyBuildingDialogBox.transform.parent = DialogRoot;
		CurrentlyBuildingDialogBox.transform.localPosition = Vector3.zero;
		CurrentlyBuildingDialogBox.transform.localScale = Vector3.one;
		UXText component = UXUtils.FindChild(CurrentlyBuildingDialogBox.gameObject, "Text").GetComponent<UXText>();
		component.Text = text;
		UXWindow component2 = UXUtils.FindChild(CurrentlyBuildingDialogBox.gameObject, "Window").GetComponent<UXWindow>();
		component2.SetHeaderText(header);
		allowNoButtons = noButtons;
		CurrentlyBuildingDialogBox.gameObject.SetActive(value: false);
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

	public UXDialogFactory CreateCustomDialog(string prefabPath, string header = "", bool noButtons = false, bool stackDialog = false, bool canClose = true)
	{
		return BuildCustomDialog(prefabPath, header, noButtons, stackDialog, canClose);
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
		if (CurrentDialogBox != null && !stackDialog)
		{
			return null;
		}
		DestroyBuildingDialog();
		CurrentlyBuildingDialogBox = AddDialog(prefabPath);
		CurrentlyBuildingDialogBox.transform.parent = DialogRoot;
		CurrentlyBuildingDialogBox.transform.localPosition = Vector3.zero;
		CurrentlyBuildingDialogBox.transform.localScale = Vector3.one;
		UXWindow component = UXUtils.FindChild(CurrentlyBuildingDialogBox.gameObject, "Window").GetComponent<UXWindow>();
		component.SetHeaderText(header);
		allowNoButtons = noButtons;
		CurrentlyBuildingDialogBox.gameObject.SetActive(value: false);
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

	public UXDialogFactory AddPositiveButton(string txt, float buttonWidth = 0f)
	{
		string buttonText = TM._(txt);
		AddButton(buttonText, buttonWidth, CurrentlyBuildingDialogBox.OnPositiveClose);
		return this;
	}

	public UXDialogFactory AddNegativeButton(string txt, float buttonWidth = 0f)
	{
		string buttonText = TM._(txt);
		AddButton(buttonText, buttonWidth, CurrentlyBuildingDialogBox.OnNegativeClose);
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
			GameObject gameObject = UXUtils.FindChild(CurrentlyBuildingDialogBox.gameObject, key);
			if (gameObject != null)
			{
				elementDatum.Value.ApplyDataToElement(gameObject);
				continue;
			}
			throw new Exception("Could not find element with name: " + key);
		}
		return this;
	}

	public void DestroyBuildingDialog()
	{
		if (CurrentlyBuildingDialogBox != null)
		{
			UnityEngine.Object.Destroy(CurrentlyBuildingDialogBox.gameObject);
			CurrentlyBuildingDialogBox = null;
			currentDialogButtons.Clear();
		}
	}

	public void Show()
	{
		if (CurrentDialogBox != null)
		{
			CurrentDialogBox.gameObject.SetActive(value: false);
		}
		if (currentDialogButtons.Count == 0 && !allowNoButtons)
		{
			AddPositiveButton("Ok");
		}
		dialogs.Push(CurrentlyBuildingDialogBox);
		CurrentlyBuildingDialogBox = null;
		CurrentDialogBox.gameObject.SetActive(value: true);
		Vector2 size = CurrentDialogBox.GetSize();
		if (currentDialogButtons.Count > 0)
		{
			GameObject gameObject = UXUtils.FindChild(CurrentDialogBox.gameObject, "Buttons");
			gameObject.transform.localPosition = new Vector3(0f - GetTotalButtonWidth() / 2f, (0f - size.y) / 2f + ButtonsOffset, -0.01f);
		}
		CurrentDialogBox.OnShowDialog();
		go.SetActive(value: true);
	}

	public void CloseDialog()
	{
		UXDialogBox currentDialogBox = CurrentDialogBox;
		dialogs.Pop();
		if (dialogs.Count > 0)
		{
			CurrentDialogBox.gameObject.SetActive(value: true);
			CurrentDialogBox.OnShowDialog();
		}
		else
		{
			go.SetActive(value: false);
		}
		currentDialogBox.OnCloseDialog();
		UnityEngine.Object.Destroy(currentDialogBox.gameObject);
	}

	private void AddButton(string buttonText, float buttonWidth, UXBaseButton.OnClickDelegate onClick)
	{
		Transform parent = UXUtils.FindChild(CurrentlyBuildingDialogBox.gameObject, "Buttons").transform;
		UXTextButton uXTextButton = UnityEngine.Object.Instantiate(ButtonPrefab);
		uXTextButton.FitButtonSizeToText = true;
		uXTextButton.Text = buttonText;
		float minButtonWidth = Mathf.Max(MinButtonWidth, buttonWidth);
		uXTextButton.FitToText(minButtonWidth, ButtonHeight);
		uXTextButton.transform.parent = parent;
		uXTextButton.transform.localScale = Vector3.one;
		uXTextButton.transform.localPosition = new Vector3(GetTotalButtonWidth() + ButtonMargin + uXTextButton.Width / 2f, 0f, 0f);
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
		Vector2 vector = textBounds;
		vector = vector + EdgeBuffer + new Vector2(0f, ButtonHeight);
		vector.x = Mathf.Max(MinimumDialogBoxSize.x, vector.x);
		vector.y = Mathf.Max(MinimumDialogBoxSize.y, vector.y);
		return vector;
	}

	private UXDialogBox AddDialog(UXDialogType dialogType)
	{
		UXDialogBox result = null;
		switch (dialogType)
		{
		case UXDialogType.Simple:
			result = UnityEngine.Object.Instantiate(SimpleDialogPrefab);
			break;
		case UXDialogType.TextField:
			result = UnityEngine.Object.Instantiate(TextFieldDialogPrefab);
			break;
		case UXDialogType.Toggle:
			result = UnityEngine.Object.Instantiate(ToggleDialogPrefab);
			break;
		case UXDialogType.ComboBox:
			result = UnityEngine.Object.Instantiate(ComboBoxDialogPrefab);
			break;
		}
		return result;
	}

	private UXDialogBox AddDialog(string prefabPath)
	{
		UXDialogBox uXDialogBox = null;
		return (UnityEngine.Object.Instantiate(Resources.Load(prefabPath)) as GameObject).GetComponent<UXDialogBox>();
	}
}
