using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GNU.Gettext;

public class Catalog : IEnumerable<CatalogEntry>, IEnumerable
{
	private IDictionary<string, CatalogEntry> entriesDict;

	private List<CatalogEntry> entriesList;

	private List<CatalogDeletedEntry> deletedEntriesList;

	private bool isOk;

	private bool isDirty;

	private string fileName;

	public const string PluralFormsHeader = "Plural-Forms";

	private Dictionary<string, string> headerEntries = new Dictionary<string, string>();

	public string Project = string.Empty;

	public string CreationDate = string.Empty;

	public string RevisionDate = string.Empty;

	public string Translator = string.Empty;

	public string TranslatorEmail = string.Empty;

	public string Team = string.Empty;

	public string TeamEmail = string.Empty;

	public string Charset = string.Empty;

	public string Language = string.Empty;

	public string Country = string.Empty;

	public string Comment = string.Empty;

	public bool IsDirty
	{
		get
		{
			return isDirty;
		}
		set
		{
			isDirty = value;
			OnDirtyChanged(EventArgs.Empty);
		}
	}

	public int Count => entriesList.Count;

	public CatalogEntry this[int index] => (index < 0 || index >= entriesList.Count) ? null : entriesList[index];

	public int PluralFormsCount
	{
		get
		{
			int result = 2;
			PluralFormsCalculator pluralFormsCalculator = PluralFormsCalculator.Make(GetPluralFormsHeader());
			if (pluralFormsCalculator != null)
			{
				result = pluralFormsCalculator.NPlurals;
			}
			return result;
		}
	}

	public string[] PluralFormsDescriptions
	{
		get
		{
			List<string> list = new List<string>();
			if (!HasHeader("Plural-Forms"))
			{
				list.Add("Singular");
				list.Add("Plural");
				return list.ToArray();
			}
			PluralFormsCalculator pluralFormsCalculator = PluralFormsCalculator.Make(GetHeader("Plural-Forms"));
			int pluralFormsCount = PluralFormsCount;
			for (int i = 0; i < pluralFormsCount; i++)
			{
				int num = 0;
				if (pluralFormsCalculator != null)
				{
					for (num = 1; num < 1000 && pluralFormsCalculator.Evaluate(num) != i; num++)
					{
					}
					if (num == 1000 && pluralFormsCalculator.Evaluate(0L) == i)
					{
						num = 0;
					}
				}
				else
				{
					num = 1000;
				}
				string item = ((num != 1000) ? $"Form {i + 1} (e.g. \"{num}\")" : $"Form {i + 1}");
				list.Add(item);
			}
			return list.ToArray();
		}
	}

	public bool IsOk => isOk;

	public string LocaleCode
	{
		get
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(Language))
			{
				text = IsoCodes.LookupLanguageCode(Language).Name;
				if (!string.IsNullOrEmpty(Country))
				{
					text += '_';
					text += IsoCodes.LookupCountryCode(Country);
				}
			}
			if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(fileName))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
				if (fileNameWithoutExtension.Length == 2)
				{
					if (IsoCodes.IsKnownLanguageCode(fileNameWithoutExtension))
					{
						text = fileNameWithoutExtension;
					}
				}
				else if (fileNameWithoutExtension.Length == 5 && fileNameWithoutExtension[2] == '_' && IsoCodes.IsKnownLanguageCode(fileNameWithoutExtension.Substring(0, 2)) && IsoCodes.IsKnownCountryCode(fileNameWithoutExtension.Substring(3, 2)))
				{
					text = fileNameWithoutExtension;
				}
			}
			return text;
		}
	}

	public bool HasDeletedItems => deletedEntriesList.Count > 0;

	public string CommentForGui
	{
		get
		{
			if (string.IsNullOrEmpty(Comment))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			string[] array = Comment.Split('\n');
			foreach (string text in array)
			{
				if (!flag)
				{
					stringBuilder.Append('\n');
				}
				else
				{
					flag = false;
				}
				if (text.StartsWith("#"))
				{
					stringBuilder.Append(text.Substring(1).TrimStart(' ', '\t'));
				}
				else
				{
					stringBuilder.Append(text.TrimStart(' ', '\t'));
				}
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				Comment = string.Empty;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = value.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
			foreach (string text in array)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("# " + text);
			}
			Comment = stringBuilder.ToString();
		}
	}

	public event EventHandler DirtyChanged;

	public Catalog()
	{
		entriesDict = new Dictionary<string, CatalogEntry>();
		entriesList = new List<CatalogEntry>();
		deletedEntriesList = new List<CatalogDeletedEntry>();
		isOk = true;
		CreateNewHeaders();
	}

	public string GetPluralFormsHeader()
	{
		if (HasHeader("Plural-Forms"))
		{
			return GetHeader("Plural-Forms");
		}
		return "nplurals=2; plural=(n != 1);\\n";
	}

	private static string GetDateTimeRfc822Format()
	{
		return DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'sszz00");
	}

	private static void FormatMessageForFile(StringBuilder sb, string prefix, string message, string newlineChar)
	{
		string text = StringEscaping.ToGettextFormat(message);
		if (prefix.Length + text.Length < 77 && !text.Contains("\\n"))
		{
			sb.Append(prefix);
			sb.Append(" \"");
			sb.Append(text);
			sb.Append("\"");
			sb.Append(newlineChar);
			return;
		}
		sb.Append(prefix);
		sb.Append(" \"\"");
		sb.Append(newlineChar);
		int num = -1;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		int num4 = 0;
		while (num4 < text.Length)
		{
			char c = text[num4];
			if (c == '\\' && num4 + 1 < text.Length)
			{
				num4++;
				num2++;
				switch (text[num4])
				{
				case 'n':
					num = num4 + 1;
					flag = true;
					break;
				case 't':
					num = num4 + 1;
					break;
				}
			}
			if (c == ' ')
			{
				num = num4 + 1;
			}
			if (flag || (num2 >= 77 && num != -1))
			{
				sb.Append("\"");
				sb.Append(text.Substring(num3, num - num3));
				sb.Append("\"");
				sb.Append(newlineChar);
				num2 = 0;
				num3 = num;
				num = -1;
				flag = false;
			}
			num4++;
			num2++;
		}
		string text2 = text.Substring(num3);
		if (text2.Length > 0)
		{
			sb.Append("\"");
			sb.Append(text2);
			sb.Append("\"");
			sb.Append(newlineChar);
		}
	}

	private void Clear()
	{
		entriesDict.Clear();
		entriesList.Clear();
		deletedEntriesList.Clear();
		isOk = true;
	}

	public void Load(string text, string fileName)
	{
		Clear();
		isOk = false;
		this.fileName = fileName;
		CharsetInfoFinder charsetInfoFinder = new CharsetInfoFinder(text);
		Charset = charsetInfoFinder.Charset;
		try
		{
			charsetInfoFinder.Parse(text);
			Charset = charsetInfoFinder.Charset;
			Charset = charsetInfoFinder.Charset;
		}
		catch (Exception)
		{
			Debug.Log($"Cannot detect charset of file '{fileName}'. Using default charset '{charsetInfoFinder.Charset}'");
		}
		LoadParser loadParser = new LoadParser(this, text, GetEncoding(Charset));
		if (!loadParser.Parse(text))
		{
			throw new Exception($"Error during parsing '{fileName}' file, file is probably corrupted.");
		}
		isOk = true;
		IsDirty = false;
	}

	private static string EnsureCorrectEndings(string reference, string text)
	{
		if (text.Length == 0)
		{
			return string.Empty;
		}
		int num = 0;
		int num2 = text.Length - 1;
		while (num2 >= 0 && text[num2] == '\n')
		{
			num2--;
			num++;
		}
		StringBuilder stringBuilder = new StringBuilder(text, 0, text.Length - num, text.Length + reference.Length - num);
		int num3 = reference.Length - 1;
		while (num3 >= 0 && reference[num3] == '\n')
		{
			stringBuilder.Append('\n');
			num3--;
		}
		return stringBuilder.ToString();
	}

	private static void SaveMultiLines(StringBuilder sb, string text, string newLine)
	{
		if (text != null)
		{
			string[] array = text.Split(new string[5] { "\n\r", "\r\n", "\r", "\n", "\r" }, StringSplitOptions.None);
			foreach (string arg in array)
			{
				sb.AppendFormat("{0}{1}", arg, newLine);
			}
		}
	}

	private static bool CanEncodeToCharset(string charset)
	{
		EncodingInfo[] encodings = Encoding.GetEncodings();
		foreach (EncodingInfo encodingInfo in encodings)
		{
			try
			{
				if (encodingInfo.Name.ToLower() == charset.ToLower())
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
		}
		return false;
	}

	private static Encoding GetEncoding(string charset)
	{
		EncodingInfo[] encodings = Encoding.GetEncodings();
		foreach (EncodingInfo encodingInfo in encodings)
		{
			try
			{
				if (encodingInfo.Name.ToLower() == charset.ToLower())
				{
					return encodingInfo.GetEncoding();
				}
			}
			catch (Exception)
			{
			}
		}
		return null;
	}

	public bool Translate(string msgid, string context, string translation)
	{
		CatalogEntry catalogEntry = FindItem(msgid, context);
		if (catalogEntry == null)
		{
			return false;
		}
		catalogEntry.SetTranslation(translation, 0);
		return true;
	}

	public CatalogEntry FindItem(string msgid, string context)
	{
		if (entriesDict.ContainsKey(CatalogEntry.MakeKey(msgid, context)))
		{
			return entriesDict[CatalogEntry.MakeKey(msgid, context)];
		}
		return null;
	}

	public bool Contains(string msgid, string context)
	{
		return entriesDict.ContainsKey(CatalogEntry.MakeKey(msgid, context));
	}

	public CatalogEntry FindItem(CatalogEntry entry)
	{
		return (!entriesDict.ContainsKey(entry.Key)) ? null : entriesDict[entry.Key];
	}

	public CatalogEntry AddItem(string original, string plural)
	{
		if (!entriesDict.TryGetValue(original, out var value))
		{
			value = new CatalogEntry(this, original, plural);
			if (!string.IsNullOrEmpty(plural))
			{
				value.SetTranslations(new string[2]
				{
					string.Empty,
					string.Empty
				});
			}
			AddItem(value);
		}
		return value;
	}

	public void GetStatistics(out int all, out int fuzzy, out int missing, out int badtokens, out int untranslated)
	{
		all = (fuzzy = (missing = (badtokens = (untranslated = 0))));
		for (int i = 0; i < Count; i++)
		{
			all++;
			if (this[i].IsFuzzy)
			{
				fuzzy++;
			}
			if (this[i].References.Length == 0)
			{
				missing++;
			}
			if (this[i].DataValidity == CatalogEntry.Validity.Invalid)
			{
				badtokens++;
			}
			if (!this[i].IsTranslated)
			{
				untranslated++;
			}
		}
	}

	public void Append(Catalog catalog)
	{
		for (int i = 0; i < catalog.Count; i++)
		{
			CatalogEntry catalogEntry = catalog[i];
			CatalogEntry catalogEntry2 = FindItem(catalogEntry);
			if (catalogEntry2 == null)
			{
				catalogEntry2 = new CatalogEntry(this, catalogEntry);
				entriesDict.Add(catalogEntry.Key, catalogEntry2);
				entriesList.Add(catalogEntry2);
				continue;
			}
			for (uint num = 0u; num < catalogEntry.References.Length; num++)
			{
				catalogEntry2.AddReference(catalogEntry.References[num]);
			}
			if (!string.IsNullOrEmpty(catalogEntry.GetTranslation(0)))
			{
				catalogEntry2.SetTranslation(catalogEntry.GetTranslation(0), 0);
			}
			if (catalogEntry.IsFuzzy)
			{
				catalogEntry2.IsFuzzy = true;
			}
			if (!string.IsNullOrEmpty(catalogEntry.Flags))
			{
				catalogEntry2.Flags = catalogEntry.Flags;
			}
		}
		IsDirty = true;
	}

	public void AddItem(CatalogEntry data)
	{
		if (FindItem(data) == null)
		{
			entriesDict.Add(data.Key, data);
			entriesList.Add(data);
		}
	}

	public void RemoveItem(CatalogEntry data)
	{
		if (FindItem(data) != null)
		{
			entriesDict.Remove(data.Key);
		}
		if (entriesList.Contains(data))
		{
			entriesList.Remove(data);
		}
	}

	public void AddDeletedItem(CatalogDeletedEntry data)
	{
		deletedEntriesList.Add(data);
	}

	public void RemoveDeletedItems()
	{
		deletedEntriesList.Clear();
	}

	public void GetMergeSummary(Catalog refCat, out string[] newEntries, out string[] obsoleteEntries)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < Count; i++)
		{
			if (refCat.FindItem(this[i]) == null)
			{
				list2.Add(this[i].String);
			}
		}
		for (int i = 0; i < refCat.Count; i++)
		{
			if (FindItem(refCat[i]) == null)
			{
				list.Add(refCat[i].String);
			}
		}
		newEntries = list.ToArray();
		obsoleteEntries = list2.ToArray();
	}

	protected virtual void OnDirtyChanged(EventArgs e)
	{
		if (DirtyChanged != null)
		{
			DirtyChanged(this, e);
		}
	}

	private void CreateNewHeaders()
	{
		RevisionDate = (CreationDate = GetDateTimeRfc822Format());
		Language = (Country = (Project = (Team = (TeamEmail = string.Empty))));
		Charset = "utf-8";
		UpdateHeaderDict();
	}

	public void ParseHeaderString(string headers)
	{
		string text = StringEscaping.FromGettextFormat(headers);
		string[] array = text.Split('\n');
		headerEntries.Clear();
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text2 != string.Empty)
			{
				int num = text2.IndexOf(':');
				if (num == -1)
				{
					throw new Exception($"Malformed header: '{text2}'");
				}
				string key = text2.Substring(0, num).Trim();
				string value = text2.Substring(num + 1).Trim();
				headerEntries[key] = value;
			}
		}
		ParseHeaderDict();
	}

	public string GetHeaderString(string lineDelimeter)
	{
		UpdateHeaderDict();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string key in headerEntries.Keys)
		{
			string arg = string.Empty;
			if (headerEntries[key] != null)
			{
				arg = StringEscaping.ToGettextFormat(headerEntries[key]);
			}
			stringBuilder.AppendFormat("\"{0}: {1}\\n\"{2}", key, arg, lineDelimeter);
		}
		return stringBuilder.ToString();
	}

	public string GetHeaderString()
	{
		return GetHeaderString(Environment.NewLine);
	}

	public void UpdateHeaderDict()
	{
		SetHeader("Project-Id-Version", Project);
		SetHeader("POT-Creation-Date", CreationDate);
		SetHeader("PO-Revision-Date", RevisionDate);
		if (string.IsNullOrEmpty(TranslatorEmail))
		{
			SetHeader("Last-Translator", Translator);
		}
		else
		{
			SetHeader("Last-Translator", $"{Translator} <{TranslatorEmail}>");
		}
		if (string.IsNullOrEmpty(TeamEmail))
		{
			SetHeader("Language-Team", Team);
		}
		else
		{
			SetHeader("Language-Team", $"{Team} <{TeamEmail}>");
		}
		SetHeader("MIME-Version", "1.0");
		SetHeader("Content-Type", "text/plain; charset=" + Charset);
		SetHeader("Content-Transfer-Encoding", "8bit");
		SetHeader("X-Generator", "MonoDevelop Gettext addin");
	}

	private void ParseHeaderDict()
	{
		Project = GetHeader("Project-Id-Version");
		CreationDate = GetHeader("POT-Creation-Date");
		RevisionDate = GetHeader("PO-Revision-Date");
		string header = GetHeader("Last-Translator");
		if (!string.IsNullOrEmpty(header))
		{
			string[] array = header.Split('<', '>');
			if (array.Length < 2)
			{
				Translator = header;
				TranslatorEmail = string.Empty;
			}
			else
			{
				Translator = array[0].Trim();
				TranslatorEmail = array[1].Trim();
			}
		}
		header = GetHeader("Language-Team");
		if (!string.IsNullOrEmpty(header))
		{
			string[] array2 = header.Split('<', '>');
			if (array2.Length < 2)
			{
				Team = header;
				TeamEmail = string.Empty;
			}
			else
			{
				Team = array2[0].Trim();
				TeamEmail = array2[1].Trim();
			}
		}
		string header2 = GetHeader("Content-Type");
		int num = header2.IndexOf("; charset=");
		if (num != -1)
		{
			Charset = header2.Substring(num + "; charset=".Length).Trim();
		}
		else
		{
			Charset = "iso-8859-1";
		}
	}

	public string GetHeader(string key)
	{
		if (headerEntries.ContainsKey(key))
		{
			return headerEntries[key];
		}
		return string.Empty;
	}

	public bool HasHeader(string key)
	{
		return headerEntries.ContainsKey(key);
	}

	public void SetHeader(string key, string value)
	{
		headerEntries[key] = value;
	}

	public void SetHeaderNotEmpty(string key, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			DeleteHeader(key);
		}
		else
		{
			SetHeader(key, value);
		}
	}

	public void DeleteHeader(string key)
	{
		if (HasHeader(key))
		{
			headerEntries.Remove(key);
		}
	}

	public IEnumerator<CatalogEntry> GetEnumerator()
	{
		return entriesList.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return entriesList.GetEnumerator();
	}
}
