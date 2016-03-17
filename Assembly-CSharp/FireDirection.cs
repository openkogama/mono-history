using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FireDirection : MonoBehaviour, IDragHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private Image dirMarker;

	private static List<Vector3> fixedDirs = new List<Vector3>
	{
		Vector3.up,
		Vector3.down,
		Vector3.left,
		Vector3.right,
		(Vector3.up + Vector3.left).normalized,
		(Vector3.up + Vector3.right).normalized,
		(Vector3.down + Vector3.left).normalized,
		(Vector3.down + Vector3.right).normalized
	};

	public void OnDrag(PointerEventData eventData)
	{
		Vector3 vector = eventData.position;
		SetToDir(vector - rectTransform.position);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SetToFixedDir((Vector3)eventData.position - rectTransform.position);
	}

	public void SetToDir(Vector3 dir)
	{
		dir.Normalize();
		SetMarker(dir);
		Vector3 fixedDir = GetFixedDir(dir);
		UpdateLineOfFire(fixedDir);
	}

	public void SetToFixedDir(Vector3 dir)
	{
		dir = GetFixedDir(dir);
		SetMarker(dir);
		UpdateLineOfFire(dir);
	}

	private void SetMarker(Vector3 dir)
	{
		float num = rectTransform.rect.width / 2f - 20f;
		dirMarker.rectTransform.localPosition = dir * num;
	}

	private static void UpdateLineOfFire(Vector3 dir)
	{
		MVGameControllerBase.IPlayModeUI.GetCrossHair().Direction = dir;
	}

	private static Vector3 GetFixedDir(Vector3 dir)
	{
		float num = -1f;
		int index = -1;
		for (int i = 0; i < fixedDirs.Count; i++)
		{
			float num2 = Vector3.Dot(fixedDirs[i], dir);
			if (num2 >= num)
			{
				num = num2;
				index = i;
			}
		}
		return fixedDirs[index];
	}
}
