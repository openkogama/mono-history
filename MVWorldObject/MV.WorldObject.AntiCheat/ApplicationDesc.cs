namespace MV.WorldObject.AntiCheat;

public class ApplicationDesc
{
	public class RegistryKey
	{
		private string name;

		private bool strictComparison;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public bool StrictComparison => strictComparison;

		public RegistryKey()
		{
		}

		public RegistryKey(string keyName, bool strictComparison)
		{
			name = keyName;
			this.strictComparison = strictComparison;
		}
	}

	private string programName;

	private string processName;

	private bool strictComparison;

	public RegistryKey[] associatedRegistryKeys;

	public string ProgramName
	{
		get
		{
			return programName;
		}
		set
		{
			programName = value;
		}
	}

	public string ProcessName
	{
		get
		{
			return processName;
		}
		set
		{
			processName = value;
		}
	}

	public bool StrictComparison => strictComparison;

	public ApplicationDesc()
	{
	}

	public ApplicationDesc(string displayName, string processName, bool strictNameComparison, RegistryKey[] associatedRegKeys)
	{
		programName = displayName;
		this.processName = processName;
		strictComparison = strictNameComparison;
		associatedRegistryKeys = associatedRegKeys;
	}
}
