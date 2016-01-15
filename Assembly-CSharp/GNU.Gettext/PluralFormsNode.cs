namespace GNU.Gettext;

internal class PluralFormsNode
{
	public delegate void IterateNodesDelegate(PluralFormsNode node);

	private PluralFormsToken token;

	private PluralFormsNode[] nodes = new PluralFormsNode[3];

	private RecursiveTracer tracer = new RecursiveTracer();

	public PluralFormsToken Token => token;

	public PluralFormsNode[] Nodes => nodes;

	public int NodesCount => nodes.Length;

	internal RecursiveTracer Tracer
	{
		get
		{
			return tracer;
		}
		set
		{
			tracer = value;
		}
	}

	public PluralFormsNode(PluralFormsToken token)
	{
		this.token = token;
	}

	public PluralFormsNode Node(int i)
	{
		if (i >= 0 && i <= 2)
		{
			return nodes[i];
		}
		return null;
	}

	public void SetNode(int i, PluralFormsNode n)
	{
		if (i >= 0 && i <= 2)
		{
			nodes[i] = n;
		}
	}

	public PluralFormsNode ReleaseNode(int i)
	{
		PluralFormsNode result = nodes[i];
		nodes[i] = null;
		return result;
	}

	public long Evaluate(long n)
	{
		if (Tracer != null)
		{
			Tracer.Text.AppendFormat("{0}: (n = {1}): ", token.TokenType, n);
		}
		long num = -1L;
		long num2 = -1L;
		long num3 = -1L;
		long num4 = -1L;
		switch (token.TokenType)
		{
		case PluralFormsToken.Type.Number:
			num4 = token.Number;
			break;
		case PluralFormsToken.Type.N:
			num4 = n;
			break;
		case PluralFormsToken.Type.Equal:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num == num2) ? 1 : 0);
			break;
		case PluralFormsToken.Type.NotEqual:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num != num2) ? 1 : 0);
			break;
		case PluralFormsToken.Type.Greater:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num > num2) ? 1 : 0);
			break;
		case PluralFormsToken.Type.GreaterOrEqual:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num >= num2) ? 1 : 0);
			break;
		case PluralFormsToken.Type.Less:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num < num2) ? 1 : 0);
			break;
		case PluralFormsToken.Type.LessOrEqual:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num <= num2) ? 1 : 0);
			break;
		case PluralFormsToken.Type.Reminder:
		{
			long num5 = nodes[1].Evaluate(n);
			if (num5 != 0L)
			{
				num = nodes[0].Evaluate(n);
				num4 = num % num5;
			}
			else
			{
				num4 = 0L;
			}
			Tracer.Text.AppendFormat("n0 % number = {0} % {1} = {2} | ", num, num5, num4);
			break;
		}
		case PluralFormsToken.Type.LogicalAnd:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num != 0L && num2 != 0L) ? 1 : 0);
			break;
		case PluralFormsToken.Type.LogicalOr:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num4 = ((num != 0L || num2 != 0L) ? 1 : 0);
			break;
		case PluralFormsToken.Type.Question:
			num = nodes[0].Evaluate(n);
			num2 = nodes[1].Evaluate(n);
			num3 = nodes[2].Evaluate(n);
			num4 = ((num == 0L) ? num3 : num2);
			break;
		default:
			num4 = 0L;
			break;
		}
		if (Tracer != null)
		{
			Tracer.Text.AppendFormat("{0}{1}{2} result = {3}", (num == -1) ? string.Empty : ("n0 = " + num + ", "), (num2 == -1) ? string.Empty : (", n1 = " + num2 + ", "), (num3 == -1) ? string.Empty : (", n2 = " + num3 + ", "), num4);
			Tracer.Level--;
		}
		return num4;
	}

	public override string ToString()
	{
		return $"[Node: Token={Token}]";
	}

	public static void IterateNodes(PluralFormsNode node, IterateNodesDelegate doBefore, IterateNodesDelegate doAfter)
	{
		doBefore(node);
		for (int i = 0; i < node.NodesCount; i++)
		{
			if (node.Nodes[i] != null)
			{
				IterateNodes(node.Nodes[i], doBefore, doAfter);
			}
		}
		doAfter(node);
	}
}
