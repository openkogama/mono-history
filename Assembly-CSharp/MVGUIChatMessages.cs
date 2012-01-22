using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIChatMessages : MonoBehaviour
{
	private List<MVGUIChatText> texts = new List<MVGUIChatText>();

	public void AddMessage(string sender, string message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = GetTargetTextPosition(texts.Count);
		val.transform.localScale = Vector3.one;
		MVGUIChatText mVGUIChatText = val.AddComponent<MVGUIChatText>();
		mVGUIChatText.Text = $"{sender}: {message}";
		mVGUIChatText.OnDisappear = (MVGUIChatText.OnDisappearDelegate)Delegate.Combine(mVGUIChatText.OnDisappear, new MVGUIChatText.OnDisappearDelegate(Disappear));
		texts.Add(mVGUIChatText);
		MoveTexts();
	}

	private Vector3 GetTargetTextPosition(int i)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(3.5f, -14f - (float)i * 2f, 10f);
	}

	private void Disappear(MVGUIChatText chatText)
	{
		texts.Remove(chatText);
		MoveTexts();
	}

	private void MoveTexts()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < texts.Count; i++)
		{
			MVGUIChatText mVGUIChatText = texts[i];
			mVGUIChatText.MoveToPosition(GetTargetTextPosition(i));
		}
	}
}
