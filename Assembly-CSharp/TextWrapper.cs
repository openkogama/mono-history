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
		GameObject gameObject = new GameObject
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.enabled = false;
		meshRenderer.material = new Material(material);
		textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.font = font;
		textMesh.fontSize = fontSize;
	}

	public string Wrap(string text, float maxWidth, TextWrapStyle wrapStyle)
	{
		this.wrapStyle = wrapStyle;
		string[] source = text.Split('\n');
		string[] value = source.Select((string line) => WrapLine(line, maxWidth)).ToArray();
		return string.Join("\n", value);
	}

	private string WrapLine(string unwrappedLine, float maxWidth)
	{
		Queue<string> queue = new Queue<string>(unwrappedLine.Split(' '));
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
			if (meshRenderer.bounds.size.x <= maxWidth)
			{
				list2.Add(text);
				queue.Dequeue();
				continue;
			}
			float x = meshRenderer.bounds.size.x;
			textMesh.text = text;
			float x2 = meshRenderer.bounds.size.x;
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
					while (meshRenderer.bounds.size.x < maxWidth)
					{
						num++;
						textMesh.text = text3 + text2.Substring(0, num);
					}
					if (meshRenderer.bounds.size.x > maxWidth)
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
					x = (x2 = meshRenderer.bounds.size.x);
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
