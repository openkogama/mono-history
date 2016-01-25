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

	protected Renderer mRenderer;

	private Rect boundingRect;

	private static List<Rect> g_clipRects = new List<Rect>(32);

	public Vector3 Alignment { get; private set; }

	public Vector3 Size => new Vector3(Width, Height, 0f);

	public Vector3 ScreenAlignment => Alignment * screen.Scale;

	public Vector3 ScreenSize => Size * screen.Scale;

	public Renderer Renderer
	{
		get
		{
			if (mRenderer == null)
			{
				mRenderer = GetComponent<Renderer>();
			}
			return mRenderer;
		}
	}

	public virtual void Awake()
	{
		screen = UXUtils.UXScreen;
		boundingRect = default;
		Alignment = new Vector3(UXEnums.GetRatio(horizontalAlign) * Width, UXEnums.GetRatio(verticalAlign) * Height, 0f);
	}

	public virtual void SetMaterial(Material material)
	{
		if (Renderer == null)
		{
			mRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		mRenderer.material = new Material(material);
	}

	public virtual void SetAlpha(float alpha, string materialProperty = "")
	{
		Color color = Renderer.material.GetColor("_MainColor");
		color.a = alpha;
		Renderer.material.SetColor("_MainColor", color);
	}

	public virtual void SetVisible(bool visible)
	{
		Visible = visible;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = Visible;
		}
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].enabled = Visible;
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
		if (mRenderer != null)
		{
			mRenderer.material.SetVector("_ClipRect", new Vector4(clippedBounds.xMin, clippedBounds.yMin, clippedBounds.xMax, clippedBounds.yMax));
		}
	}

	protected virtual void BuildMesh(Mesh mesh)
	{
		if (uses9PatchMaterial)
		{
			UXUtils.Build9PatchPlaneMesh(mesh, Alignment, Renderer.material.mainTexture, Width, Height, gameObject.name + "9Patch");
		}
		else
		{
			UXUtils.BuildPlaneMesh(mesh, Alignment, Width, Height, gameObject.name);
		}
	}

	public virtual Rect GetBoundingBox()
	{
		Vector3 localScale = transform.localScale;
		Vector3 position = transform.position;
		float x = (0f - ScreenAlignment.x) * localScale.x;
		float y = (0f - ScreenAlignment.y) * localScale.y;
		float width = ScreenSize.x * localScale.x;
		float height = ScreenSize.y * localScale.y;
		boundingRect.Set(x, y, width, height);
		boundingRect.center += position.xy();
		return boundingRect;
	}

	protected Rect GetClippedBounds()
	{
		g_clipRects.Clear();
		if (!Visible)
		{
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (ignoreClipping)
		{
			return Rect.MinMaxRect(-1000f, -1000f, 1000f, 1000f);
		}
		if (transform.parent != null)
		{
			UXGUIElement[] someElements = transform.parent.GetComponentsInParent<UXGUIElement>();
			GetClippedBoundsNonRecursive(ref someElements, ref g_clipRects);
		}
		Rect result = Rect.MinMaxRect(-1000f, -1000f, 1000f, 1000f);
		foreach (Rect g_clipRect in g_clipRects)
		{
			if (g_clipRect.xMin > result.xMin)
			{
				result.xMin = g_clipRect.xMin;
			}
			if (g_clipRect.xMax < result.xMax)
			{
				result.xMax = g_clipRect.xMax;
			}
			if (g_clipRect.yMin > result.yMin)
			{
				result.yMin = g_clipRect.yMin;
			}
			if (g_clipRect.yMax < result.yMax)
			{
				result.yMax = g_clipRect.yMax;
			}
		}
		return result;
	}

	private void GetClippedBoundsNonRecursive(ref UXGUIElement[] someElements, ref List<Rect> clipRects)
	{
		int num = someElements.Length;
		for (int i = 0; i < num; i++)
		{
			UXGUIElement uXGUIElement = someElements[i];
			if (uXGUIElement is IUXContainer)
			{
				clipRects.Add(uXGUIElement.GetBoundingBox());
			}
		}
	}

	private void GetClippedBoundsRecursive(Transform current, ref List<Rect> clipRects)
	{
		UXGUIElement component = current.GetComponent<UXGUIElement>();
		if (component != null && component is IUXContainer)
		{
			clipRects.Add(component.GetBoundingBox());
		}
		Transform parent = current.parent;
		if (parent != null)
		{
			GetClippedBoundsRecursive(parent, ref clipRects);
		}
	}
}
