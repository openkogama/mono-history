using UnityEngine;

public class AdvancedGhostVisualizaton : MonoBehaviour
{
	public enum Effect
	{
		Attack,
		Die,
		Respawn,
		None
	}

	private abstract class EffectBase
	{
		protected float duration;

		protected float timeLeft;

		protected EffectBase(float duration)
		{
			this.duration = duration;
			timeLeft = duration;
		}

		protected abstract void UpdateEffect(AdvancedGhostVisualizaton ghostVisualizaton);

		public abstract void Exit(AdvancedGhostVisualizaton ghost);

		public bool Update(AdvancedGhostVisualizaton ghostVisualizaton)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0f)
			{
				timeLeft = 0f;
			}
			if (timeLeft <= 0f)
			{
				Exit(ghostVisualizaton);
				return true;
			}
			UpdateEffect(ghostVisualizaton);
			return false;
		}
	}

	private class Die : EffectBase
	{
		public Die(float duration, AdvancedGhostVisualizaton ghostVisualizaton)
			: base(duration)
		{
		}

		protected override void UpdateEffect(AdvancedGhostVisualizaton ghostVisualizaton)
		{
			Vector3 localScale = UpdateScale(ghostVisualizaton);
			ghostVisualizaton.transform.localScale = localScale;
		}

		private Vector3 UpdateScale(AdvancedGhostVisualizaton ghostVisualizaton)
		{
			float num = timeLeft / duration;
			return ghostVisualizaton.baseScale * num;
		}

		public override void Exit(AdvancedGhostVisualizaton ghost)
		{
			ghost.transform.localScale = ghost.baseScale;
			ghost.gameObject.SetActive(value: false);
		}
	}

	private class Respawn : EffectBase
	{
		public Respawn(float duration, AdvancedGhostVisualizaton ghostVisualizaton)
			: base(duration)
		{
			ghostVisualizaton.transform.localScale = Vector3.zero;
			ghostVisualizaton.gameObject.SetActive(value: true);
		}

		protected override void UpdateEffect(AdvancedGhostVisualizaton ghostVisualizaton)
		{
			Vector3 localScale = UpdateScale(ghostVisualizaton);
			ghostVisualizaton.transform.localScale = localScale;
		}

		private Vector3 UpdateScale(AdvancedGhostVisualizaton ghostVisualizaton)
		{
			float num = (duration - timeLeft) / duration;
			return ghostVisualizaton.baseScale * num;
		}

		public override void Exit(AdvancedGhostVisualizaton ghost)
		{
			ghost.transform.localScale = ghost.baseScale;
		}
	}

	private EffectBase currentEffect;

	private Vector3 baseScale;

	public GhostEye ghostEye;

	public GhostBody ghostBody;

	public AdvancedGhostBlinker blinker;

	public AudioSource moving;

	public AudioSource receiveDamage;

	public AudioSource weaponHitSound;

	private void Awake()
	{
		baseScale = transform.localScale;
	}

	public void SetRotationSpeed(float rotationSpeed)
	{
		moving.pitch = 3f * rotationSpeed;
		ghostBody.SetRotationSpeed(rotationSpeed);
	}

	public void ReceivedDamage()
	{
		if (gameObject.activeInHierarchy)
		{
			receiveDamage.Play();
			blinker.StartBlinking(BlinkType.Damage, 1.3f);
		}
	}

	public void ReceivedHealing()
	{
		if (gameObject.activeInHierarchy)
		{
			blinker.StartBlinking(BlinkType.Healing, 1.3f);
		}
	}

	public void PlayEffect(Effect effect, float duration)
	{
		if (currentEffect != null)
		{
			currentEffect.Exit(this);
		}
		switch (effect)
		{
		case Effect.Die:
			currentEffect = new Die(duration, this);
			break;
		case Effect.Respawn:
			currentEffect = new Respawn(duration, this);
			break;
		case Effect.None:
			currentEffect = null;
			break;
		}
	}

	private void Update()
	{
		if (currentEffect != null && currentEffect.Update(this))
		{
			currentEffect = null;
		}
	}

	private void Start()
	{
		blinker.Init(gameObject.GetComponentsInChildren<MeshFilter>());
	}

	private void OnEnable()
	{
		moving.Play();
	}

	private void OnDisable()
	{
		moving.Stop();
	}
}
