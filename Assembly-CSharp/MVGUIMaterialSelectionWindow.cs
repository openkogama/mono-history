using System;
using System.Collections.Generic;
using Localize;
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
		((Component)_window).gameObject.SetActiveRecursively(true);
		PlaceIndicator(_selectedMaterialIndicator, _selectedMaterialId);
		if (_pages != null)
		{
			_pages[_currentPage].Show();
		}
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	public override void OnHide()
	{
		((Component)_window).gameObject.SetActiveRecursively(false);
		if (_pages != null)
		{
			_pages[_currentPage].Hide();
		}
		if ((Object)(object)_mouseOverMaterialIndicator != (Object)null && (Object)(object)_selectedMaterialIndicator != (Object)null)
		{
			((Component)_mouseOverMaterialIndicator).gameObject.SetActiveRecursively(false);
			((Component)_selectedMaterialIndicator).gameObject.SetActiveRecursively(false);
		}
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
	}

	public override void OnInitialize()
	{
		_window = ((Component)this).gameObject.GetComponentInChildren<UXWindow>();
	}

	public void InitializeMaterialSelectionWindow()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected Obj, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
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
		GameObject val = new GameObject("PageRoot");
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = -GetPageSize() / 2f;
		val.transform.localScale = Vector3.one;
		_pageRoot = val.transform;
		_totalMaterialCount = MVGameController.Instance.Game.MaterialRepository.MaterialCount;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected Obj, but got Unknown
		GameObject val = new GameObject(name);
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)_pageRoot).transform;
		val.transform.localPosition = Vector3.zero;
		val.transform.localScale = Vector3.one;
		MeshRenderer val2 = val.AddComponent<MeshRenderer>();
		((Renderer)val2).material = new Material(IndicatorMaterial);
		UXPlane uXPlane = val.AddComponent<UXPlane>();
		uXPlane.uses9PatchMaterial = true;
		uXPlane.SetSize(INDICATOR_SIZE, INDICATOR_SIZE);
		uXPlane.SetAlpha(alpha, "_MainColor");
		return uXPlane;
	}

	private void BuildPage(int pageIndex)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Page #" + pageIndex);
		val.transform.parent = _pageRoot;
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.localPosition = Vector3.zero;
		val.transform.localScale = Vector3.one;
		UXGroup uXGroup = val.AddComponent<UXGroup>();
		uXGroup.Hide();
		AddMaterialsToPage(uXGroup, pageIndex * MaxCubesPerPage, Mathf.Min(_totalMaterialCount - pageIndex * MaxCubesPerPage, MaxCubesPerPage));
		_pages.Add(uXGroup);
	}

	private void AddMaterialsToPage(UXGroup pageGroup, int materialIdOffset, int materials)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < materials; i++)
		{
			Object val = Object.Instantiate((Object)(object)materialCubePrefab);
			GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
			val2.transform.parent = ((Component)pageGroup).transform;
			val2.transform.localPosition = GetMaterialCubePos(i / Columns, i % Columns);
			val2.transform.localScale = Vector3.one;
			MVGUIMaterialSelectionCube component = val2.GetComponent<MVGUIMaterialSelectionCube>();
			component.OnSelection = (MVGUIMaterialSelectionCube.OnMaterialDelegate)Delegate.Combine(component.OnSelection, new MVGUIMaterialSelectionCube.OnMaterialDelegate(OnMaterialSelectionHandler));
			component.OnMouseOver = (MVGUIMaterialSelectionCube.OnMaterialDelegate)Delegate.Combine(component.OnMouseOver, new MVGUIMaterialSelectionCube.OnMaterialDelegate(OnMaterialCubeMouseOver));
			component.MaterialId = i + materialIdOffset;
			component.SetVisible(visible: false);
		}
	}

	private Vector3 GetPageSize()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return (float)(Rows - 1) * VERTICAL_SPACING * Vector3.down + (float)(Columns - 1) * HORIZONTAL_SPACING * Vector3.right;
	}

	private Vector3 GetMaterialCubePos(int row, int column)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		return (float)row * VERTICAL_SPACING * Vector3.down + (float)column * HORIZONTAL_SPACING * Vector3.right + 5f * Vector3.back;
	}

	private void PlaceIndicator(UXPlane indicator, int materialId)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (materialId < _currentPage * MaxCubesPerPage || materialId >= (_currentPage + 1) * MaxCubesPerPage)
		{
			((Component)indicator).gameObject.SetActiveRecursively(false);
			return;
		}
		int num = materialId % MaxCubesPerPage;
		int row = num / Columns;
		int column = num % Columns;
		((Component)indicator).transform.localPosition = GetMaterialCubePos(row, column);
		((Component)indicator).gameObject.SetActiveRecursively(true);
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
		((Component)_mouseOverMaterialIndicator).gameObject.SetActiveRecursively(false);
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
		if ((Object)(object)_selectedMaterialIndicator != (Object)null && View.isVisible)
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
		MVMaterial material = MVGameController.Instance.Game.MaterialRepository.GetMaterial(b);
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
		if ((Object)(object)_purchaseCube != (Object)null)
		{
			_purchaseCube.StopKeyAnimation();
		}
		_purchaseCube = cube;
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/ProductShopDialog", TextSlotIndex.Empty, noButtons: true).SetOnResultCallback(OnMaterialPurchaseDialogResult).SetValues(BuildDialogData(mvMaterial, cube.GetPreviewCube()))
			.Show();
		MVGUIProductShopDialog mVGUIProductShopDialog = (MVGUIProductShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIProductShopDialog.SetPrice(mvMaterial.unlockPriceGold, mvMaterial.unlockPriceSilver);
		mVGUIProductShopDialog.OnTryPurchaseProduct = () =>
		{
			MVGameController.Instance.Game.UnlockMaterial(cube.MaterialId);
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
			MVGameController.Instance.Game.MaterialRepository.SetMaterialUnlocked(_purchaseCube.MaterialId, unlocked: true);
			_purchaseCube.StartKeyAnimation();
			SetSelectedMaterial(_purchaseCube.MaterialId);
			if (OnMaterialSelection != null)
			{
				OnMaterialSelection((byte)_purchaseCube.MaterialId);
			}
		}
	}
}
