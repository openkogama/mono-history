using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using MV.Common;
using UnityEngine;

public class BrowserComm : MonoBehaviour
{
	public static class ToWeb
	{
		private static int callbackIdCounter;

		private static string prefix = "UNITY_";

		public static void ExternalCall(string functionName, params object[] args)
		{
			Application.ExternalCall(ToNameSpace(functionName), args);
		}

		public static void ExternalCall(string functionName, Action<Dictionary<string, object>> callback)
		{
			callbacks.Add(callbackIdCounter, new Callback(functionName, callback));
			Hashtable hashtable = new Hashtable();
			hashtable.Add("callbackId", callbackIdCounter);
			Hashtable value = hashtable;
			string text = JsonWriter.Serialize(value);
			Application.ExternalCall(ToNameSpace(functionName), new object[1] { text });
			callbackIdCounter++;
		}

		private static string ToNameSpace(string functionName)
		{
			return prefix + functionName;
		}
	}

	private static class DataValidator
	{
		private class DataValidationDefinition
		{
			private Type expectedType;

			private bool isOptional;

			public DataValidationDefinition(Type expectedType, bool isOptional = false)
			{
				this.expectedType = expectedType;
				this.isOptional = isOptional;
			}

			public virtual void Validate(object value)
			{
				if (!isOptional && value == null)
				{
					Debug.LogError((object)"Throw exception");
					throw new InvalidCallbackData("None optional key not found. " + this);
				}
				if ((!isOptional || value != null) && (object)value.GetType() != expectedType)
				{
					throw new InvalidCallbackData($"Value has type: {value.GetType()}. {this}");
				}
			}

			public override string ToString()
			{
				return $"Expected type: {expectedType}.\nIsOptional: {isOptional}.";
			}
		}

		private class DataValidationDefinitionGroup(bool isOptional = false) : DataValidationDefinition(typeof(Dictionary<string, object>), isOptional)
		{
			private Dictionary<string, DataValidationDefinition> dataValidationDefinitions = new Dictionary<string, DataValidationDefinition>();

			public override void Validate(object value)
			{
				base.Validate(value);
				Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
				foreach (KeyValuePair<string, DataValidationDefinition> dataValidationDefinition in dataValidationDefinitions)
				{
					string key = dataValidationDefinition.Key;
					DataValidationDefinition value2 = dataValidationDefinition.Value;
					object value3 = null;
					if (dictionary.ContainsKey(key))
					{
						value3 = dictionary[key];
					}
					try
					{
						value2.Validate(value3);
					}
					catch (InvalidCallbackData invalidCallbackData)
					{
						invalidCallbackData.Key = key;
						throw invalidCallbackData;
					}
				}
			}

			public void Add(string key, DataValidationDefinitionGroup validationDefinition)
			{
				dataValidationDefinitions.Add(key, validationDefinition);
			}

			public void Add(string key, Type type, bool isOptional = false)
			{
				dataValidationDefinitions.Add(key, new DataValidationDefinition(type, isOptional));
			}
		}

		private static readonly Dictionary<string, Func<DataValidationDefinition>> dataDefinitions = new Dictionary<string, Func<DataValidationDefinition>>
		{
			{ "sendPlayerParams", GetSendPlayerParams },
			{ "requestRewardData", GetRequestRewardData }
		};

		private static DataValidationDefinition GetSendPlayerParams()
		{
			DataValidationDefinitionGroup dataValidationDefinitionGroup = new DataValidationDefinitionGroup();
			dataValidationDefinitionGroup.Add("serverIP", typeof(string));
			dataValidationDefinitionGroup.Add("profileID", typeof(int));
			dataValidationDefinitionGroup.Add("planetID", typeof(int));
			dataValidationDefinitionGroup.Add("gameMode", typeof(int));
			dataValidationDefinitionGroup.Add("language", typeof(string));
			dataValidationDefinitionGroup.Add("planetName", typeof(string), isOptional: true);
			return dataValidationDefinitionGroup;
		}

		private static DataValidationDefinition GetRequestRewardData()
		{
			DataValidationDefinitionGroup dataValidationDefinitionGroup = new DataValidationDefinitionGroup();
			dataValidationDefinitionGroup.Add("rewardEnabled", typeof(bool));
			dataValidationDefinitionGroup.Add("timeInSeconds", typeof(int));
			dataValidationDefinitionGroup.Add("gold", typeof(int));
			dataValidationDefinitionGroup.Add("silver", typeof(int));
			return dataValidationDefinitionGroup;
		}

		public static void Validate(string functionName, Dictionary<string, object> package)
		{
			Func<DataValidationDefinition> func = dataDefinitions[functionName];
			if (func == null)
			{
				throw new InvalidCallbackData("Data definition not found ", functionName);
			}
			try
			{
				func().Validate(package);
			}
			catch (InvalidCallbackData invalidCallbackData)
			{
				invalidCallbackData.Function = functionName;
				throw invalidCallbackData;
			}
		}
	}

	private class Callback
	{
		private string functionName = string.Empty;

		private Action<Dictionary<string, object>> callbackFunction;

		public Callback(string functionName, Action<Dictionary<string, object>> callbackFunction)
		{
			this.functionName = functionName;
			this.callbackFunction = callbackFunction;
		}

		public void Execute(Dictionary<string, object> data)
		{
			try
			{
				DataValidator.Validate(functionName, data);
				callbackFunction(data);
			}
			catch (InvalidCallbackData invalidCallbackData)
			{
				Debug.LogError((object)invalidCallbackData);
			}
		}
	}

	private class InvalidCallbackData : Exception
	{
		private string key = string.Empty;

		private string function = string.Empty;

		private string message = string.Empty;

		public string Key
		{
			set
			{
				if (!(key != string.Empty))
				{
					key = value;
				}
			}
		}

		public string Function
		{
			set
			{
				function = value;
			}
		}

		public override string Message
		{
			get
			{
				string text = $"\nFunction: {function}.\nError:{message}\n";
				if (key != string.Empty)
				{
					text = text + "Key: " + key;
				}
				return text;
			}
		}

		public InvalidCallbackData(string message)
		{
			this.message = message;
		}

		public InvalidCallbackData(string message, string function)
			: this(message)
		{
			this.function = function;
		}
	}

	private static string browserName = "browser name not set";

	private static int browserVersion = -1;

	private static Dictionary<int, Callback> callbacks = new Dictionary<int, Callback>();

	public static string BrowserName => browserName;

	public static int BrowserVersion => browserVersion;

	public void Awake()
	{
		if (!Application.isEditor)
		{
			Application.ExternalCall("get_browser_version", new object[0]);
		}
	}

	public void CreatePlanetScreenshot()
	{
		if (!((Object)(object)((Component)MVGameController.Instance).gameObject == (Object)null) && MVGameController.Instance.Game.JoinState == MVJoinState.Playing && MVGameController.Instance.Game.GameMode == MVGameMode.Edit)
		{
			MVGameController.Instance.Game.UploadGameScreenShot();
		}
	}

	public void PublishPlanetFromWeb()
	{
		MVGameController.Instance.Game.PublishPlanet();
	}

	public void GiveBrowserInfo(string browserinfo)
	{
		string[] array = browserinfo.Split(new char[1] { ',' });
		browserName = array[0];
		browserVersion = int.Parse(array[1]);
	}

	public void ExternalCallback(string jsonData)
	{
		Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
		if (!dictionary.ContainsKey("callbackId"))
		{
			Debug.LogError((object)"package does not contain callbackId");
			return;
		}
		int num = (int)dictionary["callbackId"];
		if (!callbacks.ContainsKey(num))
		{
			Debug.LogError((object)("No callback function with callbackId " + num));
			return;
		}
		Callback callback = callbacks[num];
		callbacks.Remove(num);
		if (!dictionary.ContainsKey("data") && !dictionary.ContainsKey("error"))
		{
			Debug.LogError((object)"package does not contain data or error");
			return;
		}
		if (dictionary.ContainsKey("data") && dictionary.ContainsKey("error"))
		{
			Debug.LogError((object)"package contains both data and error. These are mutually exclusive");
		}
		if (dictionary.ContainsKey("error"))
		{
			HandleError((Dictionary<string, object>)dictionary["error"]);
			return;
		}
		if (!dictionary.ContainsKey("data"))
		{
			Debug.LogError((object)"package does not contain data");
			return;
		}
		if (dictionary["data"] == null)
		{
			Debug.LogError((object)"Data is null ");
			return;
		}
		Dictionary<string, object> data = (Dictionary<string, object>)dictionary["data"];
		callback.Execute(data);
	}

	public void Exit()
	{
		if (MVGameController.Instance.Game != null)
		{
			MVGameController.Instance.Game.Leave();
		}
	}

	public void TestValidator()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("serverIP", "asdmkl");
		dictionary.Add("profileID", 34);
		dictionary.Add("planetID", 3);
		dictionary.Add("gameMode", 0);
		dictionary.Add("language", "0");
		dictionary.Add("planetName", "0");
		try
		{
			Debug.Log((object)"Validating");
			DataValidator.Validate("sendPlayerParams", dictionary);
		}
		catch (InvalidCallbackData invalidCallbackData)
		{
			Debug.LogError((object)invalidCallbackData);
		}
	}

	private void HandleError(Dictionary<string, object> error)
	{
		string text = "JavaScript externalCall error: ";
		foreach (KeyValuePair<string, object> item in error)
		{
			text += $"{item.Key}, {item.Value}\n";
		}
		Debug.LogError((object)text);
	}
}
