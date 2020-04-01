using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePrototypeSettings;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore;

public static class KogamaSettingTools
{
	public static void EnforceSettingsConstraints(AttributeSettingWoType attributeSettingWoType, Dictionary<object, object> data)
	{
		KogamaSettingWrapperBase kogamaSettingWrapperBase = CreateFromValues(data, AttributePrototypeSettingsManager.GetRoot(attributeSettingWoType), KogamaSettingsFactory.KogamaSettingValueFactory);
		if (kogamaSettingWrapperBase != null)
		{
			Dictionary<object, object> source = KogamaSettingsToDictionary(kogamaSettingWrapperBase);
			CommonUtils.PartialUpdateHashtable(data, source);
		}
	}

	public static KogamaSettingWrapperBase CreatePrototypeWithUserValues(Dictionary<object, object> userValuesDict, KogamaSettingWrapperBase prototypeRoot, Func<KeyValuePair<object, object>, KogamaSettingValueWrapperBase, KogamaSettingsCollectionBase, KogamaSettingValueWrapperBase> factoryFunc)
	{
		KogamaSettingWrapperBase kogamaSettingWrapperBase = CreateFromValues(userValuesDict, prototypeRoot, factoryFunc);
		KogamaSettingWrapperBase kogamaSettingWrapperBase2 = CreateDeepCopy(prototypeRoot, factoryFunc);
		if (kogamaSettingWrapperBase != null)
		{
			OverrideValues(kogamaSettingWrapperBase2, kogamaSettingWrapperBase);
		}
		return kogamaSettingWrapperBase2;
	}

	public static KogamaSettingWrapperBase CreateDeepCopy(KogamaSettingWrapperBase source, Func<KeyValuePair<object, object>, KogamaSettingValueWrapperBase, KogamaSettingsCollectionBase, KogamaSettingValueWrapperBase> factoryFunc)
	{
		Dictionary<object, object> values = KogamaSettingsToDictionary(source);
		return CreateFromValues(values, source, factoryFunc);
	}

	public static void OverrideValues(KogamaSettingWrapperBase target, KogamaSettingWrapperBase source)
	{
		if (target == null)
		{
			throw new Exception("Entry not found in target");
		}
		if (source == null)
		{
			throw new Exception("Source is null");
		}
		if (source.GetType() != target.GetType())
		{
			throw new Exception("Source and target type are not the same");
		}
		if (source is KogamaSettingValueWrapperBase)
		{
			((KogamaSettingValueWrapperBase)target).KogamaSetting.Value = ((KogamaSettingValueWrapperBase)source).KogamaSetting.Value;
			return;
		}
		if (source is KogamaSettingsCollectionBase)
		{
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)source;
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase2 = (KogamaSettingsCollectionBase)target;
			{
				foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
				{
					KogamaSettingWrapperBase target2 = kogamaSettingsCollectionBase2.Children[child.Key];
					OverrideValues(target2, child.Value);
				}
				return;
			}
		}
		throw new Exception("Unknown base type");
	}

	public static KogamaSettingWrapperBase CreateFromValues(Dictionary<object, object> values, KogamaSettingWrapperBase prototypeRoot, Func<KeyValuePair<object, object>, KogamaSettingValueWrapperBase, KogamaSettingsCollectionBase, KogamaSettingValueWrapperBase> factoryFunc)
	{
		foreach (KeyValuePair<object, object> value in values)
		{
			if ((string)value.Key == prototypeRoot.Key)
			{
				return CreateFromValues(value, prototypeRoot, null, factoryFunc);
			}
		}
		return null;
	}

	public static void Traverse(KogamaSettingWrapperBase root, Action<KogamaSettingWrapperBase> callback)
	{
		if (root == null)
		{
			return;
		}
		callback(root);
		if (!(root is KogamaSettingsCollectionBase))
		{
			return;
		}
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)root;
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			Traverse(child.Value, callback);
		}
	}

	public static bool RootDestinationContains(KogamaSettingWrapperBase obj, KogamaSettingWrapperBase destinationNode)
	{
		Dictionary<object, object> settingBranch = GetSettingBranch(obj);
		bool contains = true;
		Traverse(settingBranch, (KeyValuePair<object, object> deltaSettingNode) =>
		{
			if (contains && destinationNode.Key != (string)deltaSettingNode.Key)
			{
				KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)destinationNode;
				if (kogamaSettingsCollectionBase.Children.ContainsKey((string)deltaSettingNode.Key))
				{
					destinationNode = kogamaSettingsCollectionBase.Children[(string)deltaSettingNode.Key];
				}
				else
				{
					contains = false;
				}
			}
		});
		return contains;
	}

	public static void Traverse(Dictionary<object, object> data, Action<KeyValuePair<object, object>> callback)
	{
		foreach (KeyValuePair<object, object> datum in data)
		{
			Traverse(datum, callback);
		}
	}

	private static void Traverse(KeyValuePair<object, object> data, Action<KeyValuePair<object, object>> callback)
	{
		callback(data);
		if (!(data.Value is Dictionary<object, object>))
		{
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data.Value;
		foreach (KeyValuePair<object, object> item in dictionary)
		{
			Traverse(item, callback);
		}
	}

	public static Dictionary<object, object> KogamaSettingsToDictionary(KogamaSettingWrapperBase obj)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		KogamaSettingsToDictionary(obj, dictionary);
		return dictionary;
	}

	public static KogamaSettingWrapperBase GetRoot(KogamaSettingWrapperBase obj)
	{
		while (obj.Parent != null)
		{
			obj = obj.Parent;
		}
		return obj;
	}

	public static Dictionary<object, object> GetSubTree(KogamaSettingWrapperBase obj)
	{
		Dictionary<object, object> settingBranch = GetSettingBranch(obj);
		Dictionary<object, object> dictionary = KogamaSettingsToDictionary(obj);
		Dictionary<object, object> dictionary2 = settingBranch;
		while (!dictionary2.ContainsKey(obj.Key))
		{
			using Dictionary<object, object>.Enumerator enumerator = dictionary2.GetEnumerator();
			if (enumerator.MoveNext())
			{
				dictionary2 = (Dictionary<object, object>)enumerator.Current.Value;
			}
		}
		dictionary2[obj.Key] = dictionary[obj.Key];
		return settingBranch;
	}

	public static Dictionary<object, object> GetSettingBranch(KogamaSettingWrapperBase obj)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		while (obj != null)
		{
			if (obj is KogamaSettingsCollectionBase)
			{
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				dictionary2.Add(obj.Key, dictionary);
				dictionary = dictionary2;
			}
			else
			{
				KogamaSettingValueWrapperBase kogamaSettingValueWrapperBase = (KogamaSettingValueWrapperBase)obj;
				dictionary.Add(kogamaSettingValueWrapperBase.Key, kogamaSettingValueWrapperBase.KogamaSetting.Value);
			}
			obj = obj.Parent;
		}
		return dictionary;
	}

	private static void KogamaSettingsToDictionary(KogamaSettingWrapperBase obj, Dictionary<object, object> settingsDictionary)
	{
		if (obj is KogamaSettingsCollectionBase)
		{
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)obj;
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			settingsDictionary.Add(obj.Key, dictionary);
			{
				foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
				{
					KogamaSettingsToDictionary(child.Value, dictionary);
				}
				return;
			}
		}
		KogamaSettingValueWrapperBase kogamaSettingValueWrapperBase = (KogamaSettingValueWrapperBase)obj;
		settingsDictionary.Add(kogamaSettingValueWrapperBase.Key, kogamaSettingValueWrapperBase.KogamaSetting.Value);
	}

	private static KogamaSettingWrapperBase CreateFromValues(KeyValuePair<object, object> valuePair, KogamaSettingWrapperBase prototype, KogamaSettingsCollectionBase parent, Func<KeyValuePair<object, object>, KogamaSettingValueWrapperBase, KogamaSettingsCollectionBase, KogamaSettingValueWrapperBase> factoryFunc)
	{
		if (prototype is KogamaSettingsCollectionBase)
		{
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase = ((KogamaSettingsCollectionBase)prototype).CopyWithOutChildren(parent);
			Dictionary<object, object> dictionary = (Dictionary<object, object>)valuePair.Value;
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase2 = (KogamaSettingsCollectionBase)prototype;
			{
				foreach (KeyValuePair<object, object> item in dictionary)
				{
					KogamaSettingWrapperBase prototype2 = kogamaSettingsCollectionBase2.Children[(string)item.Key];
					KogamaSettingWrapperBase kogamaSetting = CreateFromValues(item, prototype2, kogamaSettingsCollectionBase, factoryFunc);
					kogamaSettingsCollectionBase.AddChild(kogamaSetting);
				}
				return kogamaSettingsCollectionBase;
			}
		}
		KogamaSettingValueWrapperBase arg = (KogamaSettingValueWrapperBase)prototype;
		return factoryFunc(valuePair, arg, parent);
	}
}
