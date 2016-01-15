namespace GNU.Gettext;

public class PluralFormsCalculator
{
	private int nplurals;

	private PluralFormsNode plural;

	private string expression;

	public int NPlurals => nplurals;

	public PluralFormsCalculator(string expression)
	{
		nplurals = 0;
		plural = null;
		this.expression = expression;
	}

	public long Evaluate(long n, bool traceToFile)
	{
		if (plural == null)
		{
			return 0L;
		}
		RecursiveTracer tracer = new RecursiveTracer();
		tracer.Text.AppendFormat("Expression: {0}", expression);
		tracer.Text.AppendLine();
		tracer.Text.AppendFormat("Evaluate: {0}", n);
		tracer.Text.AppendLine();
		tracer.Text.AppendLine();
		long num = plural.Evaluate(n);
		PluralFormsNode.IterateNodes(plural, (PluralFormsNode node) =>
		{
			tracer.Text.AppendFormat("{0}: ", tracer.Level++);
			tracer.Indent();
			if (node.Tracer != null)
			{
				tracer.Text.AppendLine(node.Tracer.Text.ToString());
			}
		}, (PluralFormsNode node) =>
		{
			tracer.Level--;
		});
		if (traceToFile)
		{
			tracer.SaveToFile("Evaluations.txt");
		}
		if (num < 0 || num > nplurals)
		{
			return 0L;
		}
		return num;
	}

	public long Evaluate(long n)
	{
		return Evaluate(n, traceToFile: false);
	}

	public static PluralFormsCalculator Make(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		if (str.EndsWith("\n"))
		{
			str = str.Remove(str.Length - 1, 1);
		}
		if (str.EndsWith("\\n"))
		{
			str = str.Remove(str.Length - 2, 2);
		}
		if (!str.EndsWith(";"))
		{
			str += ";";
		}
		PluralFormsCalculator pluralFormsCalculator = new PluralFormsCalculator(str);
		PluralFormsScanner scanner = new PluralFormsScanner(str);
		PluralFormsParser pluralFormsParser = new PluralFormsParser(scanner);
		if (!pluralFormsParser.Parse(pluralFormsCalculator))
		{
			return null;
		}
		return pluralFormsCalculator;
	}

	public void DumpNodes(string fileName)
	{
		if (plural != null)
		{
			RecursiveTracer tracer = new RecursiveTracer();
			tracer.Text.Append(expression);
			tracer.Text.AppendLine();
			PluralFormsNode.IterateNodes(plural, (PluralFormsNode node) =>
			{
				tracer.Text.AppendFormat("{0}: ", tracer.Level++);
				tracer.Indent();
				tracer.Text.AppendLine(node.ToString());
			}, (PluralFormsNode node) =>
			{
				tracer.Level--;
			});
			tracer.SaveToFile(fileName);
		}
	}

	internal void Init(int nplurals, PluralFormsNode plural)
	{
		this.nplurals = nplurals;
		this.plural = plural;
	}
}
