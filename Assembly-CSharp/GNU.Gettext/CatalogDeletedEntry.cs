using System.Collections.Generic;

namespace GNU.Gettext;

public class CatalogDeletedEntry
{
	private List<string> deletedLines;

	private List<string> references;

	private List<string> autocomments;

	private string flags;

	private string comment;

	public string[] DeletedLines => deletedLines.ToArray();

	public string[] References => references.ToArray();

	public string Comment => comment;

	public string[] AutoComments => autocomments.ToArray();

	public bool HasComment => !string.IsNullOrEmpty(comment);

	public string Flags
	{
		get
		{
			if (string.IsNullOrEmpty(flags))
			{
				return string.Empty;
			}
			if (flags.StartsWith("#,"))
			{
				return flags;
			}
			return "#, " + flags;
		}
		set
		{
			flags = value;
		}
	}

	public CatalogDeletedEntry(string[] deletedLines)
	{
		this.deletedLines = new List<string>(deletedLines);
		references = new List<string>();
		autocomments = new List<string>();
	}

	public CatalogDeletedEntry(CatalogDeletedEntry dt)
	{
		deletedLines = new List<string>(dt.deletedLines);
		references = new List<string>(dt.references);
		autocomments = new List<string>(dt.autocomments);
		flags = dt.flags;
		comment = dt.comment;
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

	public void SetDeletedLines(string[] lines)
	{
		deletedLines = new List<string>(lines);
	}

	public void SetComment(string comment)
	{
		this.comment = comment;
	}

	public void AddAutoComments(string comment)
	{
		autocomments.Add(comment);
	}

	public void ClearAutoComments()
	{
		autocomments.Clear();
	}
}
