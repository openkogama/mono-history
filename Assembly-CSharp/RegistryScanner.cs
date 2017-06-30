using System.Collections.Generic;
using Microsoft.Win32;
using UnityEngine;

public static class RegistryScanner
{
	private static readonly Dictionary<string, RegistryKey> registerRoots = new Dictionary<string, RegistryKey>
	{
		{
			"HKEY_CLASSES_ROOT",
			Registry.ClassesRoot
		},
		{
			"HKEY_CURRENT_CONFIG",
			Registry.CurrentConfig
		},
		{
			"HKEY_CURRENT_USER",
			Registry.CurrentUser
		},
		{
			"HKEY_DYN_DATA",
			Registry.DynData
		},
		{
			"HKEY_LOCAL_MACHINE",
			Registry.LocalMachine
		},
		{
			"HKEY_PERFORMANCE_DATA",
			Registry.PerformanceData
		},
		{
			"HKEY_USERS",
			Registry.Users
		}
	};

	private static RegistryKey GetRoot(string root)
	{
		return registerRoots[root];
	}

	public static void StartScan(HackingToolDetector.ApplicationDesc[] banList)
	{
		for (int i = 0; i < banList.Length; i++)
		{
			RegistrySearch(banList[i]);
		}
	}

	private static void RegistrySearch(HackingToolDetector.ApplicationDesc appDesc)
	{
		for (int i = 0; i < appDesc.associatedRegistryKeys.Length; i++)
		{
			HackingToolDetector.ApplicationDesc.RegistryKey registryKey = appDesc.associatedRegistryKeys[i];
			string regPath = GetRegPath(registryKey);
			if (regPath != null)
			{
				HackingToolDetector.instance.detectedHackingTools.Enqueue(new HackingToolDetector.HackingToolReport(appDesc, registryKey));
				continue;
			}
			Debug.Log("Registry key " + registryKey.Name + " associated with " + appDesc.ProgramName + " not found.");
		}
	}

	private static string GetRegPath(HackingToolDetector.ApplicationDesc.RegistryKey key)
	{
		string name = key.Name;
		string text = name.Substring(0, name.IndexOf('\\'));
		int num = name.IndexOf('\\') + 1;
		int num2 = name.LastIndexOf('\\');
		string text2 = string.Empty;
		if (num2 > num)
		{
			text2 = name.Substring(num, num2 - num);
		}
		string text3 = name.Substring(num2 + 1);
		RegistryKey root = GetRoot(text);
		if (root != null)
		{
			RegistryKey registryKey = root.OpenSubKey(text2);
			if (registryKey != null)
			{
				if (key.StrictComparison)
				{
					RegistryKey registryKey2 = registryKey.OpenSubKey(text3);
					if (registryKey2 != null)
					{
						return text + '\\' + text2 + '\\' + text3;
					}
				}
				else
				{
					string[] subKeyNames = registryKey.GetSubKeyNames();
					for (int i = 0; i < subKeyNames.Length; i++)
					{
						RegistryKey registryKey3 = registryKey.OpenSubKey(subKeyNames[i]);
						string name2 = registryKey3.Name;
						name2 = name2.Substring(name2.LastIndexOf('\\') + 1).ToLowerInvariant();
						string value = text3.ToLowerInvariant();
						if (name2.StartsWith(value))
						{
							return text + '\\' + text2 + '\\' + text3;
						}
					}
				}
			}
		}
		return null;
	}
}
