using System;
using System.Collections.Generic;

namespace GNU.Gettext;

public class CatalogEntry
{
	public enum Validity
	{
		Unknown,
		Invalid,
		Valid
	}

	private string str;

	private string plural;

	private bool hasPlural;

	private List<string> translations;

	private List<string> references;

	private List<string> autocomments;

	private bool isFuzzy;

	private bool isModified;

	private bool isAutomatic;

	private bool hasBadTokens;

	private string moreFlags;

	private string comment;

	private Validity validity;

	private string errorString;

	private string context = string.Empty;

	private Catalog owner;

	public string String => str;

	public bool HasPlural => hasPlural;

	public string PluralString => plural;

	public int NumberOfTranslations => translations.Count;

	public int TranslationsCount => translations.Count;

	public string Context
	{
		get
		{
			return context;
		}
		set
		{
			context = value.Trim();
		}
	}

	public bool HasContext => !string.IsNullOrEmpty(context);

	public string Key => MakeKey(String, Context);

	public string[] References => references.ToArray();

	public string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			if (comment != value)
			{
				comment = value;
				MarkOwnerDirty();
			}
		}
	}

	public string[] AutoComments => autocomments.ToArray();

	public bool HasComment => !string.IsNullOrEmpty(comment);

	public string Flags
	{
		get
		{
			string text = string.Empty;
			if (isFuzzy)
			{
				text = ", fuzzy";
			}
			text += moreFlags;
			if (!string.IsNullOrEmpty(text))
			{
				return "#" + text;
			}
			return string.Empty;
		}
		set
		{
			isFuzzy = false;
			moreFlags = string.Empty;
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			string[] array = value.TrimStart('#', ',').Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Trim() == "fuzzy")
				{
					isFuzzy = true;
				}
				else
				{
					moreFlags = moreFlags + ", " + text.Trim();
				}
			}
		}
	}

	public bool IsFuzzy
	{
		get
		{
			return isFuzzy;
		}
		set
		{
			isFuzzy = value;
			MarkOwnerDirty();
		}
	}

	public bool IsTranslated
	{
		get
		{
			bool flag = false;
			flag = translations.Count >= owner.PluralFormsCount || (!HasPlural && !string.IsNullOrEmpty(translations[0]));
			if (flag && HasPlural)
			{
				for (int i = 0; i < owner.PluralFormsCount; i++)
				{
					if (string.IsNullOrEmpty(translations[i]))
					{
						flag = false;
						break;
					}
				}
			}
			return flag;
		}
	}

	public bool IsModified
	{
		get
		{
			return isModified;
		}
		set
		{
			isModified = value;
		}
	}

	public bool IsAutomatic
	{
		get
		{
			return isAutomatic;
		}
		set
		{
			isAutomatic = value;
		}
	}

	public Validity DataValidity
	{
		get
		{
			return validity;
		}
		set
		{
			validity = value;
		}
	}

	public string ErrorString
	{
		get
		{
			return errorString;
		}
		set
		{
			errorString = value;
		}
	}

	public string LocaleCode => owner.LocaleCode;

	public CatalogEntry(Catalog owner, string str, string plural)
	{
		this.owner = owner;
		this.str = str;
		this.plural = plural;
		hasPlural = !string.IsNullOrEmpty(plural);
		references = new List<string>();
		autocomments = new List<string>();
		translations = new List<string>();
		isFuzzy = false;
		isModified = false;
		isAutomatic = false;
		validity = Validity.Unknown;
	}

	public CatalogEntry(Catalog owner, CatalogEntry dt)
	{
		this.owner = owner;
		str = dt.str;
		plural = dt.plural;
		hasPlural = dt.hasPlural;
		translations = new List<string>(dt.translations);
		references = new List<string>(dt.references);
		autocomments = new List<string>(dt.autocomments);
		isFuzzy = dt.isFuzzy;
		isModified = dt.isModified;
		isAutomatic = dt.isAutomatic;
		hasBadTokens = dt.hasBadTokens;
		moreFlags = dt.moreFlags;
		comment = dt.comment;
		validity = dt.validity;
		errorString = dt.errorString;
		context = dt.Context;
	}

	public string GetTranslation(int index)
	{
		if (index < 0 || index >= translations.Count)
		{
			return string.Empty;
		}
		return translations[index];
	}

	public static string MakeKey(string msgid, string context)
	{
		if (string.IsNullOrEmpty(msgid))
		{
			throw new Exception("Msgid cannot be empty");
		}
		return $"{((!string.IsNullOrEmpty(context)) ? (context.Trim() + '|') : string.Empty)}{msgid}";
	}

	public void AddReference(string reference)
	{
		if (!references.Contains(reference))
		{
			references.Add(reference);
		}
	}

	public void ClearReferences()
	{
		references.Clear();
	}

	public bool RemoveReferenceTo(string fileNamePrefix)
	{
		bool result = false;
		for (int i = 0; i < references.Count; i++)
		{
			if (references[i].StartsWith(fileNamePrefix))
			{
				references.RemoveAt(i);
				i--;
				result = true;
			}
		}
		return result;
	}

	public void RemoveReference(string reference)
	{
		if (references.Contains(reference))
		{
			references.Remove(reference);
		}
	}

	public void SetString(string str)
	{
		this.str = str;
		validity = Validity.Unknown;
	}

	public void SetPluralString(string plural)
	{
		this.plural = plural;
		hasPlural = !string.IsNullOrEmpty(plural);
	}

	public void SetTranslation(string translation, int index)
	{
		while (index >= translations.Count)
		{
			translations.Add(string.Empty);
		}
		if (translations[index] != translation)
		{
			translations[index] = translation;
			validity = Validity.Unknown;
			MarkOwnerDirty();
		}
	}

	public void SetTranslations(string[] translations)
	{
		this.translations = new List<string>(translations);
		validity = Validity.Unknown;
		MarkOwnerDirty();
	}

	public bool IsInFormat(string format)
	{
		if (string.IsNullOrEmpty(moreFlags))
		{
			return false;
		}
		string text = $"{format}-format";
		string[] array = moreFlags.Split(',');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text2.Trim() == text)
			{
				return true;
			}
		}
		return false;
	}

	public void AddAutoComment(string comment, bool ifNotExists)
	{
		if (!ifNotExists || !autocomments.Contains(comment))
		{
			autocomments.Add(comment);
		}
	}

	public void AddAutoComment(string comment)
	{
		AddAutoComment(comment, ifNotExists: false);
	}

	public void ClearAutoComments()
	{
		autocomments.Clear();
	}

	private void MarkOwnerDirty()
	{
		if (owner != null)
		{
			owner.IsDirty = true;
		}
	}
}
