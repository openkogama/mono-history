using System;

namespace Sentry;

[Serializable]
public class StackTraceSpec
{
	public string filename;

	public string function;

	public string module = string.Empty;

	public int lineno;

	public bool in_app;

	public StackTraceSpec(string filename, string function, int lineNo, bool inApp)
	{
		this.filename = filename;
		this.function = function;
		lineno = lineNo;
		in_app = inApp;
	}

	public override string ToString()
	{
		return $"filename: {filename}. function {function}. module {module}. lineno {lineno}. in_app {in_app}.";
	}
}
