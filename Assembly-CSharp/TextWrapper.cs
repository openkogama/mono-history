using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TextWrapper
{
	private MeshRenderer meshRenderer;

	private TextMesh textMesh;

	private TextWrapStyle wrapStyle;

	public TextWrapper(Font font, Material material, int fontSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected Obj, but got Unknown
		GameObject val = new GameObject();
		((Object)val).hideFlags = (HideFlags)13;
		meshRenderer = val.AddComponent<MeshRenderer>();
		((Renderer)meshRenderer).enabled = false;
		((Renderer)meshRenderer).material = new Material(material);
		textMesh = val.AddComponent<TextMesh>();
		textMesh.font = font;
		textMesh.fontSize = fontSize;
	}

	public string Wrap(string text, float maxWidth, TextWrapStyle wrapStyle)
	{
		this.wrapStyle = wrapStyle;
		string[] source = text.Split(new char[1] { '\n' });
		string[] value = source.Select((string line) => WrapLine(line, maxWidth)).ToArray();
		return string.Join("\n", value);
	}

	private string WrapLine(string unwrappedLine, float maxWidth)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		Queue<string> queue = new Queue<string>(unwrappedLine.Split(new char[1] { ' ' }));
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		string empty = string.Empty;
		while (queue.Count > 0)
		{
			string text = queue.Peek();
			if (list2.Count > 0)
			{
				stringBuilder.Append(" ");
			}
			empty = stringBuilder.ToString();
			stringBuilder.Append(text);
			textMesh.text = stringBuilder.ToString();
			Bounds bounds = ((Renderer)meshRenderer).bounds;
			if (bounds.size.x <= maxWidth)
			{
				list2.Add(text);
				queue.Dequeue();
				continue;
			}
			Bounds bounds2 = ((Renderer)meshRenderer).bounds;
			float x = bounds2.size.x;
			textMesh.text = text;
			Bounds bounds3 = ((Renderer)meshRenderer).bounds;
			float x2 = bounds3.size.x;
			if (wrapStyle == TextWrapStyle.Mixed && x > maxWidth && x2 <= maxWidth / 2f)
			{
				list.Add(string.Join(" ", list2.ToArray()));
				list2.Clear();
				stringBuilder = new StringBuilder();
			}
			else
			{
				if (wrapStyle != TextWrapStyle.OnLetter && (wrapStyle != TextWrapStyle.Mixed || !(x > maxWidth) || !(x2 > maxWidth / 2f)))
				{
					continue;
				}
				string text2 = text;
				string text3 = empty;
				do
				{
					int num = 0;
					textMesh.text = text3 + text2.Substring(0, num);
					while (true)
					{
						Bounds bounds4 = ((Renderer)meshRenderer).bounds;
						if (!(bounds4.size.x < maxWidth))
						{
							break;
						}
						num++;
						textMesh.text = text3 + text2.Substring(0, num);
					}
					Bounds bounds5 = ((Renderer)meshRenderer).bounds;
					if (bounds5.size.x > maxWidth)
					{
						num--;
					}
					string item = text2.Substring(0, num);
					string text4 = text2.Substring(num, text2.Length - num);
					list2.Add(item);
					list.Add(string.Join(" ", list2.ToArray()));
					list2.Clear();
					text2 = text4;
					textMesh.text = text2;
					Bounds bounds6 = ((Renderer)meshRenderer).bounds;
					x = (x2 = bounds6.size.x);
					text3 = string.Empty;
				}
				while (x > maxWidth);
				list2.Add(text2);
				queue.Dequeue();
				stringBuilder = new StringBuilder();
				stringBuilder.Append(text2);
			}
		}
		list.Add(string.Join(" ", list2.ToArray()));
		return string.Join("\n", list.ToArray());
	}
}
