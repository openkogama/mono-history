namespace GNU.Gettext;

internal class PluralFormsScanner
{
	private string str;

	private int pos;

	private PluralFormsToken token;

	public PluralFormsToken Token => token;

	public PluralFormsScanner(string str)
	{
		this.str = str;
		token = new PluralFormsToken();
		NextToken();
	}

	public bool NextToken()
	{
		PluralFormsToken.Type type = PluralFormsToken.Type.Error;
		while (pos < str.Length && str[pos] == ' ')
		{
			pos++;
		}
		if (pos >= str.Length || str[pos] == '\0')
		{
			type = PluralFormsToken.Type.Eof;
		}
		else if (char.IsDigit(str[pos]))
		{
			int num = str[pos++] - 48;
			while (pos < str.Length && char.IsDigit(str[pos]))
			{
				num = num * 10 + (str[pos++] - 48);
			}
			token.Number = num;
			type = PluralFormsToken.Type.Number;
		}
		else if (char.IsLetter(str[pos]))
		{
			int num2 = pos++;
			while (pos < str.Length && char.IsLetterOrDigit(str[pos]))
			{
				pos++;
			}
			int num3 = pos - num2;
			if (num3 == 1 && str[num2] == 'n')
			{
				type = PluralFormsToken.Type.N;
			}
			else if (num3 == 6 && str.Substring(num2, num3) == "plural")
			{
				type = PluralFormsToken.Type.Plural;
			}
			else if (num3 == 8 && str.Substring(num2, num3) == "nplurals")
			{
				type = PluralFormsToken.Type.Nplurals;
			}
		}
		else if (str[pos] == '=')
		{
			pos++;
			if (pos < str.Length && str[pos] == '=')
			{
				pos++;
				type = PluralFormsToken.Type.Equal;
			}
			else
			{
				type = PluralFormsToken.Type.Assign;
			}
		}
		else if (str[pos] == '>')
		{
			pos++;
			if (pos < str.Length && str[pos] == '=')
			{
				pos++;
				type = PluralFormsToken.Type.GreaterOrEqual;
			}
			else
			{
				type = PluralFormsToken.Type.Greater;
			}
		}
		else if (str[pos] == '<')
		{
			pos++;
			if (pos < str.Length && str[pos] == '=')
			{
				pos++;
				type = PluralFormsToken.Type.LessOrEqual;
			}
			else
			{
				type = PluralFormsToken.Type.Less;
			}
		}
		else if (str[pos] == '%')
		{
			pos++;
			type = PluralFormsToken.Type.Reminder;
		}
		else if (str[pos] == '!' && str[pos + 1] == '=')
		{
			pos += 2;
			type = PluralFormsToken.Type.NotEqual;
		}
		else if (pos + 1 < str.Length && str[pos] == '&' && str[pos + 1] == '&')
		{
			pos += 2;
			type = PluralFormsToken.Type.LogicalAnd;
		}
		else if (pos + 1 < str.Length && str[pos] == '|' && str[pos + 1] == '|')
		{
			pos += 2;
			type = PluralFormsToken.Type.LogicalOr;
		}
		else if (str[pos] == '?')
		{
			pos++;
			type = PluralFormsToken.Type.Question;
		}
		else if (str[pos] == ':')
		{
			pos++;
			type = PluralFormsToken.Type.Colon;
		}
		else if (str[pos] == ';')
		{
			pos++;
			type = PluralFormsToken.Type.Semicolon;
		}
		else if (str[pos] == '(')
		{
			pos++;
			type = PluralFormsToken.Type.LeftBracket;
		}
		else if (str[pos] == ')')
		{
			pos++;
			type = PluralFormsToken.Type.RightBracket;
		}
		token.TokenType = type;
		return type != PluralFormsToken.Type.Error;
	}
}
