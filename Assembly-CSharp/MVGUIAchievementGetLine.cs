using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIAchievementGetLine : UXLine
{
	private List<UXText> texts;

	private UXAdvancedTextBuilder textBuilder;

	public Color nameColor = new Color(0.1f, 0.1f, 0.7f);

	private Dictionary<AchievementType, string> prettyMap = new Dictionary<AchievementType, string>
	{
		{
			AchievementType.TotalKill100,
			"Killer (100 Kills Total)"
		},
		{
			AchievementType.TotalKill500,
			"Destroyer (100 Kills Total)"
		},
		{
			AchievementType.TotalKill1000,
			"Monster (1000 Kills Total)"
		},
		{
			AchievementType.GameKill10,
			"Concentrated Killer (10 Kills in Game)"
		},
		{
			AchievementType.GameKill50,
			"Concentrated Destroyer (50 Kills in Game)"
		},
		{
			AchievementType.GameKill150,
			"Concentrated Monster (150 Kills in Game)"
		},
		{
			AchievementType.SessionKill1,
			"First Blood (1 Kill in Session)"
		},
		{
			AchievementType.SessionKill20,
			"Killing Spree (20 Kills in Session)"
		},
		{
			AchievementType.SessionKill50,
			"Dominator (50 Kills in Session)"
		},
		{
			AchievementType.TotalFlagCapture50,
			"Flag Enthusiast (50 Flag Captures Total)"
		},
		{
			AchievementType.TotalFlagCapture250,
			"Flag Collector (250 Flag Captures Total)"
		},
		{
			AchievementType.TotalFlagCapture500,
			"Flag Maniac (500 Flag Captures Total)"
		},
		{
			AchievementType.GameFlagCapture25,
			"Game Winner (25 Flag Captures in Game)"
		},
		{
			AchievementType.GameFlagCapture50,
			"Game Dominator (50 Flag Captures in Game)"
		},
		{
			AchievementType.GameFlagCapture100,
			"Game Maniac (100 Flag Captures in Game)"
		},
		{
			AchievementType.SessionFlagCapture1,
			"First Flag (1 Flag Capture in Session)"
		},
		{
			AchievementType.TotalTime3600,
			"KoGaMa Fan (1 Hour Spent in KoGaMa)"
		},
		{
			AchievementType.GameTime1800,
			"KoGaMa Player (30 Min Spent in Play)"
		},
		{
			AchievementType.SessionTime5,
			"Session Time (30 seconds)"
		}
	};

	public MVGUIAchievementGetLine()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	public void BuildLine(GameMessages.AchievementGetMessage data)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)textBuilder == (Object)null)
		{
			textBuilder = ((Component)this).gameObject.AddComponent<UXAdvancedTextBuilder>();
		}
		textBuilder.ignoreClipping = true;
		MVPlayer mVPlayer = MVGameController.Instance.Game.Players[data.playerId];
		string text = MakeAchievementPretty(data.achievementType);
		textBuilder.AddText(mVPlayer.Username, nameColor);
		textBuilder.AddText(" gained the achievement '");
		textBuilder.AddText(text, Color.magenta);
		textBuilder.AddText("'!");
		texts = textBuilder.BuildTexts(((Component)this).transform);
	}

	private string MakeAchievementPretty(AchievementType achievementType)
	{
		return prettyMap[achievementType];
	}

	public override Vector2 GetLineSize()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = float.MinValue;
		foreach (UXText text in texts)
		{
			num += text.TextWidth;
			num2 = Math.Max(num2, text.TextHeight);
		}
		return new Vector2(num, num2);
	}

	public override void SetAlpha(float alpha, string materialProperty)
	{
		foreach (UXText text in texts)
		{
			text.SetAlpha(alpha, materialProperty);
		}
	}
}
