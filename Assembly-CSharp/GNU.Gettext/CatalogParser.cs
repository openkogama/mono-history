using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GNU.Gettext;

public abstract class CatalogParser
{
	internal static readonly string[] LineSplitStrings = new string[3] { "\r\n", "\r", "\n" };

	private string newLine;

	public string NewLine => newLine;

	public CatalogParser(string text, Encoding encoding)
	{
		newLine = GetNewLine(text, encoding);
	}

	private static string GetNewLine(string text, Encoding encoding)
	{
		char c = 'x';
		char[] array = new char[1] { 'x' };
		using (TextReader textReader = new StringReader(text))
		{
			while (textReader.Read(array, 0, 1) != 0)
			{
				if (array[0] == '\n')
				{
					if (c == '\r')
					{
						return "\r\n";
					}
					return "\n";
				}
				if (array[0] == '\r')
				{
					switch (c)
					{
					case 'x':
						c = '\r';
						break;
					case '\r':
						return "\r";
					}
				}
				else if (c != 'x')
				{
					return c.ToString();
				}
			}
		}
		if (c != 'x')
		{
			return c.ToString();
		}
		return Environment.NewLine;
	}

	private static bool ReadParam(string input, string pattern, out string output)
	{
		output = string.Empty;
		if (input == null)
		{
			return false;
		}
		input = input.TrimStart(' ', '\t');
		if (input.Length < pattern.Length)
		{
			return false;
		}
		if (!input.StartsWith(pattern))
		{
			return false;
		}
		if (pattern.Trim().Equals("#:"))
		{
			input = input.Replace('\\', '/');
		}
		output = StringEscaping.FromGettextFormat(input.Substring(pattern.Length).TrimEnd(' ', '\t'));
		return true;
	}

	private string ParseMessage(ref string line, ref string dummy, StringReader sr)
	{
		StringBuilder stringBuilder = new StringBuilder(dummy.Substring(0, dummy.Length - 1));
		while (!string.IsNullOrEmpty(line = sr.ReadLine()))
		{
			if (line[0] == '\t')
			{
				line = line.Substring(1);
			}
			if (line[0] == '"' && line[line.Length - 1] == '"')
			{
				stringBuilder.Append(StringEscaping.FromGettextFormat(line.Substring(1, line.Length - 2)));
				continue;
			}
			break;
		}
		return stringBuilder.ToString();
	}

	public bool Parse(string text)
	{
		string flags = string.Empty;
		string msgid = string.Empty;
		string msgctxt = string.Empty;
		string msgidPlural = string.Empty;
		string text2 = string.Empty;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		bool flag = false;
		using (StringReader stringReader = new StringReader(text))
		{
			string line = stringReader.ReadLine();
			while (line == string.Empty)
			{
				line = stringReader.ReadLine();
			}
			if (line == null)
			{
				return false;
			}
			while (line != null)
			{
				while (line == "#," || line == "#:")
				{
					line = stringReader.ReadLine();
				}
				if (ReadParam(line, "#, ", out var output))
				{
					flags = output;
					line = stringReader.ReadLine();
				}
				if (ReadParam(line, "#. ", out output) || ReadParam(line, "#.", out output))
				{
					list2.Add(output);
					line = stringReader.ReadLine();
				}
				else if (ReadParam(line, "#: ", out output))
				{
					output = output.Trim();
					while (output != string.Empty)
					{
						int i;
						for (i = 0; i < output.Length && output[i] != ':'; i++)
						{
						}
						for (; i < output.Length && !char.IsWhiteSpace(output[i]); i++)
						{
						}
						string text3 = output.Substring(0, i);
						if (Path.DirectorySeparatorChar == '\\')
						{
							text3 = text3.Replace('/', Path.DirectorySeparatorChar);
						}
						list.Add(text3);
						output = output.Substring(i).Trim();
					}
					line = stringReader.ReadLine();
				}
				else if (ReadParam(line, "msgctxt \"", out output) || ReadParam(line, "msgctxt\t\"", out output))
				{
					msgctxt = ParseMessage(ref line, ref output, stringReader);
				}
				else if (ReadParam(line, "msgid \"", out output) || ReadParam(line, "msgid\t\"", out output))
				{
					msgid = ParseMessage(ref line, ref output, stringReader);
				}
				else if (ReadParam(line, "msgid_plural \"", out output) || ReadParam(line, "msgid_plural\t\"", out output))
				{
					msgidPlural = ParseMessage(ref line, ref output, stringReader);
					flag = true;
				}
				else if (ReadParam(line, "msgstr \"", out output) || ReadParam(line, "msgstr\t\"", out output))
				{
					if (flag)
					{
						Console.WriteLine("Broken catalog file: singular form msgstr used together with msgid_plural");
						return false;
					}
					string item = ParseMessage(ref line, ref output, stringReader);
					list3.Add(item);
					if (!OnEntry(msgid, string.Empty, hasPlural: false, list3.ToArray(), flags, list.ToArray(), text2, list2.ToArray(), msgctxt))
					{
						return false;
					}
					text2 = (msgid = (msgidPlural = (flags = (msgctxt = string.Empty))));
					flag = false;
					list.Clear();
					list2.Clear();
					list3.Clear();
				}
				else if (ReadParam(line, "msgstr[", out output))
				{
					if (!flag)
					{
						Console.WriteLine("Broken catalog file: plural form msgstr used without msgid_plural");
						return false;
					}
					int num = output.IndexOf(']');
					string text4 = output.Substring(num - 1, 1);
					string text5 = "msgstr[" + text4 + "]";
					while (ReadParam(line, text5 + " \"", out output) || ReadParam(line, text5 + "\t\"", out output))
					{
						StringBuilder stringBuilder = new StringBuilder(output.Substring(0, output.Length - 1));
						while (!string.IsNullOrEmpty(line = stringReader.ReadLine()))
						{
							if (line[0] == '\t')
							{
								line = line.Substring(1);
							}
							if (line[0] == '"' && line[line.Length - 1] == '"')
							{
								stringBuilder.Append(line.Substring(1, line.Length - 2));
								continue;
							}
							if (ReadParam(line, "msgstr[", out output))
							{
								num = output.IndexOf(']');
								text4 = output.Substring(num - 1, 1);
								text5 = "msgstr[" + text4 + "]";
							}
							break;
						}
						list3.Add(StringEscaping.FromGettextFormat(stringBuilder.ToString()));
					}
					if (!OnEntry(msgid, msgidPlural, hasPlural: true, list3.ToArray(), flags, list.ToArray(), text2, list2.ToArray(), msgctxt))
					{
						return false;
					}
					text2 = (msgid = (msgidPlural = (flags = string.Empty)));
					flag = false;
					list.Clear();
					list2.Clear();
					list3.Clear();
				}
				else if (ReadParam(line, "#~ ", out output))
				{
					List<string> list4 = new List<string>();
					list4.Add(line);
					while (!string.IsNullOrEmpty(line = stringReader.ReadLine()) && ReadParam(line, "#~ ", out output))
					{
						list4.Add(line);
					}
					if (!OnDeletedEntry(list4.ToArray(), flags, null, text2, list2.ToArray()))
					{
						return false;
					}
					text2 = (msgid = (msgidPlural = (flags = (msgctxt = string.Empty))));
					flag = false;
					list.Clear();
					list2.Clear();
					list3.Clear();
				}
				else if (line != null && line[0] == '#')
				{
					while (!string.IsNullOrEmpty(line) && ((line[0] == '#' && line.Length < 2) || (line[0] == '#' && line[1] != ',' && line[1] != ':' && line[1] != '.' && line[1] != '~')))
					{
						text2 += ((text2.Length <= 0) ? line : ('\n' + line));
						line = stringReader.ReadLine();
					}
				}
				else
				{
					line = stringReader.ReadLine();
				}
				while (line == string.Empty)
				{
					line = stringReader.ReadLine();
				}
			}
		}
		return true;
	}

	protected abstract bool OnEntry(string msgid, string msgidPlural, bool hasPlural, string[] translations, string flags, string[] references, string comment, string[] autocomments, string msgctxt);

	protected virtual bool OnDeletedEntry(string[] deletedLines, string flags, string[] references, string comment, string[] autocomments)
	{
		return true;
	}
}
