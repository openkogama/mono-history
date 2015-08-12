using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCoinStringRenderer : MonoBehaviour
{
	private struct PriceTagString
	{
		public Vector3 start;

		public Vector3 end;

		public Color color;
	}

	private static Material stringMaterial;

	private Queue<PriceTagString> strings = new Queue<PriceTagString>();

	public void Start()
	{
		enabled = true;
		CreateStringMaterial();
	}

	public void AddString(Vector3 start, Vector3 end, Color color)
	{
		PriceTagString item = new PriceTagString
		{
			start = start,
			end = end,
			color = color
		};
		strings.Enqueue(item);
	}

	private void OnPostRender()
	{
		while (strings.Count > 0)
		{
			DrawString(strings.Dequeue());
		}
	}

	private void DrawString(PriceTagString s)
	{
		try
		{
			stringMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(s.color);
			GL.Vertex3(s.start.x, s.start.y, s.start.z);
			GL.Vertex3(s.end.x, s.end.y, s.end.z);
			GL.End();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Draw string exception: " + ex);
		}
	}

	private static void CreateStringMaterial()
	{
		if (!stringMaterial)
		{
			stringMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {     Blend One OneMinusSrcAlpha     ZWrite Off Cull Off Fog { Mode Off }     BindChannels {      Bind \"vertex\", vertex Bind \"color\", color }} } }");
			stringMaterial.hideFlags = HideFlags.HideAndDontSave;
			stringMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}
}
