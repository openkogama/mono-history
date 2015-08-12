using System.Collections.Generic;
using UnityEngine;

public class AssetBundleEditorTestSet : ScriptableObject
{
	public const string AssetNameExt = "AssetBundleEditorTestList.asset";

	public const string AssetName = "AssetBundleEditorTestList";

	public const string ProjAssetDirPath = "Assets/Resources/Editor/";

	public List<AssetBundleInfo> bundleInfos;

	public List<string> bundleInfosPaths;

	public List<string> bundleURIs;

	private static AssetBundleEditorTestSet _instance;

	public static AssetBundleEditorTestSet Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load("Editor/AssetBundleEditorTestList", typeof(AssetBundleEditorTestSet)) as AssetBundleEditorTestSet;
			}
			return _instance;
		}
	}

	public AssetBundleEditorTestSet()
	{
		bundleInfos = new List<AssetBundleInfo>();
		bundleInfosPaths = new List<string>();
		bundleURIs = new List<string>();
	}

	public void Add(string bundleInfoPath, AssetBundleInfo bundleInfo, string bundleURI)
	{
		int num = bundleInfosPaths.FindIndex((string p) => p == bundleInfoPath);
		if (0 <= num)
		{
			bundleInfos[num] = bundleInfo;
			bundleURIs[num] = bundleURI;
		}
		else
		{
			bundleInfos.Add(bundleInfo);
			bundleInfosPaths.Add(bundleInfoPath);
			bundleURIs.Add(bundleURI);
		}
	}

	public void Remove(string bundleInfoPath)
	{
		int num = bundleInfosPaths.FindIndex((string p) => p == bundleInfoPath);
		if (0 <= num)
		{
			bundleInfosPaths.RemoveAt(num);
			bundleInfos.RemoveAt(num);
			bundleURIs.RemoveAt(num);
		}
	}

	public bool Contains(string bundleInfoPath)
	{
		return bundleInfosPaths.Contains(bundleInfoPath);
	}

	public bool ContainsAny()
	{
		return bundleInfosPaths.Count != 0;
	}

	public void Clear()
	{
		bundleInfos.Clear();
		bundleInfosPaths.Clear();
		bundleURIs.Clear();
	}
}
