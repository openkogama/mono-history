using System.Collections.Generic;
using MV.WorldObject;

public class TeamEditor : MVLogicObject
{
	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.TeamEditor;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	protected override bool HasVisualsInPlaymode => false;

	public TeamEditor(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.TeamEditorPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		interactionFlags &= ~InteractionFlags.CanClone;
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Initialize()
	{
		base.Initialize();
		UpdateTeamNamesFromData();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		UpdateTeamNamesFromData();
	}

	private void UpdateTeamNamesFromData()
	{
		Dictionary<MVTeam, string> dictionary = new Dictionary<MVTeam, string>(MVGameControllerBase.Game.TeamManager.GetTeamNames());
		foreach (KeyValuePair<MVTeam, string> item in dictionary)
		{
			string text = item.Value;
			if (Data.ContainsKey(item.Key.ToString()))
			{
				text = (string)Data[item.Key.ToString()];
			}
			MVGameControllerBase.Game.TeamManager.UpdateTeamName(item.Key, text);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
