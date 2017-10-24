using System.Collections;
using UnityEngine;

public class NinjaRunModifier : AvatarModifier
{
	public TrailRenderer trailRenderer;

	[SerializeField]
	private float startWidth;

	[SerializeField]
	private float endWidth;

	[SerializeField]
	private float trailHeight;

	[SerializeField]
	private AudioSource soundEffect;

	private Vector3 oldPosition;

	private Vector3 oldScale;

	private float initialVolume;

	private bool isDestroying;

	private Transform ownerTransform;

	private const float minRemoteSpeed = 0.1f;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.NinjaRun;

	private void Awake()
	{
		initialVolume = soundEffect.volume;
	}

	protected override void OnActivated(Avatar target)
	{
		owner = target;
		ownerTransform = owner.transform;
		oldScale = owner.mvAvatar.Transform.localScale;
		trailRenderer.startWidth = startWidth * ownerTransform.localScale.x;
		trailRenderer.endWidth = endWidth * ownerTransform.localScale.x;
		Vector3 localPosition = trailRenderer.transform.localPosition;
		localPosition.y = trailHeight * ownerTransform.localScale.x;
		trailRenderer.transform.localPosition = localPosition;
	}

	protected override void OnDeactivated(Avatar target)
	{
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(DoFadeAndDestroy());
		}
		else
		{
			Object.Destroy(gameObject);
		}
	}

	private IEnumerator DoFadeAndDestroy()
	{
		isDestroying = true;
		soundEffect.volume = 0f;
		trailRenderer.gameObject.transform.SetParent(null);
		yield return new WaitForSeconds(trailRenderer.time);
		Object.Destroy(trailRenderer.gameObject);
		Object.Destroy(gameObject);
	}

	private void Update()
	{
		trailRenderer.enabled = !owner.IsLocal || MVGameControllerBase.CameraController.CurCamera.CameraType != CameraType.FirstPersonCamera;
		if (oldPosition != ownerTransform.position && !isDestroying)
		{
			float magnitude = (ownerTransform.position - oldPosition).magnitude;
			if (ownerTransform.localScale != oldScale)
			{
				oldScale = ownerTransform.localScale;
				trailRenderer.startWidth = startWidth * ownerTransform.localScale.x;
				trailRenderer.endWidth = endWidth * ownerTransform.localScale.x;
			}
			if (owner.IsLocal)
			{
				soundEffect.volume = Mathf.Clamp(magnitude, 0f, 1f);
			}
			else if (magnitude > 0.1f)
			{
				soundEffect.volume = 0.5f;
			}
			else
			{
				soundEffect.volume = 0f;
			}
			soundEffect.volume *= initialVolume;
			oldPosition = ownerTransform.position;
		}
	}
}
