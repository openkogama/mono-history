using System.Text;

namespace GNU.Gettext;

internal class CharsetInfoFinder : CatalogParser
{
	private string charset;

	public string Charset => charset;

	public CharsetInfoFinder(string text)
		: base(text, Encoding.GetEncoding("iso-8859-1"))
	{
		charset = "iso-8859-1";
	}

	protected override bool OnEntry(string msgid, string msgidPlural, bool hasPlural, string[] translations, string flags, string[] references, string comment, string[] autocomments, string msgctxt)
	{
		if (string.IsNullOrEmpty(msgid))
		{
			Catalog catalog = new Catalog();
			catalog.ParseHeaderString(translations[0]);
			charset = catalog.Charset;
			if (charset == "CHARSET")
			{
				charset = "iso-8859-1";
			}
			return false;
		}
		return true;
	}
}
