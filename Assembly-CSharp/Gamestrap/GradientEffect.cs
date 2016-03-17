using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamestrap;

[AddComponentMenu("UI/Gamestrap UI/Gradient")]
public class GradientEffect : BaseMeshEffect
{
	public Color top = Color.white;

	public Color bottom = Color.white;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}
	}

	public void ModifyVertices(List<UIVertex> vertexList)
	{
		if (!IsActive() || vertexList.Count < 4)
		{
			return;
		}
		if (vertexList.Count == 6)
		{
			SetVertexColor(vertexList, 0, bottom);
			SetVertexColor(vertexList, 1, top);
			SetVertexColor(vertexList, 2, top);
			SetVertexColor(vertexList, 3, top);
			SetVertexColor(vertexList, 4, bottom);
			SetVertexColor(vertexList, 5, bottom);
			return;
		}
		float y = vertexList[vertexList.Count - 1].position.y;
		float y2 = vertexList[0].position.y;
		float num = y2 - y;
		for (int i = 0; i < vertexList.Count; i++)
		{
			UIVertex value = vertexList[i];
			value.color *= Color.Lerp(top, bottom, (value.position.y - y) / num);
			vertexList[i] = value;
		}
	}

	private void SetVertexColor(List<UIVertex> vertexList, int index, Color color)
	{
		UIVertex value = vertexList[index];
		value.color = color;
		vertexList[index] = value;
	}
}
