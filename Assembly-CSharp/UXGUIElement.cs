using System.Collections.Generic;
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

	public Vector3 Alignment { get; private set; }

	public Vector3 Size => new Vector3(Width, Height, 0f);

	public Vector3 ScreenAlignment => Alignment * screen.Scale;

	public Vector3 ScreenSize => Size * screen.Scale;

	public virtual void Awake()
	{
		screen = UXUtils.UXScreen;
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void SetMaterial(Material material)
	{
		if (GetComponent<Renderer>() == null)
		{
			gameObject.AddComponent<MeshRenderer>();
		}
		GetComponent<Renderer>().material = new Material(material);
	}

	public virtual void SetAlpha(float alpha, string materialProperty = "")
	{
		Color color = GetComponent<Renderer>().material.GetColor("_MainColor");
		color.a = alpha;
		GetComponent<Renderer>().material.SetColor("_MainColor", color);
	}

	public virtual void SetVisible(bool visible)
	{
		Visible = visible;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = Visible;
		}
		Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>(includeInactive: true);
		foreach (Collider collider in componentsInChildren2)
		{
			collider.enabled = Visible;
		}
	}

	public void SetSize(Vector2 size)
	{
		SetSize(size.x, size.y);
	}

	public virtual void SetSize(float width, float height)
	{
		Width = width;
		Height = height;
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void SetAlignment(UXHorizontal horizontal, UXVertical vertical)
	{
		horizontalAlign = horizontal;
		verticalAlign = vertical;
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void Update()
	{
		Rect clippedBounds = GetClippedBounds();
		if (GetComponent<Renderer>() != null)
		{
			GetComponent<Renderer>().material.SetVector("_ClipRect", new Vector4(clippedBounds.xMin, clippedBounds.yMin, clippedBounds.xMax, clippedBounds.yMax));
		}
	}

	protected virtual void BuildMesh(Mesh mesh)
	{
		if (uses9PatchMaterial)
		{
			UXUtils.Build9PatchPlaneMesh(mesh, Alignment, GetComponent<Renderer>().material.mainTexture, Width, Height, gameObject.name + "9Patch");
		}
		else
		{
			UXUtils.BuildPlaneMesh(mesh, Alignment, Width, Height, gameObject.name);
		}
	}

	public virtual Rect GetBoundingBox()
	{
		float left = (0f - ScreenAlignment.x) * transform.localScale.x;
		float top = (0f - ScreenAlignment.y) * transform.localScale.y;
		float width = ScreenSize.x * transform.localScale.x;
		float height = ScreenSize.y * transform.localScale.y;
		Rect result = new Rect(left, top, width, height);
		result.center += transform.position.xy();
		return result;
	}

	protected Rect GetClippedBounds()
	{
		if (!Visible)
		{
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (ignoreClipping)
		{
			return Rect.MinMaxRect(-1000f, -1000f, 1000f, 1000f);
		}
		List<Rect> clipRects = new List<Rect>();
		if (transform.parent != null)
		{
			GetClippedBoundsRecursive(transform.parent.gameObject, ref clipRects);
		}
		Rect result = Rect.MinMaxRect(-1000f, -1000f, 1000f, 1000f);
		foreach (Rect item in clipRects)
		{
			if (item.xMin > result.xMin)
			{
				result.xMin = item.xMin;
			}
			if (item.xMax < result.xMax)
			{
				result.xMax = item.xMax;
			}
			if (item.yMin > result.yMin)
			{
				result.yMin = item.yMin;
			}
			if (item.yMax < result.yMax)
			{
				result.yMax = item.yMax;
			}
		}
		return result;
	}

	private void GetClippedBoundsRecursive(GameObject current, ref List<Rect> clipRects)
	{
		if (!(current == null))
		{
			UXGUIElement component = current.GetComponent<UXGUIElement>();
			if (component != null && component is IUXContainer)
			{
				clipRects.Add(component.GetBoundingBox());
			}
			if (current.transform.parent != null)
			{
				GetClippedBoundsRecursive(current.transform.parent.gameObject, ref clipRects);
			}
		}
	}
}
