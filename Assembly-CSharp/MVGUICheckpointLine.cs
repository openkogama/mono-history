using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUICheckpointLine : UXLine
{
	private List<UXText> texts;

	private UXAdvancedTextBuilder textBuilder;

	public Color nameColor = new Color(0.1f, 0.1f, 0.7f);

	public MVGUICheckpointLine()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	public void BuildLine(GameMessages.CheckpointMessage data)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)textBuilder == (Object)null)
		{
			textBuilder = ((Component)this).gameObject.AddComponent<UXAdvancedTextBuilder>();
		}
		textBuilder.ignoreClipping = true;
		MVPlayer mVPlayer = MVGameController.Instance.Game.Players[data.playerID];
		textBuilder.AddText(mVPlayer.Username, nameColor);
		textBuilder.AddText(" reached a new checkpoint");
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
