using System;
using System.Text;

namespace GNU.Gettext;

public static class StringEscaping
{
	public enum EscapeMode
	{
		None,
		CSharp,
		CSharpVerbatim,
		Xml
	}

	public static string ToGettextFormat(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in text)
		{
			switch (c)
			{
			case '"':
				stringBuilder.Append("\\\"");
				continue;
			case '\\':
				stringBuilder.Append("\\\\");
				continue;
			case '\n':
				stringBuilder.Append("\\n");
				continue;
			case '\r':
				stringBuilder.Append("\\r");
				continue;
			case '\t':
				stringBuilder.Append("\\t");
				continue;
			default:
				if (char.IsControl(c))
				{
					throw new FormatException($"Invalid character '{c}' in translatable string: '{text}'");
				}
				break;
			case '_':
				break;
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	public static string FromGettextFormat(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '\\' && i + 1 < text.Length)
			{
				char c2 = text[i + 1];
				switch (c2)
				{
				case '"':
				case '\\':
					stringBuilder.Append(c2);
					i++;
					break;
				case 'n':
					stringBuilder.Append('\n');
					i++;
					break;
				case 't':
					stringBuilder.Append('\t');
					i++;
					break;
				case 'r':
					stringBuilder.Append('\r');
					i++;
					break;
				default:
					throw new FormatException($"Invalid escape sequence '{c2}' in string: '{text}'");
				}
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static string UnEscape(EscapeMode mode, string text)
	{
		return mode switch
		{
			EscapeMode.None => text, 
			EscapeMode.CSharp => FromCSharpFormat(text), 
			EscapeMode.CSharpVerbatim => FromCSharpVerbatimFormat(text), 
			EscapeMode.Xml => FromXml(text), 
			_ => throw new Exception("Unknown string escaping mode '" + mode.ToString() + "'"), 
		};
	}

	private static string FromCSharpVerbatimFormat(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '"')
			{
				i++;
				char c2 = text[i];
				if (c2 != '"')
				{
					throw new FormatException("Unescaped \" character in C# verbatim string.");
				}
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	private static string FromXml(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '&')
			{
				int num = text.IndexOf(';', i);
				if (num == -1)
				{
					throw new FormatException("Unterminated XML entity.");
				}
				string text2 = text.Substring(i + 1, num - i - 1);
				switch (text2)
				{
				case "lt":
					stringBuilder.Append('<');
					break;
				case "gt":
					stringBuilder.Append('>');
					break;
				case "amp":
					stringBuilder.Append('&');
					break;
				case "apos":
					stringBuilder.Append('\'');
					break;
				case "quot":
					stringBuilder.Append('"');
					break;
				default:
					throw new FormatException("Unrecogised XML entity '&" + text2 + ";'.");
				}
				i = num;
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private static string FromCSharpFormat(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c != '\\')
			{
				stringBuilder.Append(c);
				continue;
			}
			i++;
			char c2 = text[i];
			switch (c2)
			{
			case '"':
			case '\'':
			case '\\':
				stringBuilder.Append(c2);
				break;
			case 'a':
				stringBuilder.Append('\a');
				break;
			case 'b':
				stringBuilder.Append('\b');
				break;
			case 'f':
				stringBuilder.Append('\f');
				break;
			case 'n':
				stringBuilder.Append('\n');
				break;
			case 'r':
				stringBuilder.Append('\r');
				break;
			case 't':
				stringBuilder.Append('\t');
				break;
			case 'v':
				stringBuilder.Append('\v');
				break;
			default:
				throw new FormatException("Invalid escape '\\" + c2 + "' in translatable string.");
			}
		}
		return stringBuilder.ToString();
	}
}
