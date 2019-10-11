using System;
using System.Collections.Generic;
using System.Text;
using SharpRaven.Data;
using SharpRaven.Utilities;
using UnityEngine;

namespace SharpRaven;

public class RavenClient
{
	public DSN CurrentDSN { get; set; }

	public string Logger { get; set; }

	public RavenClient(string dsn)
	{
		CurrentDSN = new DSN(dsn);
		Logger = "root";
	}

	public RavenClient(DSN dsn)
	{
		CurrentDSN = dsn;
		Logger = "root";
	}

	public int CaptureException(Exception e)
	{
		return CaptureException(e, null, null);
	}

	public int CaptureException(Exception e, Dictionary<string, string> tags)
	{
		return CaptureException(e, tags, null);
	}

	public int CaptureException(Exception e, Dictionary<string, string> tags, object extra = null)
	{
		JsonPacket jsonPacket = new JsonPacket(CurrentDSN.ProjectID, e);
		jsonPacket.Level = ErrorLevel.error;
		jsonPacket.Tags = tags;
		jsonPacket.Extra = extra;
		SendAsAsyncWebRequest(jsonPacket, CurrentDSN);
		return 0;
	}

	public int CaptureUntiyLog(string log, string stack, LogType logType, Dictionary<string, string> tags = null, object extra = null)
	{
		JsonPacket jsonPacket = new JsonPacket(CurrentDSN.ProjectID, log, stack, logType);
		jsonPacket.Level = ErrorLevel.error;
		jsonPacket.Tags = tags;
		jsonPacket.Extra = extra;
		SendAsAsyncWebRequest(jsonPacket, CurrentDSN);
		return 0;
	}

	public int CaptureMessage(string message)
	{
		return CaptureMessage(message, ErrorLevel.info, null, null);
	}

	public int CaptureMessage(string message, ErrorLevel level)
	{
		return CaptureMessage(message, level, null, null);
	}

	public int CaptureMessage(string message, ErrorLevel level, Dictionary<string, string> tags)
	{
		return CaptureMessage(message, level, tags, null);
	}

	public int CaptureMessage(string message, ErrorLevel level, Dictionary<string, string> tags, object extra)
	{
		JsonPacket jsonPacket = new JsonPacket(CurrentDSN.ProjectID);
		jsonPacket.Message = message;
		jsonPacket.Level = level;
		jsonPacket.Tags = tags;
		jsonPacket.Extra = extra;
		SendAsAsyncWebRequest(jsonPacket, CurrentDSN);
		return 0;
	}

	public bool SendAsAsyncWebRequest(JsonPacket packet, DSN dsn)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept", "application/json");
		dictionary.Add("Content-Type", "application/json; charset=utf-8");
		dictionary.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
		dictionary.Add("User-Agent", "SharpRaven/1.0.0.0");
		string packetData = packet.Serialize();
		AsyncWWWManager.WWWRequest(new CustomPostRequest(dsn.SentryURI, CreatePostData(packetData), dictionary, null, WWWRequestPriority.ExecuteIgnoreAllConstraints));
		return true;
	}

	private byte[] CreatePostData(string packetData)
	{
		return Encoding.UTF8.GetBytes(packetData);
	}
}
