using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StatHat;

public static class Post
{
	private class FormPoster
	{
		private Action<WWW> callback;

		private Dictionary<string, string> Parameters;

		private string RelUrl;

		private string BaseUrl;

		public FormPoster(string base_url, string rel_url, Dictionary<string, string> parameters, Action<WWW> callback)
		{
			BaseUrl = base_url;
			Parameters = parameters;
			RelUrl = rel_url;
			this.callback = callback;
			PostForm();
		}

		public FormPoster(string base_url, string rel_url, Dictionary<string, string> parameters)
		{
			BaseUrl = base_url;
			Parameters = parameters;
			RelUrl = rel_url;
			PostForm();
		}

		private void PostForm()
		{
			byte[] postData = CreatePostData();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Content-Type"] = "application/x-www-form-urlencoded";
			AsyncWWWManager.WWWRequest(new CustomPostRequest(BaseUrl + RelUrl, postData, dictionary, callback));
		}

		private byte[] CreatePostData()
		{
			string text = string.Empty;
			foreach (string key in Parameters.Keys)
			{
				string text2 = text;
				text = text2 + encodeUriComponent(key) + "=" + encodeUriComponent(Parameters[key]) + "&";
			}
			return Encoding.UTF8.GetBytes(text);
		}

		private string encodeUriComponent(string s)
		{
			string text = s.Replace("&", "%26");
			return text.Replace(" ", "%20");
		}
	}

	private const string BaseUrl = "http://api.stathat.com";

	public static void Counter(string key, string ukey, float count)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		dictionary.Add("ukey", ukey);
		dictionary.Add("count", count.ToString());
		new FormPoster("http://api.stathat.com", "/c", dictionary);
	}

	public static void Counter(string key, string ukey, int count)
	{
		Counter(key, ukey, (float)count);
	}

	public static void Value(string key, string ukey, float value)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		dictionary.Add("ukey", ukey);
		dictionary.Add("value", value.ToString());
		new FormPoster("http://api.stathat.com", "/v", dictionary);
	}

	public static void Value(string key, string ukey, int value)
	{
		Value(key, ukey, (float)value);
	}

	public static void EzCounter(string ezkey, string stat, float count)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ezkey", ezkey);
		dictionary.Add("stat", stat);
		dictionary.Add("count", count.ToString());
		new FormPoster("http://api.stathat.com", "/ez", dictionary);
	}

	public static void EzCounter(string ezkey, string stat, int count)
	{
		EzCounter(ezkey, stat, (float)count);
	}

	public static void EzValue(string ezkey, string stat, float value)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ezkey", ezkey);
		dictionary.Add("stat", stat);
		dictionary.Add("value", value.ToString());
		new FormPoster("http://api.stathat.com", "/ez", dictionary);
	}

	public static void EzValue(string ezkey, string stat, int value)
	{
		EzValue(ezkey, stat, (float)value);
	}

	public static void Counter(string key, string ukey, float count, Action<WWW> callback)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		dictionary.Add("ukey", ukey);
		dictionary.Add("count", count.ToString());
		new FormPoster("http://api.stathat.com", "/c", dictionary, callback);
	}

	public static void Counter(string key, string ukey, int count, Action<WWW> callback)
	{
		Counter(key, ukey, (float)count, callback);
	}

	public static void Value(string key, string ukey, float value, Action<WWW> callback)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		dictionary.Add("ukey", ukey);
		dictionary.Add("value", value.ToString());
		new FormPoster("http://api.stathat.com", "/v", dictionary, callback);
	}

	public static void Value(string key, string ukey, int value, Action<WWW> callback)
	{
		Value(key, ukey, (float)value, callback);
	}

	public static void EzCounter(string ezkey, string stat, float count, Action<WWW> callback)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ezkey", ezkey);
		dictionary.Add("stat", stat);
		dictionary.Add("count", count.ToString());
		new FormPoster("http://api.stathat.com", "/ez", dictionary, callback);
	}

	public static void EzCounter(string ezkey, string stat, int count, Action<WWW> callback)
	{
		EzCounter(ezkey, stat, (float)count, callback);
	}

	public static void EzValue(string ezkey, string stat, float value, Action<WWW> callback)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ezkey", ezkey);
		dictionary.Add("stat", stat);
		dictionary.Add("value", value.ToString());
		new FormPoster("http://api.stathat.com", "/ez", dictionary, callback);
	}

	public static void EzValue(string ezkey, string stat, int value, Action<WWW> callback)
	{
		EzValue(ezkey, stat, (float)value, callback);
	}
}
