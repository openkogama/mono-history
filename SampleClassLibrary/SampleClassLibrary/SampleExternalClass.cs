using System.Collections.Generic;

namespace SampleClassLibrary;

public class SampleExternalClass
{
	public string SampleString { get; set; }

	public Dictionary<int, string> SampleDictionary { get; set; }

	public SampleExternalClass()
	{
		SampleDictionary = new Dictionary<int, string>();
	}
}
