namespace GNU.Gettext;

internal class PluralFormsToken
{
	public enum Type
	{
		Error,
		Eof,
		Number,
		N,
		Plural,
		Nplurals,
		Equal,
		Assign,
		Greater,
		GreaterOrEqual,
		Less,
		LessOrEqual,
		Reminder,
		NotEqual,
		LogicalAnd,
		LogicalOr,
		Question,
		Colon,
		Semicolon,
		LeftBracket,
		RightBracket
	}

	private Type type;

	private int number;

	public Type TokenType
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public int Number
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
		}
	}

	public PluralFormsToken()
	{
	}

	public PluralFormsToken(PluralFormsToken src)
	{
		type = src.type;
		number = src.number;
	}

	public override string ToString()
	{
		return $"[Token: Type={TokenType}, Number={Number}]";
	}
}
