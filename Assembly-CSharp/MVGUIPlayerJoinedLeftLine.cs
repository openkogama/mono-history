using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIPlayerJoinedLeftLine : UXLine
{
	private List<UXText> texts;

	private UXAdvancedTextBuilder textBuilder;

	public void BuildLine(GameMessages.PlayerJoinMessage data)
	{
		MVPlayer mVPlayer = MVGameController.Instance.Game.Players[data.playerId];
		Build(mVPlayer.Username, " joined the game");
	}

	public void BuildLine(GameMessages.PlayerLeftMessage data)
	{
		Build(string.Empty, data.userName + " left the game");
	}

	private void Build(string username, string message)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)textBuilder == (Object)null)
		{
			textBuilder = ((Component)this).gameObject.AddComponent<UXAdvancedTextBuilder>();
		}
		textBuilder.ignoreClipping = true;
		textBuilder.AddText(username, Color.green);
		textBuilder.AddText(message);
		texts = textBuilder.BuildTexts(((Component)this).transform);
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
