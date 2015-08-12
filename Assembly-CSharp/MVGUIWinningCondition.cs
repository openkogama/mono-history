using UnityEngine;

public class MVGUIWinningCondition : MonoBehaviour
{
	[SerializeField]
	private UXText limit;

	[SerializeField]
	private UXPlane uxPlane;

	public Vector3 Size
	{
		get
		{
			Vector3 result = default;
			UXGUIElement[] componentsInChildren = GetComponentsInChildren<UXGUIElement>();
			UXGUIElement[] array = componentsInChildren;
			foreach (UXGUIElement uXGUIElement in array)
			{
				Vector3 size = uXGUIElement.Size;
				if (size.x > result.x)
				{
					result.x = size.x;
				}
				if (size.y > result.y)
				{
					result.y = size.y;
				}
			}
			return result;
		}
	}

	public void SetLimit(int limit)
	{
		this.limit.Text = limit.ToString();
	}

	public void HideLimit()
	{
		limit.Text = string.Empty;
	}

	public void SetMaterial(Material material)
	{
		uxPlane.GetComponent<Renderer>().material = material;
	}
}
