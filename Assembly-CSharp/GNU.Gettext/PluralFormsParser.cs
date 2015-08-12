namespace GNU.Gettext;

internal class PluralFormsParser
{
	private PluralFormsScanner scanner;

	private PluralFormsToken Token => scanner.Token;

	public PluralFormsParser(PluralFormsScanner scanner)
	{
		this.scanner = scanner;
	}

	public bool Parse(PluralFormsCalculator calculator)
	{
		if (Token.TokenType != PluralFormsToken.Type.Nplurals)
		{
			return false;
		}
		if (!NextToken())
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Assign)
		{
			return false;
		}
		if (!NextToken())
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Number)
		{
			return false;
		}
		int number = Token.Number;
		if (!NextToken())
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Semicolon)
		{
			return false;
		}
		if (!NextToken())
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Plural)
		{
			return false;
		}
		if (!NextToken())
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Assign)
		{
			return false;
		}
		if (!NextToken())
		{
			return false;
		}
		PluralFormsNode pluralFormsNode = ParsePlural();
		if (pluralFormsNode == null)
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Semicolon)
		{
			return false;
		}
		if (!NextToken())
		{
			return false;
		}
		if (Token.TokenType != PluralFormsToken.Type.Eof)
		{
			return false;
		}
		calculator.Init(number, pluralFormsNode);
		return true;
	}

	private PluralFormsNode ParsePlural()
	{
		PluralFormsNode pluralFormsNode = Expression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		if (Token.TokenType != PluralFormsToken.Type.Semicolon)
		{
			return null;
		}
		return pluralFormsNode;
	}

	private bool NextToken()
	{
		if (!scanner.NextToken())
		{
			return false;
		}
		return true;
	}

	private PluralFormsNode Expression()
	{
		PluralFormsNode pluralFormsNode = LogicalOrExpression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		PluralFormsNode pluralFormsNode2 = pluralFormsNode;
		if (Token.TokenType == PluralFormsToken.Type.Question)
		{
			PluralFormsNode pluralFormsNode3 = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = Expression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			pluralFormsNode3.SetNode(1, pluralFormsNode);
			if (Token.TokenType != PluralFormsToken.Type.Colon)
			{
				return null;
			}
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = Expression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			pluralFormsNode3.SetNode(2, pluralFormsNode);
			pluralFormsNode3.SetNode(0, pluralFormsNode2);
			return pluralFormsNode3;
		}
		return pluralFormsNode2;
	}

	private PluralFormsNode LogicalOrExpression()
	{
		PluralFormsNode pluralFormsNode = LogicalAndExpression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		PluralFormsNode pluralFormsNode2 = pluralFormsNode;
		if (Token.TokenType == PluralFormsToken.Type.LogicalOr)
		{
			PluralFormsNode pluralFormsNode3 = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = LogicalOrExpression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			PluralFormsNode pluralFormsNode4 = pluralFormsNode;
			if (pluralFormsNode4.Token.TokenType == PluralFormsToken.Type.LogicalOr)
			{
				pluralFormsNode3.SetNode(0, pluralFormsNode2);
				pluralFormsNode3.SetNode(1, pluralFormsNode4.ReleaseNode(0));
				pluralFormsNode4.SetNode(0, pluralFormsNode3);
				return pluralFormsNode4;
			}
			pluralFormsNode3.SetNode(0, pluralFormsNode2);
			pluralFormsNode3.SetNode(1, pluralFormsNode4);
			return pluralFormsNode3;
		}
		return pluralFormsNode2;
	}

	private PluralFormsNode LogicalAndExpression()
	{
		PluralFormsNode pluralFormsNode = EqualityExpression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		PluralFormsNode pluralFormsNode2 = pluralFormsNode;
		if (Token.TokenType == PluralFormsToken.Type.LogicalAnd)
		{
			PluralFormsNode pluralFormsNode3 = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = LogicalAndExpression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			PluralFormsNode pluralFormsNode4 = pluralFormsNode;
			if (pluralFormsNode4.Token.TokenType == PluralFormsToken.Type.LogicalAnd)
			{
				pluralFormsNode3.SetNode(0, pluralFormsNode2);
				pluralFormsNode3.SetNode(1, pluralFormsNode4.ReleaseNode(0));
				pluralFormsNode4.SetNode(0, pluralFormsNode3);
				return pluralFormsNode4;
			}
			pluralFormsNode3.SetNode(0, pluralFormsNode2);
			pluralFormsNode3.SetNode(1, pluralFormsNode4);
			return pluralFormsNode3;
		}
		return pluralFormsNode2;
	}

	private PluralFormsNode EqualityExpression()
	{
		PluralFormsNode pluralFormsNode = RelationalExpression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		PluralFormsNode pluralFormsNode2 = pluralFormsNode;
		if (Token.TokenType == PluralFormsToken.Type.Equal || Token.TokenType == PluralFormsToken.Type.NotEqual)
		{
			PluralFormsNode pluralFormsNode3 = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = RelationalExpression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			pluralFormsNode3.SetNode(1, pluralFormsNode);
			pluralFormsNode3.SetNode(0, pluralFormsNode2);
			return pluralFormsNode3;
		}
		return pluralFormsNode2;
	}

	private PluralFormsNode MultiplicativeExpression()
	{
		PluralFormsNode pluralFormsNode = PmExpression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		PluralFormsNode pluralFormsNode2 = pluralFormsNode;
		if (Token.TokenType == PluralFormsToken.Type.Reminder)
		{
			PluralFormsNode pluralFormsNode3 = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = PmExpression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			pluralFormsNode3.SetNode(1, pluralFormsNode);
			pluralFormsNode3.SetNode(0, pluralFormsNode2);
			return pluralFormsNode3;
		}
		return pluralFormsNode2;
	}

	private PluralFormsNode RelationalExpression()
	{
		PluralFormsNode pluralFormsNode = MultiplicativeExpression();
		if (pluralFormsNode == null)
		{
			return null;
		}
		PluralFormsNode pluralFormsNode2 = pluralFormsNode;
		if (Token.TokenType == PluralFormsToken.Type.Greater || Token.TokenType == PluralFormsToken.Type.Less || Token.TokenType == PluralFormsToken.Type.GreaterOrEqual || Token.TokenType == PluralFormsToken.Type.LessOrEqual)
		{
			PluralFormsNode pluralFormsNode3 = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
			pluralFormsNode = MultiplicativeExpression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			pluralFormsNode3.SetNode(1, pluralFormsNode);
			pluralFormsNode3.SetNode(0, pluralFormsNode2);
			return pluralFormsNode3;
		}
		return pluralFormsNode2;
	}

	private PluralFormsNode PmExpression()
	{
		PluralFormsNode result;
		if (Token.TokenType == PluralFormsToken.Type.N || Token.TokenType == PluralFormsToken.Type.Number)
		{
			result = new PluralFormsNode(new PluralFormsToken(Token));
			if (!NextToken())
			{
				return null;
			}
		}
		else
		{
			if (Token.TokenType != PluralFormsToken.Type.LeftBracket)
			{
				return null;
			}
			if (!NextToken())
			{
				return null;
			}
			PluralFormsNode pluralFormsNode = Expression();
			if (pluralFormsNode == null)
			{
				return null;
			}
			result = pluralFormsNode;
			if (Token.TokenType != PluralFormsToken.Type.RightBracket)
			{
				return null;
			}
			if (!NextToken())
			{
				return null;
			}
		}
		return result;
	}
}
