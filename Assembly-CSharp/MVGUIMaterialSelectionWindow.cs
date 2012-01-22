using System;
using UnityEngine;

public class MVGUIMaterialSelectionWindow : UXViewScript
{
	private class Page
	{
		private GameObject root;

		private GameObject[] cubes;

		public bool IsVisible
		{
			set
			{
				root.SetActiveRecursively(value);
			}
		}

		public Page(MVGUIMaterialSelectionWindow window, int pageNumber)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected Obj, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			byte b = (byte)(pageNumber * 25);
			root = new GameObject($"Page {pageNumber}");
			root.transform.parent = ((Component)window).transform;
			root.transform.localPosition = Vector3.zero;
			int num = Mathf.Min(25, 28 - pageNumber * 25);
			cubes = new GameObject[num];
			MVMaterialRepository materialRepository = MVGameController.Instance.WOCM.MaterialRepository;
			for (int i = 0; i < num; i++)
			{
				int row = i / 5;
				int column = i % 5;
				Object val = Object.Instantiate((Object)(object)window.materialCubePrefab);
				GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
				val2.transform.parent = root.transform;
				val2.transform.localPosition = window.GetCubePosition(row, column);
				MVGUIMaterialSelectionCube component = val2.GetComponent<MVGUIMaterialSelectionCube>();
				component.Material = materialRepository.GetMaterial((byte)(i + b)).material;
				component.OnSelection = (OnMaterialDelegate)Delegate.Combine(component.OnSelection, new OnMaterialDelegate(window.OnMaterialSelectionHandler));
				component.OnMouseOver = (OnMaterialDelegate)Delegate.Combine(component.OnMouseOver, new OnMaterialDelegate(window.OnMaterialCubeMouseOver));
				component.materialId = (byte)(i + b);
				cubes[i] = val2;
			}
			IsVisible = false;
		}
	}

	private const int START_PAGE = 0;

	private const int START_SELECTED_MATERIAL_ID = 0;

	private const int NUMBER_OF_MATERIALS = 28;

	private const int NUMBER_OF_ROWS = 5;

	private const int NUMBER_OF_COLUMNS = 5;

	private const int MAX_NUMBER_OF_CUBES_PER_PAGE = 25;

	private const float spacing = 5f;

	public OnMaterialDelegate OnMaterialSelection;

	public GameObject materialCubePrefab;

	public UXWindow window;

	public Transform indicator;

	public Transform indicatorMouseOver;

	public UXButton nextButton;

	public UXButton previousButton;

	private GameObject[] cubes;

	private int NUMBER_OF_PAGES = 2;

	private int currentPage;

	private Page[] pages;

	private int selectedMaterialId;

	public static MVGUIMaterialSelectionWindow New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/MaterialSelectionWindow"));
		MVGUIMaterialSelectionWindow dialog = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIMaterialSelectionWindow>();
		dialog.Initialize();
		UXView uXView = dialog.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(dialog);
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(dialog);
		return dialog;
	}

	public new void Awake()
	{
		nextButton.OnClick = () =>
		{
			if (currentPage < 1)
			{
				GoToPage(1);
			}
		};
		previousButton.OnClick = () =>
		{
			if (currentPage > 0)
			{
				GoToPage(0);
			}
		};
	}

	public override void OnInitialize()
	{
		pages = new Page[NUMBER_OF_PAGES];
		for (int i = 0; i < NUMBER_OF_PAGES; i++)
		{
			pages[i] = new Page(this, i);
		}
		SetSelectedMaterialCube(0);
		GoToPage(0);
	}

	public override void OnShow()
	{
		((Component)this).gameObject.active = true;
		((Component)window).gameObject.SetActiveRecursively(true);
		((Component)indicator).gameObject.SetActiveRecursively(true);
		((Component)indicatorMouseOver).gameObject.SetActiveRecursively(true);
		((Component)nextButton).gameObject.SetActiveRecursively(true);
		((Component)previousButton).gameObject.SetActiveRecursively(true);
		UpdatePageVisibility();
	}

	private void UpdatePageVisibility()
	{
		for (int i = 0; i < pages.Length; i++)
		{
			pages[i].IsVisible = currentPage == i;
		}
		SetSelectedMaterialCube(selectedMaterialId);
		((Component)indicator).gameObject.SetActiveRecursively(selectedMaterialId / 25 == currentPage);
	}

	private void GoToPage(int pageNumber)
	{
		currentPage = pageNumber;
		UpdatePageVisibility();
		((Component)indicatorMouseOver).gameObject.SetActiveRecursively(false);
	}

	private Vector3 GetCubePosition(int row, int column)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		return ((float)row - 2f) * 5f * Vector3.right + ((float)column - 2f) * 5f * Vector3.down + 5f * Vector3.back;
	}

	public void SetSelectedMaterialCube(int materialId)
	{
		selectedMaterialId = materialId;
		PlaceTransform(indicator, materialId);
	}

	private void SetMouseOverIndicator(int materialId)
	{
		PlaceTransform(indicatorMouseOver, materialId);
		((Component)indicatorMouseOver).gameObject.SetActiveRecursively(true);
	}

	private void PlaceTransform(Transform transform, int materialId)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		int num = materialId - currentPage * 25;
		int row = num / 5;
		int column = num % 5;
		transform.localPosition = GetCubePosition(row, column);
	}

	public void OnMaterialSelectionHandler(int materialId)
	{
		SetSelectedMaterialCube(materialId);
		if (OnMaterialSelection != null)
		{
			OnMaterialSelection(materialId);
		}
		View.Hide();
	}

	public void OnMaterialCubeMouseOver(int materialId)
	{
		SetMouseOverIndicator(materialId);
	}
}
