using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TextWrapper
{
	private MeshRenderer meshRenderer;

	private TextMesh textMesh;

	public TextWrapper(Font font, int fontSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		GameObject val = new GameObject();
		((Object)val).hideFlags = (HideFlags)13;
		meshRenderer = val.AddComponent<MeshRenderer>();
		((Renderer)meshRenderer).enabled = false;
		textMesh = val.AddComponent<TextMesh>();
		textMesh.font = font;
		textMesh.fontSize = fontSize;
	}

	public string Wrap(string text, float maxWidth)
	{
		string[] source = text.Split(new char[1] { '\n' });
		string[] value = source.Select((string line) => WrapLine(line, maxWidth)).ToArray();
		return string.Join("\n", value);
	}

	private string WrapLine(string unwrappedLine, float maxWidth)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Queue<string> queue = new Queue<string>(unwrappedLine.Split(new char[1] { ' ' }));
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		while (queue.Count > 0)
		{
			string text = queue.Peek();
			if (list2.Count > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(text);
			textMesh.text = stringBuilder.ToString();
			if (list2.Count != 0)
			{
				Bounds bounds = ((Renderer)meshRenderer).bounds;
				if (!(bounds.size.x < maxWidth))
				{
					list.Add(string.Join(" ", list2.ToArray()));
					list2.Clear();
					stringBuilder = new StringBuilder();
					continue;
				}
			}
			list2.Add(text);
			queue.Dequeue();
		}
		list.Add(string.Join(" ", list2.ToArray()));
		return string.Join("\n", list.ToArray());
	}
}
