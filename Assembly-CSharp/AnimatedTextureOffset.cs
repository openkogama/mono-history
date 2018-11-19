using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTextureOffset : ActivateOnAnimationBase
{
	[Serializable]
	private struct TextureOffsetAnimationData
	{
		public float textureOffset;

		public float frameToChangeTextureAt;

		public bool hasAlreadyTransitioned;
	}

	[SerializeField]
	private Renderer skinnedRenderer;

	[SerializeField]
	private Renderer renderer;

	[SerializeField]
	private float animationFrameAmount = 24f;

	[SerializeField]
	private string triggerAnimationName;

	[SerializeField]
	private List<TextureOffsetAnimationData> textureOffsetAnimationDataList;

	[SerializeField]
	private Animation animations;

	private bool isActive;

	private float animationStartTime;

	private float previousAnimationTime;

	private float offsetRatio = 1f;

	private float currentOffset;

	private float previousOffset;

	private float timer;

	private const float updateCooldown = 0.1f;

	private const float animationsFramePerSeconds = 24f;

	protected override void Start()
	{
		base.Start();
		enabled = FindAnimatedTexture();
	}

	private void Update()
	{
		UpdateAnimatedTextureOffset();
	}

	public override void OnAvatarAnimationChange(string newAnimation)
	{
		if (newAnimation == triggerAnimationName)
		{
			isActive = true;
			timer = 0f;
			animationStartTime = Time.time;
		}
		else
		{
			isActive = false;
			SetTextureOffset(0f);
		}
	}

	private bool FindAnimatedTexture()
	{
		if (!renderer)
		{
			Debug.Log("Error in AnimatedTextureOffset: Could not find a renderer on this object.");
			return false;
		}
		Texture mainTexture = renderer.material.mainTexture;
		offsetRatio = (float)mainTexture.height / (float)mainTexture.width;
		Vector2 value = new Vector2(offsetRatio, 1f);
		renderer.material.SetTextureScale("_MainTex", value);
		if (!skinnedRenderer)
		{
			Debug.Log("Error in AnimatedTextureOffset: Could not find a renderer on this object.");
			return false;
		}
		Texture mainTexture2 = skinnedRenderer.material.mainTexture;
		offsetRatio = (float)mainTexture2.height / (float)mainTexture2.width;
		Vector2 value2 = new Vector2(offsetRatio, 1f);
		skinnedRenderer.material.SetTextureScale("_MainTex", value2);
		return true;
	}

	private void UpdateAnimatedTextureOffset()
	{
		if (!isActive)
		{
			return;
		}
		if (timer <= 0f)
		{
			float num = Time.time - animationStartTime;
			if (num % (animationFrameAmount / 24f) < previousAnimationTime)
			{
				ResetTextureOffsets();
			}
			for (int i = 0; i < textureOffsetAnimationDataList.Count; i++)
			{
				if (textureOffsetAnimationDataList[i].frameToChangeTextureAt / 24f < num % (animationFrameAmount / 24f) && !textureOffsetAnimationDataList[i].hasAlreadyTransitioned)
				{
					TextureOffsetAnimationData value = textureOffsetAnimationDataList[i];
					value.hasAlreadyTransitioned = true;
					textureOffsetAnimationDataList[i] = value;
					SetTextureOffset(textureOffsetAnimationDataList[i].textureOffset);
					break;
				}
			}
			previousAnimationTime = num % (animationFrameAmount / 24f);
			timer = 0.1f;
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}

	private void SetTextureOffset(float offset)
	{
		currentOffset = offset * offsetRatio;
		if (currentOffset != previousOffset)
		{
			Vector2 value = new Vector2(currentOffset, 0f);
			skinnedRenderer.material.SetTextureOffset("_MainTex", value);
			previousOffset = currentOffset;
		}
	}

	private void ResetTextureOffsets()
	{
		for (int i = 0; i < textureOffsetAnimationDataList.Count; i++)
		{
			TextureOffsetAnimationData value = textureOffsetAnimationDataList[i];
			value.hasAlreadyTransitioned = false;
			textureOffsetAnimationDataList[i] = value;
		}
	}
}
