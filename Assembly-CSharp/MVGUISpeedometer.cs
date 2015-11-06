using UnityEngine;

public class MVGUISpeedometer : UXViewScript
{
	public float fadeTime = 2f;

	public UXGroup speedGroup;

	public UXText speedText;

	private float curSpeed;

	private void Update()
	{
		if (MVGameControllerBase.WOCM == null || MVGameControllerBase.Game.PlayerController.CurrentWorldObject == null)
		{
			return;
		}
		MVRigidBody component = MVGameControllerBase.Game.PlayerController.CurrentWorldObject.GameObject.GetComponent<MVRigidBody>();
		if (component == null)
		{
			if (View.isVisible)
			{
				View.Hide();
			}
			return;
		}
		float b = component.Velocity.magnitude * 3.6f;
		curSpeed = Mathf.Lerp(curSpeed, b, Time.deltaTime);
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
		StopAllCoroutines();
		StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			speedGroup.SetAlpha(t, string.Empty);
		}));
	}

	public override void OnHide()
	{
		StopAllCoroutines();
		StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			speedGroup.SetAlpha(t, string.Empty);
			if (t == 0f)
			{
				speedGroup.SetVisible(visible: false);
			}
		}));
	}
}
