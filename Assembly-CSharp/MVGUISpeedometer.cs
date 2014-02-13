using UnityEngine;

public class MVGUISpeedometer : UXViewScript
{
	public float fadeTime = 2f;

	public UXGroup speedGroup;

	public UXText speedText;

	private float curSpeed;

	private void Update()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.WOCM == null || MVGameController.Instance.Game.PlayerController.CurrentWorldObject == null)
		{
			return;
		}
		MVRigidBody component = MVGameController.Instance.Game.PlayerController.CurrentWorldObject.GameObject.GetComponent<MVRigidBody>();
		if ((Object)(object)component == (Object)null)
		{
			if (View.isVisible)
			{
				View.Hide();
			}
			return;
		}
		Vector3 velocity = component.Velocity;
		float num = velocity.magnitude * 3.6f;
		curSpeed = Mathf.Lerp(curSpeed, num, Time.deltaTime);
		if (curSpeed > 100f)
		{
			if (!View.isVisible)
			{
				View.Show();
			}
		}
		else if (View.isVisible)
		{
			View.Hide();
		}
		speedText.Text = $"{(int)curSpeed} km/h";
	}

	public override void OnShow()
	{
		speedGroup.SetVisible(visible: true);
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			speedGroup.SetAlpha(t);
		}));
	}

	public override void OnHide()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			speedGroup.SetAlpha(t);
			if (t == 0f)
			{
				speedGroup.SetVisible(visible: false);
			}
		}));
	}

	public void SetSpeedText(string text)
	{
		speedText.Text = text;
	}
}
