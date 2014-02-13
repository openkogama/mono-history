using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class UXGUIElement : MonoBehaviour
{
	public float Width = 1f;

	public float Height = 1f;

	public bool Visible = true;

	public UXHorizontal horizontalAlign = UXHorizontal.Center;

	public UXVertical verticalAlign = UXVertical.Middle;

	public bool ignoreClipping;

	public bool uses9PatchMaterial;

	protected UXScreen screen;

	public Vector3 Alignment
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public Vector3 Size
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(Width, Height, 0f);
		}
	}

	public Vector3 ScreenAlignment
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return Alignment * screen.Scale;
		}
	}

	public Vector3 ScreenSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return Size * screen.Scale;
		}
	}

	public virtual void Awake()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		screen = UXUtils.FindGUIObjectOfType<UXScreen>();
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void SetMaterial(Material material)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		if ((Object)(object)((Component)this).renderer == (Object)null)
		{
			((Component)this).gameObject.AddComponent<MeshRenderer>();
		}
		((Component)this).renderer.material = new Material(material);
	}

	public virtual void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Color color = ((Component)this).renderer.material.GetColor("_MainColor");
		color.a = alpha;
		((Component)this).renderer.material.SetColor("_MainColor", color);
	}

	public virtual void SetVisible(bool visible)
	{
		Visible = visible;
		Renderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<Renderer>(true);
		foreach (Renderer val in componentsInChildren)
		{
			val.enabled = Visible;
		}
		Collider[] componentsInChildren2 = ((Component)this).GetComponentsInChildren<Collider>(true);
		foreach (Collider val2 in componentsInChildren2)
		{
			val2.enabled = Visible;
		}
	}

	public void SetSize(Vector2 size)
	{
		SetSize(size.x, size.y);
	}

	public virtual void SetSize(float width, float height)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Width = width;
		Height = height;
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void SetAlignment(UXHorizontal horizontal, UXVertical vertical)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		horizontalAlign = horizontal;
		verticalAlign = vertical;
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Rect clippedBounds = GetClippedBounds();
		if ((Object)(object)((Component)this).renderer != (Object)null)
		{
			((Component)this).renderer.material.SetVector("_ClipRect", new Vector4(clippedBounds.xMin, clippedBounds.yMin, clippedBounds.xMax, clippedBounds.yMax));
		}
	}

	protected virtual Mesh BuildMesh()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (uses9PatchMaterial)
		{
			return UXUtils.Build9PatchPlaneMesh(Alignment, ((Component)this).renderer.material.mainTexture, Width, Height, ((Object)((Component)this).gameObject).name + "9Patch");
		}
		return UXUtils.BuildPlaneMesh(Alignment, Width, Height, ((Object)((Component)this).gameObject).name);
	}

	public virtual Rect GetBoundingBox()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		float num = (0f - ScreenAlignment.x) * ((Component)this).transform.localScale.x;
		float num2 = (0f - ScreenAlignment.y) * ((Component)this).transform.localScale.y;
		float num3 = ScreenSize.x * ((Component)this).transform.localScale.x;
		float num4 = ScreenSize.y * ((Component)this).transform.localScale.y;
		Rect result = new Rect(num, num2, num3, num4);
		result.center += ((Component)this).transform.position.xy();
		return result;
	}

	protected Rect GetClippedBounds()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if (!Visible)
		{
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (ignoreClipping)
		{
			return Rect.MinMaxRect(-1000f, -1000f, 1000f, 1000f);
		}
		List<Rect> clipRects = new List<Rect>();
		if ((Object)(object)((Component)this).transform.parent != (Object)null)
		{
			GetClippedBoundsRecursive(((Component)((Component)this).transform.parent).gameObject, ref clipRects);
		}
		Rect result = Rect.MinMaxRect(-1000f, -1000f, 1000f, 1000f);
		foreach (Rect item in clipRects)
		{
			Rect current = item;
			if (current.xMin > result.xMin)
			{
				result.xMin = current.xMin;
			}
			if (current.xMax < result.xMax)
			{
				result.xMax = current.xMax;
			}
			if (current.yMin > result.yMin)
			{
				result.yMin = current.yMin;
			}
			if (current.yMax < result.yMax)
			{
				result.yMax = current.yMax;
			}
		}
		return result;
	}

	private void GetClippedBoundsRecursive(GameObject current, ref List<Rect> clipRects)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)current == (Object)null))
		{
			UXGUIElement component = current.GetComponent<UXGUIElement>();
			if ((Object)(object)component != (Object)null && component is IUXContainer)
			{
				clipRects.Add(component.GetBoundingBox());
			}
			if ((Object)(object)current.transform.parent != (Object)null)
			{
				GetClippedBoundsRecursive(((Component)current.transform.parent).gameObject, ref clipRects);
			}
		}
	}
}
