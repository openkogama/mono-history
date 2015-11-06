using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIMaterialSelectionWindow : UXViewScript
{
	public delegate void OnMaterialSelectionDelegate(byte materialId);

	public OnMaterialSelectionDelegate OnMaterialSelection;

	private float VERTICAL_SPACING = 5.5f;

	private float HORIZONTAL_SPACING = 6f;

	private float INDICATOR_SIZE = 5f;

	public GameObject materialCubePrefab;

	public UXTextButton nextButton;

	public UXTextButton prevButton;

	public UXText pageIndexText;

	public int Columns = 6;

	public int Rows = 3;

	public Material IndicatorMaterial;

	private UXPlane _selectedMaterialIndicator;

	private UXPlane _mouseOverMaterialIndicator;

	private UXWindow _window;

	private List<UXGroup> _pages;

	private Transform _pageRoot;

	private int _totalMaterialCount;

	private int _currentPage;

	private int _selectedMaterialId;

	private MVGUIMaterialSelectionCube _purchaseCube;

	private bool _isInitialized;

	private int MaxCubesPerPage => Columns * Rows;

	public override void OnShow()
	{
		if (!_isInitialized)
		{
			InitializeMaterialSelectionWindow();
		}
		_window.gameObject.SetActive(value: true);
		PlaceIndicator(_selectedMaterialIndicator, _selectedMaterialId);
		if (_pages != null)
		{
			_pages[_currentPage].Show();
		}
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	public override void OnHide()
	{
		_window.gameObject.SetActive(value: false);
		if (_pages != null)
		{
			_pages[_currentPage].Hide();
		}
		if (_mouseOverMaterialIndicator != null && _selectedMaterialIndicator != null)
		{
			_mouseOverMaterialIndicator.gameObject.SetActive(value: false);
			_selectedMaterialIndicator.gameObject.SetActive(value: false);
		}
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
	}

	public override void OnInitialize()
	{
		MVGUIMaterialSelectionCube.GreyedOutMaterial = (Material)Resources.Load("Materials/GreyedOutMaterial");
		_window = gameObject.GetComponentInChildren<UXWindow>();
	}

	public void InitializeMaterialSelectionWindow()
	{
		UXTextButton uXTextButton = nextButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ChangePage(1);
		}));
		UXTextButton uXTextButton2 = prevButton;
		uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ChangePage(-1);
		}));
		UXWindow window = _window;
		window.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(window.OnExitButtonClick, (UXWindow.OnExitButtonClickDelegate)(() =>
		{
			View.Hide();
		}));
		GameObject gameObject = new GameObject("PageRoot");
		gameObject.transform.parent = transform;
		gameObject.transform.localPosition = -GetPageSize() / 2f;
		gameObject.transform.localScale = Vector3.one;
		_pageRoot = gameObject.transform;
		_totalMaterialCount = MVGameControllerBase.Game.MaterialRepository.MaterialCount;
		_pages = new List<UXGroup>();
		int num = _totalMaterialCount / MaxCubesPerPage;
		if (_totalMaterialCount % MaxCubesPerPage != 0)
		{
			num++;
		}
		for (int num2 = 0; num2 < num; num2++)
		{
			BuildPage(num2);
		}
		_selectedMaterialIndicator = BuildIndicator("SelectedMaterialIndicator", 1f);
		PlaceIndicator(_selectedMaterialIndicator, _selectedMaterialId);
		_mouseOverMaterialIndicator = BuildIndicator("MouseOverMaterialIndicator", 0.2f);
		UpdatePageIndex();
		_isInitialized = true;
	}

	private UXPlane BuildIndicator(string name, float alpha)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = _pageRoot.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(IndicatorMaterial);
		UXPlane uXPlane = gameObject.AddComponent<UXPlane>();
		uXPlane.uses9PatchMaterial = true;
		uXPlane.SetSize(INDICATOR_SIZE, INDICATOR_SIZE);
		uXPlane.SetAlpha(alpha, "_MainColor");
		return uXPlane;
	}

	private void BuildPage(int pageIndex)
	{
		GameObject gameObject = new GameObject("Page #" + pageIndex);
		gameObject.transform.parent = _pageRoot;
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		UXGroup uXGroup = gameObject.AddComponent<UXGroup>();
		uXGroup.Hide();
		AddMaterialsToPage(uXGroup, pageIndex * MaxCubesPerPage, Mathf.Min(_totalMaterialCount - pageIndex * MaxCubesPerPage, MaxCubesPerPage));
		_pages.Add(uXGroup);
	}

	private void AddMaterialsToPage(UXGroup pageGroup, int materialIdOffset, int materials)
	{
		for (int i = 0; i < materials; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(materialCubePrefab);
			gameObject.transform.parent = pageGroup.transform;
			gameObject.transform.localPosition = GetMaterialCubePos(i / Columns, i % Columns);
			gameObject.transform.localScale = Vector3.one;
			MVGUIMaterialSelectionCube component = gameObject.GetComponent<MVGUIMaterialSelectionCube>();
			component.OnSelection = (MVGUIMaterialSelectionCube.OnMaterialDelegate)Delegate.Combine(component.OnSelection, new MVGUIMaterialSelectionCube.OnMaterialDelegate(OnMaterialSelectionHandler));
			component.OnMouseOver = (MVGUIMaterialSelectionCube.OnMaterialDelegate)Delegate.Combine(component.OnMouseOver, new MVGUIMaterialSelectionCube.OnMaterialDelegate(OnMaterialCubeMouseOver));
			component.MaterialId = i + materialIdOffset;
			component.SetVisible(visible: false);
		}
	}

	private Vector3 GetPageSize()
	{
		return (float)(Rows - 1) * VERTICAL_SPACING * Vector3.down + (float)(Columns - 1) * HORIZONTAL_SPACING * Vector3.right;
	}

	private Vector3 GetMaterialCubePos(int row, int column)
	{
		return (float)row * VERTICAL_SPACING * Vector3.down + (float)column * HORIZONTAL_SPACING * Vector3.right + 5f * Vector3.back;
	}

	private void PlaceIndicator(UXPlane indicator, int materialId)
	{
		if (materialId < _currentPage * MaxCubesPerPage || materialId >= (_currentPage + 1) * MaxCubesPerPage)
		{
			indicator.gameObject.SetActive(value: false);
			return;
		}
		int num = materialId % MaxCubesPerPage;
		int row = num / Columns;
		int column = num % Columns;
		indicator.transform.localPosition = GetMaterialCubePos(row, column);
		indicator.gameObject.SetActive(value: true);
	}

	private void ChangePage(int relativePage)
	{
		int currentPage = _currentPage;
		currentPage += relativePage;
		currentPage = Mathf.Clamp(currentPage, 0, _pages.Count - 1);
		if (currentPage != _currentPage)
		{
			GoToPage(currentPage);
		}
	}

	private void GoToPage(int pageIndex)
	{
		_pages[_currentPage].Hide();
		_currentPage = pageIndex;
		_pages[_currentPage].Show();
		_mouseOverMaterialIndicator.gameObject.SetActive(value: false);
		PlaceIndicator(_selectedMaterialIndicator, _selectedMaterialId);
		UpdatePageIndex();
	}

	private void UpdatePageIndex()
	{
		pageIndexText.Text = $"{_currentPage + 1}/{_pages.Count}";
	}

	public void SetSelectedMaterial(int materialId)
	{
		_selectedMaterialId = materialId;
		if (_selectedMaterialIndicator != null && View.isVisible)
		{
			PlaceIndicator(_selectedMaterialIndicator, _selectedMaterialId);
		}
	}

	public void OnMaterialCubeMouseOver(MVGUIMaterialSelectionCube cube)
	{
		PlaceIndicator(_mouseOverMaterialIndicator, cube.MaterialId);
	}

	public void OnMaterialSelectionHandler(MVGUIMaterialSelectionCube cube)
	{
		byte b = (byte)cube.MaterialId;
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(b);
		if (material.isUnlocked)
		{
			SetSelectedMaterial(b);
			if (OnMaterialSelection != null)
			{
				OnMaterialSelection(b);
			}
			View.Hide();
		}
		else
		{
			ShowMaterialPurchaseDialog(cube, material);
		}
	}

	private void ShowMaterialPurchaseDialog(MVGUIMaterialSelectionCube cube, MVMaterial mvMaterial)
	{
		if (_purchaseCube != null)
		{
			_purchaseCube.StopKeyAnimation();
		}
		_purchaseCube = cube;
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/ProductShopDialog", string.Empty, noButtons: true).SetOnResultCallback(OnMaterialPurchaseDialogResult).SetValues(BuildDialogData(mvMaterial, cube.GetPreviewCube()))
			.Show();
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetPrice(mvMaterial.unlockPriceGold, mvMaterial.unlockPriceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameControllerBase.Game.UnlockMaterial(cube.MaterialId);
		};
		View.Hide();
	}

	private Dictionary<string, DialogData> BuildDialogData(MVMaterial material, GameObject previewCube)
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ProductName", new TextData
		{
			text = material.name,
			useWordWrap = true
		});
		dictionary.Add("ProductDescription", new TextData
		{
			text = material.description.Replace("\\n", "\n"),
			useWordWrap = true
		});
		dictionary.Add("ProductPreview", new ProductPreviewData
		{
			productPreview = previewCube
		});
		return dictionary;
	}

	private void OnMaterialPurchaseDialogResult(UXDialogBox dialogBox)
	{
		View.Show();
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameControllerBase.Game.MaterialRepository.SetMaterialUnlocked(_purchaseCube.MaterialId, unlocked: true);
			_purchaseCube.StartKeyAnimation();
			SetSelectedMaterial(_purchaseCube.MaterialId);
			if (OnMaterialSelection != null)
			{
				OnMaterialSelection((byte)_purchaseCube.MaterialId);
			}
		}
	}
}
