using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientEffectHorizontal : BaseMeshEffect
{
	public Color left = Color.white;

	public Color right = Color.white;

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
			SetVertexColor(vertexList, 0, left);
			SetVertexColor(vertexList, 1, left);
			SetVertexColor(vertexList, 2, right);
			SetVertexColor(vertexList, 3, right);
			SetVertexColor(vertexList, 4, right);
			SetVertexColor(vertexList, 5, left);
			return;
		}
		float y = vertexList[vertexList.Count - 1].position.y;
		float y2 = vertexList[0].position.y;
		float num = y2 - y;
		for (int i = 0; i < vertexList.Count; i++)
		{
			UIVertex value = vertexList[i];
			value.color *= Color.Lerp(left, right, (value.position.y - y) / num);
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
