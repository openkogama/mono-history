using System.Collections.Generic;

public class ValueInsert
{
	private List<object> values = new List<object>();

	public ValueInsert AddInt(int input)
	{
		values.Add(input);
		return this;
	}

	public ValueInsert AddString(string input)
	{
		values.Add(input);
		return this;
	}

	public ValueInsert AddFloat(float input)
	{
		values.Add(input);
		return this;
	}

	public object[] GetValueParams()
	{
		return values.ToArray();
	}
}
