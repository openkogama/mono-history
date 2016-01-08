using System;
using UnityEngine;

public class UXTabPane : UXGUIElement
{
	public delegate void OnTabClickDelegate(int tabid);

	public OnTabClickDelegate OnTabClick;

	[SerializeField]
	private string _headerText;

	private UXText uiHeaderText;

	[SerializeField]
	private float _headerTextScale = 1f;

	[SerializeField]
	private UXGroup _tabGroup;

	[SerializeField]
	private bool _fitWidthToText;

	[SerializeField]
	private Material _selected;

	[SerializeField]
	private Material _notSelected;

	private int _tabId;

	public UXGroup TabGroup => _tabGroup;

	public void Start()
	{
		_selected = new Material(_selected);
		_notSelected = new Material(_notSelected);
	}

	public void Show()
	{
		GetComponent<Renderer>().material = _selected;
		uiHeaderText.Color = Color.white;
		_tabGroup.Show();
	}

	public void Hide()
	{
		GetComponent<Renderer>().material = _notSelected;
		uiHeaderText.Color = Color.gray;
		_tabGroup.Hide();
	}

	public void BuildTab(float headerHeight, int tabId)
	{
		_tabId = tabId;
		uiHeaderText = UnityEngine.Object.Instantiate(PrefabPool.Instance.UXTextObject).GetComponent<UXText>();
		uiHeaderText.transform.parent = transform;
		uiHeaderText.transform.localScale = new Vector3(_headerTextScale, _headerTextScale, _headerTextScale);
		Height = headerHeight;
		CreateHeaderText();
		TM.LanguageChanged(CreateHeaderText);
		gameObject.AddComponent<MeshRenderer>();
		GetComponent<Renderer>().material = _selected;
		GetComponent<Renderer>().enabled = Visible;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		BuildMesh(meshFilter.mesh);
		uiHeaderText.transform.localPosition = new Vector3(0f, Height / 2f + 0.25f, -0.01f);
		UXUtils.AddComponentIfNotExists<BoxCollider>(gameObject);
		GetComponent<Collider>().enabled = Visible;
		UXMouseClickObject uXMouseClickObject = UXUtils.AddComponentIfNotExists<UXMouseClickObject>(gameObject);
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		uXMouseClickObject.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject.OnClick, new UXMouseClickObject.OnClickDelegate(HandleOnClick));
	}

	private void CreateHeaderText()
	{
		uiHeaderText.Text = TM._(_headerText);
		uiHeaderText.ignoreClipping = true;
		uiHeaderText.SetVisible(Visible);
		if (_fitWidthToText)
		{
			Width = uiHeaderText.TextWidth;
		}
		SetSize(Width, Height);
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		uiHeaderText.SetAlpha(alpha, string.Empty);
		Color color = _selected.GetColor(materialProperty);
		Color color2 = _notSelected.GetColor(materialProperty);
		color.a = alpha;
		color2.a = alpha;
		_selected.SetColor(materialProperty, color);
		_notSelected.SetColor(materialProperty, color2);
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		if (uiHeaderText != null)
		{
			uiHeaderText.SetVisible(visible);
		}
		if (GetComponent<Renderer>() != null)
		{
			GetComponent<Renderer>().enabled = visible;
		}
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = visible;
		}
	}

	public string GetHeaderText()
	{
		return TM._(_headerText);
	}

	public void HandleOnClick(UXMouseClickObject mouseClickObject, Vector3 mousePositionWorld)
	{
		NotifyOnClick();
	}

	public void FireOnClick()
	{
		NotifyOnClick();
	}

	private void NotifyOnClick()
	{
		if (OnTabClick != null)
		{
			OnTabClick(_tabId);
		}
	}
}
