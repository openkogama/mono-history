using UnityEngine;

public class MVGUIChatWindowLine : UXLine
{
	private const float HEIGHT_BUFFER = 0.25f;

	public UXText nameText;

	public UXText chatText;

	public Color ChatColor;

	public Color AltChatColor;

	public Color FriendChatColor;

	public override Vector2 GetLineSize()
	{
		return chatText.Size + new Vector3(0f, 0.25f, 0f);
	}

	public void BuildLine(string msg, Color color)
	{
		chatText.WordWrap = true;
		chatText.Text = msg;
		SetSize(Width, chatText.TextHeight + 0.5f);
		chatText.Color = color;
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		nameText.SetAlpha(alpha, materialProperty);
		chatText.SetAlpha(alpha, materialProperty);
	}
}
