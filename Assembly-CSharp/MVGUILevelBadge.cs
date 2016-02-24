using System;
using UnityEngine;

public class MVGUILevelBadge : MonoBehaviour
{
	[SerializeField]
	private UXPlane levelBadge;

	[SerializeField]
	private MoveAnimation moveAnimation;

	[SerializeField]
	private string outOfScreenKeyFrame;

	private Texture texture;

	private void Awake()
	{
		if (LevelingManager.IsInitialized)
		{
			Debug.Log("LevelingManager.IsInitialized");
			OnLevelingInitialized();
		}
		else
		{
			Debug.Log("!LevelingManager.IsInitialized, waiting for ");
			LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Combine(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingInitialized));
		}
		moveAnimation.SubscribeToKeyFrame(outOfScreenKeyFrame, KeyFrameCallback);
	}

	private void OnLevelingInitialized()
	{
		Debug.Log("OnLevelingInitialized");
		UpdateBadge(MVGameControllerBase.Game.LocalPlayer.Level);
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Combine(localPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(UpdateBadge));
	}

	private void UpdateBadge(int level)
	{
		BadgeManager.GetBadgeTexture(level, StreamingAssetCallback);
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (www != null && www.texture != null)
		{
			texture = www.texture;
			moveAnimation.Play();
		}
		else
		{
			Debug.Log("Failed to get: " + www.url);
		}
	}

	private void KeyFrameCallback()
	{
		if (Application.loadedLevelName != "GUIDevScene")
		{
			levelBadge.GetComponent<Renderer>().material.mainTexture = texture;
			texture = null;
			levelBadge.GetComponent<Renderer>().enabled = true;
		}
	}
}
