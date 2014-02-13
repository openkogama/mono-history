using System;
using Localize;
using UnityEngine;

public class UXTabPane : UXGUIElement
{
	public delegate void OnTabClickDelegate(int tabid);

	public OnTabClickDelegate OnTabClick;

	[SerializeField]
	public TextSlotIndex index = TextSlotIndex.Empty;

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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		_selected = new Material(_selected);
		_notSelected = new Material(_notSelected);
	}

	public void Show()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).renderer.material = _selected;
		uiHeaderText.Color = Color.white;
		_tabGroup.Show();
	}

	public void Hide()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).renderer.material = _notSelected;
		uiHeaderText.Color = Color.gray;
		_tabGroup.Hide();
	}

	public void BuildTab(float headerHeight, int tabId)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		_tabId = tabId;
		if (index != TextSlotIndex.Empty)
		{
			_headerText = Localization.Instance.GetText(index);
		}
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		uiHeaderText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Component)uiHeaderText).transform.parent = ((Component)this).transform;
		((Component)uiHeaderText).transform.localScale = new Vector3(_headerTextScale, _headerTextScale, _headerTextScale);
		uiHeaderText.Text = _headerText;
		uiHeaderText.ignoreClipping = true;
		uiHeaderText.SetVisible(Visible);
		if (_fitWidthToText)
		{
			Width = uiHeaderText.TextWidth;
		}
		Height = headerHeight;
		SetSize(Width, Height);
		((Component)this).gameObject.AddComponent<MeshRenderer>();
		((Component)this).renderer.material = _selected;
		((Component)this).renderer.enabled = Visible;
		MeshFilter val2 = ((Component)this).gameObject.AddComponent<MeshFilter>();
		val2.mesh = BuildMesh();
		((Component)uiHeaderText).transform.localPosition = new Vector3(0f, Height / 2f + 0.25f, -0.01f);
		UXUtils.AddComponentIfNotExists<BoxCollider>(((Component)this).gameObject);
		((Component)this).collider.enabled = Visible;
		UXMouseClickObject uXMouseClickObject = UXUtils.AddComponentIfNotExists<UXMouseClickObject>(((Component)this).gameObject);
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		uXMouseClickObject.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject.OnClick, new UXMouseClickObject.OnClickDelegate(HandleOnClick));
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		uiHeaderText.SetAlpha(alpha);
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
		if ((Object)(object)uiHeaderText != (Object)null)
		{
			uiHeaderText.SetVisible(visible);
		}
		if ((Object)(object)((Component)this).renderer != (Object)null)
		{
			((Component)this).renderer.enabled = visible;
		}
		if ((Object)(object)((Component)this).collider != (Object)null)
		{
			((Component)this).collider.enabled = visible;
		}
	}

	public string GetHeaderText()
	{
		return _headerText;
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
