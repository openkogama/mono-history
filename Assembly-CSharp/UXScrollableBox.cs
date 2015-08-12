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

	private int maxNumberOfLines = 30;

	public UXSlider UXSlider => uiSlider;

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
		InitializeBackground();
		InitializeSlider();
		boxItemsOffset = new GameObject("ItemsOffset").transform;
		boxItemsOffset.parent = transform;
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
		MeshFilter meshFilter = UXUtils.AddComponentIfNotExists<MeshFilter>(gameObject);
		BuildMesh(meshFilter.mesh);
		if (showBackground)
		{
			GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.white);
		}
		if (colorAlternateLines)
		{
			InitializeBackgroundLines();
		}
	}

	private void InitializeBackgroundLines()
	{
		GameObject gameObject = new GameObject("BGLines");
		bgLinesOffset = gameObject.transform;
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localPosition = new Vector3(Width / 2f - Alignment.x, Height - Alignment.y - 1f, -0.01f);
		gameObject.transform.localScale = Vector2.one;
		for (int i = 0; (float)i < Height / backgroundLineHeight + 2f; i += 2)
		{
			CreateBGLine(gameObject.transform, new Vector2(0f, (0f - backgroundLineHeight) * (float)(i + 1)), Width, backgroundLineHeight, alternateLineColor);
		}
	}

	private void CreateBGLine(Transform parent, Vector2 offset, float width, float height, Color color)
	{
		GameObject gameObject = new GameObject("BGLine");
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = offset;
		gameObject.transform.localScale = Vector2.one;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = lineMaterial;
		UXPlane uXPlane = gameObject.AddComponent<UXPlane>();
		uXPlane.SetSize(width, height);
		uXPlane.SetColor(color, string.Empty);
	}

	private void InitializeSlider()
	{
		sliderObject = Object.Instantiate(SliderPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		sliderObject.transform.parent = gameObject.transform;
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
		base.SetSize(width, height);
		if (bgLinesOffset != null)
		{
			Object.Destroy(bgLinesOffset.gameObject);
		}
		Object.Destroy(sliderObject);
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
		float num = 0f;
		for (int i = start; i < stop; i++)
		{
			num += Lines[i].GetLineSize().y;
		}
		return num;
	}

	public void SetSliderEnabled(bool enabled)
	{
		uiSlider.gameObject.SetActive(enabled);
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

	private void CalcLinePositions(UXLine line, float lineHeight)
	{
		line.gameObject.transform.parent = boxItemsOffset;
		line.gameObject.transform.localPosition = new Vector3(GetLineInsert(line), 0f - lineHeight - line.GetLineSize().y / 2f, -0.01f);
		line.gameObject.transform.localScale = Vector3.one;
	}

	public void AddLine(UXLine line)
	{
		if (Lines.Count >= maxNumberOfLines)
		{
			RemoveLine(0);
		}
		line.gameObject.layer = LayerMask.NameToLayer("UXElement");
		float num = 0f;
		foreach (UXLine line2 in Lines)
		{
			CalcLinePositions(line2, num);
			num += line2.GetLineSize().y;
		}
		CalcLinePositions(line, totalLinesHeight);
		Lines.Add(line);
		CalculateTotalLinesHeight();
		bool flag = uiSlider.Value == uiSlider.MaxValue;
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
			Object.Destroy(line.gameObject);
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
				Object.Destroy(line.gameObject);
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
		if (!(this == null))
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
		Vector3 localPosition = new Vector3(Width / 2f, Height, 0f) - Alignment + new Vector3(0f, (currentHeight - 1f) % 4f, 0f);
		localPosition.z = -0.01f;
		bgLinesOffset.localPosition = localPosition;
	}

	private void UpdateVisibility()
	{
		Rect boundingBox = GetBoundingBox();
		float yMax = boundingBox.yMax;
		float yMin = boundingBox.yMin;
		foreach (UXLine line in Lines)
		{
			Vector3 position = line.transform.position;
			float num = position.y + line.ScreenSize.y / 2f;
			float num2 = position.y - line.ScreenSize.y / 2f;
			line.SetVisible(Visible && ((num <= yMax && num > yMin) || (num2 >= yMin && num2 < yMax)));
		}
	}
}
