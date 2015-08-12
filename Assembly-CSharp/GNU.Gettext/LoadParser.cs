using System.Text;

namespace GNU.Gettext;

internal class LoadParser : CatalogParser
{
	private Catalog catalog;

	private bool headerParsed;

	public LoadParser(Catalog catalog, string text, Encoding encoding)
		: base(text, encoding)
	{
		this.catalog = catalog;
	}

	protected override bool OnEntry(string msgid, string msgidPlural, bool hasPlural, string[] translations, string flags, string[] references, string comment, string[] autocomments, string msgctxt)
	{
		if (string.IsNullOrEmpty(msgid) && !headerParsed)
		{
			catalog.ParseHeaderString(translations[0]);
			catalog.Comment = comment;
			headerParsed = true;
		}
		else
		{
			CatalogEntry catalogEntry = new CatalogEntry(catalog, string.Empty, string.Empty);
			if (!string.IsNullOrEmpty(flags))
			{
				catalogEntry.Flags = flags;
			}
			catalogEntry.SetString(msgid);
			if (hasPlural)
			{
				catalogEntry.SetPluralString(msgidPlural);
			}
			catalogEntry.SetTranslations(translations);
			catalogEntry.Comment = comment;
			for (uint num = 0u; num < references.Length; num++)
			{
				catalogEntry.AddReference(references[num]);
			}
			for (uint num2 = 0u; num2 < autocomments.Length; num2++)
			{
				catalogEntry.AddAutoComment(autocomments[num2]);
			}
			catalogEntry.Context = msgctxt;
			catalog.AddItem(catalogEntry);
		}
		return true;
	}

	protected override bool OnDeletedEntry(string[] deletedLines, string flags, string[] references, string comment, string[] autocomments)
	{
		CatalogDeletedEntry catalogDeletedEntry = new CatalogDeletedEntry(new string[0]);
		if (!string.IsNullOrEmpty(flags))
		{
			catalogDeletedEntry.Flags = flags;
		}
		catalogDeletedEntry.SetDeletedLines(deletedLines);
		catalogDeletedEntry.SetComment(comment);
		for (uint num = 0u; num < autocomments.Length; num++)
		{
			catalogDeletedEntry.AddAutoComments(autocomments[num]);
		}
		catalog.AddDeletedItem(catalogDeletedEntry);
		return true;
	}
}
