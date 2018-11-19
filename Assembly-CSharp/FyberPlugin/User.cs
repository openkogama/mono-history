using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using FyberPlugin.LitJson;
using UnityEngine;

namespace FyberPlugin;

public class User
{
	[Obfuscation(Exclude = true)]
	private class JsonResponse<T>
	{
		public bool success { get; set; }

		public string key { get; set; }

		public T value { get; set; }

		public string error { get; set; }
	}

	protected const string AGE = "age";

	protected const string BIRTHDATE = "birthdate";

	protected const string GENDER = "gender";

	protected const string SEXUAL_ORIENTATION = "sexual_orientation";

	protected const string ETHNICITY = "ethnicity";

	protected const string MARITAL_STATUS = "marital_status";

	protected const string NUMBER_OF_CHILDRENS = "children";

	protected const string ANNUAL_HOUSEHOLD_INCOME = "annual_household_income";

	protected const string EDUCATION = "education";

	protected const string ZIPCODE = "zipcode";

	protected const string INTERESTS = "interests";

	protected const string IAP = "iap";

	protected const string IAP_AMOUNT = "iap_amount";

	protected const string NUMBER_OF_SESSIONS = "number_of_sessions";

	protected const string PS_TIME = "ps_time";

	protected const string LAST_SESSION = "last_session";

	protected const string CONNECTION = "connection";

	protected const string GDPR_CONSENT = "gdpr_consent";

	protected const string DEVICE = "device";

	protected const string APP_VERSION = "app_version";

	protected const string LOCATION = "fyberlocation";

	static User()
	{
	}

	protected static void NativePut(string json)
	{
		Utils.printWarningMessage();
	}

	protected static string GetJsonMessage(string key)
	{
		Utils.printWarningMessage();
		return "{\"success\":false,\"error\":\"Unsupported platform\":\"key\":" + key + "}";
	}

	public static void SetAge(int age)
	{
		Put("age", age);
	}

	public static DateTime? GetBirthdate()
	{
		string s = Get<string>("birthdate");
		if (DateTime.TryParseExact(s, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
		{
			return result;
		}
		return null;
	}

	public static void SetBirthdate(DateTime birthdate)
	{
		Put("birthdate", birthdate);
	}

	public static void SetGender(UserGender gender)
	{
		Put("gender", gender);
	}

	public static void SetSexualOrientation(UserSexualOrientation sexualOrientation)
	{
		Put("sexual_orientation", sexualOrientation);
	}

	public static void SetEthnicity(UserEthnicity ethnicity)
	{
		Put("ethnicity", ethnicity);
	}

	public static Location GetLocation()
	{
		return Get<Location>("fyberlocation");
	}

	public static void SetLocation(Location location)
	{
		Put("fyberlocation", location);
	}

	public static void SetMaritalStatus(UserMaritalStatus maritalStatus)
	{
		Put("marital_status", maritalStatus);
	}

	public static void SetNumberOfChildrens(int numberOfChildrens)
	{
		Put("children", numberOfChildrens);
	}

	public static void SetAnnualHouseholdIncome(int annualHouseholdIncome)
	{
		Put("annual_household_income", annualHouseholdIncome);
	}

	public static void SetEducation(UserEducation education)
	{
		Put("education", education);
	}

	public static string GetZipcode()
	{
		return Get<string>("zipcode");
	}

	public static void SetZipcode(string zipcode)
	{
		Put("zipcode", zipcode);
	}

	public static string[] GetInterests()
	{
		return Get<string[]>("interests");
	}

	public static void SetInterests(string[] interests)
	{
		Put("interests", interests);
	}

	public static void SetIap(bool iap)
	{
		Put("iap", iap);
	}

	public static void SetIapAmount(float iap_amount)
	{
		Put("iap_amount", (double)iap_amount);
	}

	public static void SetNumberOfSessions(int numberOfSessions)
	{
		Put("number_of_sessions", numberOfSessions);
	}

	public static void SetPsTime(long? ps_time)
	{
		Put("ps_time", ps_time);
	}

	public static void SetLastSession(long? lastSession)
	{
		Put("last_session", lastSession);
	}

	public static void SetConnection(UserConnection connection)
	{
		Put("connection", connection);
	}

	public static void SetGdprConsent(bool gdprConsent)
	{
		Put("gdpr_consent", gdprConsent);
	}

	public static string GetDevice()
	{
		return Get<string>("device");
	}

	public static void SetDevice(string device)
	{
		Put("device", device);
	}

	public static string GetAppVersion()
	{
		return Get<string>("app_version");
	}

	public static void SetAppVersion(string appVersion)
	{
		Put("app_version", appVersion);
	}

	public static void PutCustomValue(string key, string value)
	{
		Put(key, value);
	}

	public static string GetCustomValue(string key)
	{
		return Get<string>(key);
	}

	private static void Put(string key, object value)
	{
		string json = GeneratePutJsonString(key, value);
		NativePut(json);
	}

	protected static T Get<T>(string key)
	{
		string jsonMessage = GetJsonMessage(key);
		JsonResponse<T> jsonResponse = JsonMapper.ToObject<JsonResponse<T>>(jsonMessage);
		if (jsonResponse.success)
		{
			return jsonResponse.value;
		}
		Debug.Log(jsonResponse.error);
		return default;
	}

	private static string GeneratePutJsonString(string key, object value)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("action", "put");
		dictionary.Add("key", key);
		dictionary.Add("type", value.GetType().ToString());
		if (value is DateTime)
		{
			dictionary.Add("value", ((DateTime)value).ToString("yyyy/MM/dd"));
		}
		else
		{
			dictionary.Add("value", value);
		}
		return JsonMapper.ToJson(dictionary);
	}

	protected static string GenerateGetJsonString(string key)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action", "get");
		dictionary.Add("key", key);
		return JsonMapper.ToJson(dictionary);
	}
}
