using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarOfferCelebration : MonoBehaviour
{
	[SerializeField]
	private RawImage previewAvatar;

	[SerializeField]
	private AnimationCurve scaleCurve;

	[SerializeField]
	private AnimationCurve spinCurve;

	[SerializeField]
	private ParticleSystem celebratoryParticles;

	private Transform scaleTarget;

	private GameObject scaleTargetGo;

	private bool doSpin = true;

	private MVBody body;

	private float timer;

	private float speed = 0.75f;

	private float rotationSpeed = 3f;

	private AvatarPreviewer screenShooter;

	private bool hasStoppedJumping;

	public void Initialize(MVBody b, Texture rawImageTexture, AvatarPreviewer screenShooter)
	{
		body = b;
		previewAvatar.texture = rawImageTexture;
		scaleTargetGo = new GameObject("ScaleTarget");
		scaleTarget = scaleTargetGo.transform;
		scaleTarget.position = new Vector3(0f, 1f, 0f);
		body.GameObject.transform.SetParent(scaleTarget, worldPositionStays: true);
		scaleTarget.localScale = new Vector3(0f, 0f, 0f);
		body.Rotation = Quaternion.Euler(0f, 180f, 0f);
		this.screenShooter = screenShooter;
		screenShooter.previewCam.fieldOfView = 85f;
		screenShooter.FaceGameObject(b.GameObject);
	}

	private void Update()
	{
		timer += Time.deltaTime * speed;
		if (doSpin)
		{
			float num = scaleCurve.Evaluate(timer);
			float num2 = spinCurve.Evaluate(timer);
			float y = num2 * 360f * rotationSpeed + 180f;
			body.Rotation = Quaternion.Euler(0f, y, 0f);
			scaleTarget.localScale = new Vector3(num, num, num);
			if (timer >= 1f)
			{
				doSpin = false;
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary["state"] = "Jump";
				dictionary["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
				body.Animation.ComputeBlendAnimation(dictionary);
				Vector3 position = screenShooter.previewCam.transform.forward * 5f;
				position.y += 2f;
				Object.Instantiate(celebratoryParticles, position, Quaternion.identity);
				return;
			}
		}
		if (timer >= 1.7f && !hasStoppedJumping)
		{
			hasStoppedJumping = true;
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			dictionary2["state"] = "Idle";
			dictionary2["timeStamp"] = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
			body.Animation.ComputeBlendAnimation(dictionary2);
		}
		if (timer >= 4f)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData baseEventData) =>
			{
				x.Pop();
			});
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(scaleTargetGo);
		screenShooter = null;
	}
}
