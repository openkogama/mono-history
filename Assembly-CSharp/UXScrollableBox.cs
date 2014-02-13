using System.Collections.Generic;
using UnityEngine;

public class UXScrollableBox : UXGUIElement, IUXContainer
{
	public delegate void OnLinesChangedDelegate();

	public OnLinesChangedDelegate OnLinesChanged;

	public bool showBackground;

	public bool colorAlternateLines;

	public float backgroundLineHeight = 2f;

	public Color alternateLineColor = Color.gray;

	public Material lineMaterial;

	public ScrollBoxBehaviour ScrollBehaviour = ScrollBoxBehaviour.ScrollOnEnd;

	public LineAlignment lineAlignment = LineAlignment.Middle;

	public bool hideSliderWhenFull;

	public GameObject SliderPrefab;

	public Vector3 SliderOffset;

	private GameObject sliderObject;

	private UXSlider uiSlider;

	private float sliderWidth = 1f;

	private List<UXLine> Lines = new List<UXLine>();

	private float currentHeight;

	private Transform boxItemsOffset;

	private Transform bgLinesOffset;

	private float totalLinesHeight;

	public UXScrollableBox()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Awake()
	{
		base.Awake();
		Initialize();
	}

	public void Start()
	{
		UpdateVisibility();
	}

	public void Initialize()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		InitializeBackground();
		InitializeSlider();
		boxItemsOffset = new GameObject("ItemsOffset").transform;
		boxItemsOffset.parent = ((Component)this).transform;
		boxItemsOffset.localPosition = new Vector3(0f, Height, 0f) - Alignment;
		boxItemsOffset.localScale = Vector3.one;
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		if (showBackground)
		{
			base.SetAlpha(alpha, materialProperty);
		}
		uiSlider.SetAlpha(alpha, materialProperty);
		foreach (UXLine line in Lines)
		{
			line.SetAlpha(alpha, materialProperty);
		}
	}

	private void InitializeBackground()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter val = UXUtils.AddComponentIfNotExists<MeshFilter>(((Component)this).gameObject);
		val.mesh = BuildMesh();
		if (showBackground)
		{
			((Renderer)((Component)this).GetComponent<MeshRenderer>()).material.SetColor("_MainColor", Color.white);
		}
		if (colorAlternateLines)
		{
			InitializeBackgroundLines();
		}
	}

	private void InitializeBackgroundLines()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("BGLines");
		bgLinesOffset = val.transform;
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = new Vector3(Width / 2f - Alignment.x, Height - Alignment.y - 1f, -0.01f);
		val.transform.localScale = Vector2.op_Implicit(Vector2.one);
		for (int i = 0; (float)i < Height / backgroundLineHeight + 2f; i += 2)
		{
			CreateBGLine(val.transform, new Vector2(0f, (0f - backgroundLineHeight) * (float)(i + 1)), Width, backgroundLineHeight, alternateLineColor);
		}
	}

	private void CreateBGLine(Transform parent, Vector2 offset, float width, float height, Color color)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("BGLine");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = parent;
		val.transform.localPosition = Vector2.op_Implicit(offset);
		val.transform.localScale = Vector2.op_Implicit(Vector2.one);
		MeshRenderer val2 = val.AddComponent<MeshRenderer>();
		((Renderer)val2).material = lineMaterial;
		UXPlane uXPlane = val.AddComponent<UXPlane>();
		uXPlane.SetSize(width, height);
		uXPlane.SetColor(color, string.Empty);
	}

	private void InitializeSlider()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)SliderPrefab, Vector3.zero, Quaternion.identity);
		sliderObject = (GameObject)(object)((val is GameObject) ? val : null);
		sliderObject.transform.parent = ((Component)this).gameObject.transform;
		sliderObject.transform.localPosition = new Vector3(Width + sliderWidth / 2f, Height / 2f - SliderOffset.y / 2f, -0.5f) - Alignment + SliderOffset;
		sliderObject.transform.localScale = Vector3.one;
		uiSlider = sliderObject.GetComponent<UXSlider>();
		uiSlider.Initialize();
		uiSlider.SetSize(sliderWidth, Height + SliderOffset.y);
		uiSlider.OnValueChangedIntermediate = Slide;
		uiSlider.OnValueChanged = SlideEnd;
		uiSlider.MaxValue = 100f;
		if (ScrollBehaviour != ScrollBoxBehaviour.NoScroll)
		{
			uiSlider.Value = uiSlider.MaxValue;
		}
		UpdateSlider();
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		UpdateVisibility();
	}

	public override void SetSize(float width, float height)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		base.SetSize(width, height);
		if ((Object)(object)bgLinesOffset != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bgLinesOffset).gameObject);
		}
		Object.Destroy((Object)(object)sliderObject);
		uiSlider = null;
		InitializeBackground();
		InitializeSlider();
		boxItemsOffset.localPosition = new Vector3(0f, Height, 0f) - Alignment;
	}

	private void CalculateTotalLinesHeight()
	{
		totalLinesHeight = ((Lines.Count <= 0) ? 0f : FindLinesHeight(0, Lines.Count));
	}

	private float FindLinesHeight(int start, int stop)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = start; i < stop; i++)
		{
			num += Lines[i].GetLineSize().y;
		}
		return num;
	}

	public void SetSliderEnabled(bool enabled)
	{
		((Component)uiSlider).gameObject.SetActiveRecursively(enabled);
	}

	public void SetSliderValue(float val)
	{
		uiSlider.Value = val;
		SlideEnd(uiSlider);
	}

	public void FocusOnLine(UXLine line)
	{
		if (Lines.Contains(line))
		{
			FocusOnLine(Lines.IndexOf(line));
		}
	}

	public void FocusOnLine(int line)
	{
		float num = line / Lines.Count;
		uiSlider.Value = uiSlider.MaxValue * num;
		Slide(uiSlider, uiSlider.Value);
	}

	public List<UXLine> GetLines()
	{
		return Lines;
	}

	public void AddLine(UXLine line)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		((Component)line).gameObject.layer = LayerMask.NameToLayer("UXElement");
		((Component)line).gameObject.transform.parent = boxItemsOffset;
		((Component)line).gameObject.transform.localPosition = new Vector3(GetLineInsert(line), 0f - totalLinesHeight - line.GetLineSize().y / 2f, -0.01f);
		((Component)line).gameObject.transform.localScale = Vector3.one;
		Lines.Add(line);
		bool flag = uiSlider.Value == uiSlider.MaxValue;
		CalculateTotalLinesHeight();
		UpdateSlider();
		if (ScrollBehaviour == ScrollBoxBehaviour.ScrollOnNew || (flag && ScrollBehaviour == ScrollBoxBehaviour.ScrollOnEnd))
		{
			uiSlider.Value = uiSlider.MaxValue;
			Slide(uiSlider, uiSlider.Value);
		}
		else
		{
			Align();
		}
		UpdateVisibility();
		if (OnLinesChanged != null)
		{
			OnLinesChanged();
		}
	}

	private float GetLineInsert(UXLine line)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		return lineAlignment switch
		{
			LineAlignment.Left => 0f + line.Alignment.x, 
			LineAlignment.Middle => Width / 2f - line.Width / 2f + line.Alignment.x, 
			LineAlignment.Right => Width - line.Width + line.Alignment.x, 
			_ => 0f, 
		};
	}

	public void RemoveLine(int index, bool destroy = true)
	{
		RemoveLine(Lines[index], destroy);
	}

	public void RemoveLine(UXLine line, bool destroy = true)
	{
		Lines.Remove(line);
		if (destroy)
		{
			line.DestroyLine();
			Object.Destroy((Object)(object)((Component)line).gameObject);
		}
		CalculateTotalLinesHeight();
		if (OnLinesChanged != null)
		{
			OnLinesChanged();
		}
	}

	public void RemoveAllLines(bool destroy = true)
	{
		if (destroy)
		{
			foreach (UXLine line in Lines)
			{
				line.DestroyLine();
				Object.Destroy((Object)(object)((Component)line).gameObject);
			}
		}
		Lines.Clear();
		CalculateTotalLinesHeight();
		if (OnLinesChanged != null)
		{
			OnLinesChanged();
		}
	}

	public void SetLines(List<UXLine> lines)
	{
		RemoveAllLines();
		foreach (UXLine line in lines)
		{
			AddLine(line);
		}
	}

	private void UpdateSlider()
	{
		float num = Height / totalLinesHeight;
		float num2 = Mathf.Clamp((!(totalLinesHeight < Height)) ? (num * Height) : Height, 2f, Height);
		uiSlider.SetSliderSize(1f, num2);
		if (hideSliderWhenFull)
		{
			uiSlider.SetVisible(num2 != Height);
		}
	}

	private void Slide(UXSlider slider, float sliderValue)
	{
		currentHeight = (totalLinesHeight - Height) * (sliderValue / slider.MaxValue);
		if (totalLinesHeight < Height)
		{
			currentHeight = 0f;
		}
		Align();
	}

	private void SlideEnd(UXSlider slider)
	{
		Slide(slider, slider.Value);
	}

	private void Align()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)this == (Object)null))
		{
			boxItemsOffset.localPosition = new Vector3(0f, Height, 0f) - Alignment + new Vector3(0f, currentHeight, 0f);
			if (colorAlternateLines)
			{
				UpdateBGLines();
			}
			UpdateVisibility();
		}
	}

	private void UpdateBGLines()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = new Vector3(Width / 2f, Height, 0f) - Alignment + new Vector3(0f, (currentHeight - 1f) % 4f, 0f);
		localPosition.z = -0.01f;
		bgLinesOffset.localPosition = localPosition;
	}

	private void UpdateVisibility()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Rect boundingBox = GetBoundingBox();
		float yMax = boundingBox.yMax;
		float yMin = boundingBox.yMin;
		foreach (UXLine line in Lines)
		{
			Vector3 position = ((Component)line).transform.position;
			float num = position.y + line.ScreenSize.y / 2f;
			float num2 = position.y - line.ScreenSize.y / 2f;
			line.SetVisible(Visible && ((num <= yMax && num > yMin) || (num2 >= yMin && num2 < yMax)));
		}
	}
}
