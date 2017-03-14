using System;
using UnityEngine;
using UnityEngine.UI;

public class ModifierIndicator : MonoBehaviour
{
	[Serializable]
	private struct AnimatedImage
	{
		public Image image;

		public ImageAnimator animator;
	}

	private class OverlayWrapper
	{
		private AnimationCurve fadeOutCurve;

		private AnimatedImage image;

		private float fadeOutTimer;

		private Image Image
		{
			get
			{
				return image.image;
			}
			set
			{
				image.image = value;
			}
		}

		private ImageAnimator Animator
		{
			get
			{
				return image.animator;
			}
			set
			{
				image.animator = value;
			}
		}

		private float Alpha
		{
			get
			{
				return Image.color.a;
			}
			set
			{
				Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, value);
			}
		}

		public OverlayWrapper(AnimatedImage overlayImage, AnimationCurve fadeOutCurve)
		{
			image = overlayImage;
			Image.enabled = false;
			this.fadeOutCurve = fadeOutCurve;
			fadeOutTimer = fadeOutCurve.keys[fadeOutCurve.length - 1].time;
		}

		public void Update(bool active)
		{
			if (active)
			{
				Alpha = 1f;
				fadeOutTimer = 0f;
				Image.enabled = true;
				Animator.enabled = true;
			}
			else if (Alpha > 0.01f)
			{
				Animator.enabled = false;
				Alpha = fadeOutCurve.Evaluate(fadeOutTimer);
				fadeOutTimer += Time.deltaTime;
			}
			else
			{
				Image.enabled = false;
			}
		}

		public void Reset()
		{
			Image.enabled = false;
		}
	}

	private enum EOverlay : byte
	{
		Poison,
		Fire,
		Ice,
		Size
	}

	[SerializeField]
	private AnimatedImage poisonOverlay;

	[SerializeField]
	private AnimatedImage fireOverlay;

	[SerializeField]
	private AnimatedImage iceOverlay;

	[SerializeField]
	private AnimationCurve fadeOutCurve;

	private OverlayWrapper[] modifierIndicators;

	private MVInteractableBase localInteractable;

	private void OnValidate()
	{
		if (fadeOutCurve != null && fadeOutCurve.keys[fadeOutCurve.length - 1].value != 0f)
		{
			fadeOutCurve.keys[fadeOutCurve.length - 1].value = 0f;
			Debug.LogWarning("fadeOutCurve has been auto corrected to prevent lingering Modifier Inidicators.");
		}
		enabled = false;
	}

	public void Awake()
	{
		modifierIndicators = new OverlayWrapper[3];
		modifierIndicators[0] = new OverlayWrapper(poisonOverlay, fadeOutCurve);
		modifierIndicators[1] = new OverlayWrapper(fireOverlay, fadeOutCurve);
		modifierIndicators[2] = new OverlayWrapper(iceOverlay, fadeOutCurve);
		transform.SetParent(null, worldPositionStays: false);
	}

	public void Initialize(MVAvatarLocal localAvatar)
	{
		localInteractable = localAvatar.InteractableLocal;
		enabled = true;
	}

	private void Update()
	{
		bool flag = false;
		bool flag2 = false;
		flag2 = localInteractable.HasModifier(AvatarModifierPackageType.Poison);
		modifierIndicators[0].Update(!flag && flag2);
		flag = flag || flag2;
		flag2 = localInteractable.HasModifier(AvatarModifierPackageType.Fire);
		modifierIndicators[1].Update(!flag && flag2);
		flag = flag || flag2;
		flag2 = localInteractable.HasModifier(AvatarModifierPackageType.Frozen);
		modifierIndicators[2].Update(!flag && flag2);
	}

	public void ResetIndicators()
	{
		for (EOverlay eOverlay = EOverlay.Poison; (int)eOverlay < 3; eOverlay++)
		{
			modifierIndicators[(uint)eOverlay].Reset();
		}
	}
}
