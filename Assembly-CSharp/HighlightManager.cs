using System.Collections.Generic;
using MV.WorldObject.HighlightSystem;
using MV.WorldObject.HighlightSystem.HighlightPayloads;
using Newtonsoft.Json;

public static class HighlightManager
{
	private static Dictionary<int, AvailableHighlightData> highlightDatas = new Dictionary<int, AvailableHighlightData>();

	public static List<Highlight<T>> GetHighLights<T>(HighlightType highlightType) where T : HighlightPayloadBase
	{
		List<Highlight<T>> list = new List<Highlight<T>>();
		foreach (KeyValuePair<int, AvailableHighlightData> highlightData2 in highlightDatas)
		{
			if (highlightData2.Value.highlightType == highlightType)
			{
				T highlightData = JsonConvert.DeserializeObject<T>(highlightData2.Value.payload);
				Highlight<T> item = new Highlight<T>(highlightData2.Value.id, highlightData);
				list.Add(item);
			}
		}
		return list;
	}

	public static void Init(string availableHighlightDatasString)
	{
		highlightDatas = JsonConvert.DeserializeObject<Dictionary<int, AvailableHighlightData>>(availableHighlightDatasString);
	}

	public static void Reset()
	{
		highlightDatas = new Dictionary<int, AvailableHighlightData>();
	}

	public static void SetHighlightToSeen(int highlightId)
	{
		highlightDatas.Remove(highlightId);
		MVGameControllerBase.Game.OperationRequestSender.SetHighlightToSeen(highlightId);
	}
}
