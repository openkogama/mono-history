using System;
using System.Collections.Generic;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUIPlayerKilledLine : UXLine
{
	private List<UXText> texts;

	private UXAdvancedTextBuilder textBuilder;

	public Color nameColor = new Color(0.1f, 0.1f, 0.7f);

	public MVGUIPlayerKilledLine()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	public void BuildLine(GameMessages.PlayerKilledMessage data)
	{
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)textBuilder == (Object)null)
		{
			textBuilder = ((Component)this).gameObject.AddComponent<UXAdvancedTextBuilder>();
		}
		textBuilder.ignoreClipping = true;
		MVPlayer mVPlayer = MVGameController.Instance.Game.Players[data.killerId];
		if (data.killerId != data.playerId)
		{
			MVPlayer mVPlayer2 = MVGameController.Instance.Game.Players[data.playerId];
			string[] textAsList = Localization.Instance.GetTextAsList(TextSlotIndex.StandardKillMessage);
			string[] array = textAsList;
			foreach (string text in array)
			{
				switch (text)
				{
				case "0":
					textBuilder.AddText(mVPlayer.Username, nameColor);
					break;
				case "1":
					textBuilder.AddText(mVPlayer2.Username, Color.red);
					break;
				case "2":
					textBuilder.AddText(Localization.Instance.GetText((TextSlotIndex)(int)Enum.Parse(typeof(TextSlotIndex), data.weaponType.ToString())));
					break;
				default:
					textBuilder.AddText(text);
					break;
				}
			}
		}
		else
		{
			string text2 = data.weaponType.ToString();
			if (data.weaponType == PlayerKilledByType.BazookaGun)
			{
				text2 = "BazookaSuicide";
			}
			Debug.Log((object)text2);
			string[] textAsList2 = Localization.Instance.GetTextAsList((TextSlotIndex)(int)Enum.Parse(typeof(TextSlotIndex), text2));
			string[] array2 = textAsList2;
			foreach (string text3 in array2)
			{
				if (text3 == "0")
				{
					textBuilder.AddText(mVPlayer.Username, Color.red);
				}
				else
				{
					textBuilder.AddText(text3);
				}
			}
		}
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
