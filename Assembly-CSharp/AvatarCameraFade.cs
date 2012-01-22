using UnityEngine;

public class AvatarCameraFade : MonoBehaviour
{
	public Transform cameraTfm;

	public float fadeStartDistance = 4f;

	public float fadeEndDistance = 2f;

	public Shader normalShader;

	public Shader fadeShader;

	private Transform avatarTfm;

	private MeshRenderer[] avatarRenders = new MeshRenderer[0];

	private void Update()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)avatarTfm))
		{
			MVAvatar woAvatar = MVGameController.Instance.WOCM.WoAvatar;
			if (woAvatar == null)
			{
				return;
			}
			avatarTfm = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform;
			AvatarAnimation avatarAnimation = MVGameController.Instance.WOCM.WoAvatar.Avatar.AvatarAnimation;
			avatarRenders = ((Component)avatarAnimation).GetComponentsInChildren<MeshRenderer>();
		}
		float num = Vector3.Distance(avatarTfm.position, cameraTfm.position);
		MeshRenderer[] array = avatarRenders;
		foreach (MeshRenderer val in array)
		{
			Material[] materials = ((Renderer)val).materials;
			foreach (Material val2 in materials)
			{
				val2.shader = ((!(num > fadeStartDistance)) ? fadeShader : normalShader);
				float a = Mathf.Clamp01((num - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));
				Color color = val2.color;
				color.a = a;
				val2.color = color;
			}
		}
	}
}
