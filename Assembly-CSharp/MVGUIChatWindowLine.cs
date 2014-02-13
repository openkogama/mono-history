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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(chatText.Size + new Vector3(0f, 0.25f, 0f));
	}

	public void BuildLine(MVPlayer sender, string message, bool alternate)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		message = $"[{sender.Username}]: {message}";
		chatText.WordWrap = true;
		chatText.Text = message;
		bool flag = MVGameController.Instance.Game.Friends.IsFriend(sender.ProfileID);
		if (alternate || flag)
		{
			nameText.Text = $"[{sender.Username}]:";
			Transform transform = ((Component)nameText).transform;
			transform.localPosition += new Vector3(0f, (chatText.TextHeight - nameText.TextHeight) / 2f, 0f);
		}
		SetSize(Width, chatText.TextHeight + 0.5f);
		if (flag)
		{
			nameText.Color = FriendChatColor;
		}
		chatText.Color = ((!alternate) ? ChatColor : AltChatColor);
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		nameText.SetAlpha(alpha, materialProperty);
		chatText.SetAlpha(alpha, materialProperty);
	}
}
