using System;
using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public class BrowserComm : MonoBehaviour
{
	private class JsonReturnData
	{
		public int callbackId = -1;

		public string data;

		public string error;

		public bool Validate()
		{
			if (callbackId == -1)
			{
				Debug.LogError("package does not contain callbackId");
				return false;
			}
			if (string.IsNullOrEmpty(data) && string.IsNullOrEmpty(error))
			{
				Debug.LogError("package does not contain data or error");
				return false;
			}
			if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(error))
			{
				Debug.LogError("package contains both data and error. These are mutually exclusive");
			}
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError("JavaScript externalCall error: " + error);
				return false;
			}
			if (string.IsNullOrEmpty(data))
			{
				Debug.LogError("package does not contain data");
				return false;
			}
			return true;
		}
	}

	public static class ToJavaScript
	{
		private static int callbackIdCounter;

		private static string prefix = "UNITY_";

		public static void GetBrowserVersion()
		{
			if (enableExternalCall)
			{
				Application.ExternalCall("get_browser_version");
			}
		}

		public static void ExternalCall(string functionName, params object[] args)
		{
			if (enableExternalCall)
			{
				Application.ExternalCall(ToNameSpace(functionName), args);
			}
		}

		public static void ExternalCall(string functionName, Action<bool, string> callback)
		{
			if (enableExternalCall)
			{
				callbacks.Add(callbackIdCounter, new Callback(callback));
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("callbackId", callbackIdCounter);
				Dictionary<object, object> value = dictionary;
				string text = JsonConvert.SerializeObject(value);
				Application.ExternalCall(ToNameSpace(functionName), text);
				callbackIdCounter++;
			}
		}

		private static string ToNameSpace(string functionName)
		{
			return prefix + functionName;
		}
	}

	private class Callback
	{
		private Action<bool, string> callbackFunction;

		public Callback(Action<bool, string> callbackFunction)
		{
			this.callbackFunction = callbackFunction;
		}

		public void Execute(bool success, string data)
		{
			callbackFunction(success, data);
		}
	}

	private static string browserName = "browser name not set";

	private static int browserVersion = -1;

	private static Dictionary<int, Callback> callbacks = new Dictionary<int, Callback>();

	public static bool enableExternalCall = true;

	public static bool enableBrowserRequest = true;

	public static string BrowserName => browserName;

	public static int BrowserVersion => browserVersion;

	public static void ExecuteBrowserRequest(string browserRequest)
	{
		if (enableBrowserRequest)
		{
			Application.OpenURL(browserRequest);
		}
	}

	public void CreatePlanetScreenshot()
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing && MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			MVGameControllerBase.OperationRequests.UploadGameScreenShot();
		}
	}

	public void PublishPlanetFromWeb()
	{
		string errorText = string.Empty;
		MVGameControllerBase.OperationRequests.PublishPlanet(ref errorText);
	}

	public void GiveBrowserInfo(string browserinfo)
	{
		string[] array = browserinfo.Split(',');
		browserName = array[0];
		browserVersion = int.Parse(array[1]);
	}

	public void ExternalCallback(string jsonData)
	{
		JsonReturnData jsonReturnData = JsonConvert.DeserializeObject<JsonReturnData>(jsonData);
		if (!callbacks.ContainsKey(jsonReturnData.callbackId))
		{
			Debug.LogWarning("No callback function with callbackId " + jsonReturnData.callbackId);
			return;
		}
		Callback callback = callbacks[jsonReturnData.callbackId];
		callbacks.Remove(jsonReturnData.callbackId);
		if (jsonReturnData.Validate())
		{
			callback.Execute(success: true, jsonReturnData.data);
		}
		else
		{
			callback.Execute(success: false, jsonReturnData.error);
		}
	}

	public void Exit()
	{
		Debug.LogWarning("This does nothing remove from web");
	}
}
