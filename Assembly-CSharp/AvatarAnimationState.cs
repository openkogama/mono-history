using UnityEngine;

internal class AvatarAnimationState : MonoBehaviour
{
	public MeshRenderer[] poses;

	public float rotationFreq = 4f;

	public float rotationPhase = 0.4f;

	public float rotationAmplitude = 5f;

	public float bobbingFreq = 8f;

	public float bobbingAmplitude = 0.03f;

	private int activePoseIndex = -1;

	private bool isVisible;

	public void Start()
	{
		UpdateVisibilityOfPoses();
	}

	public void SetVisible(bool visible)
	{
		isVisible = visible;
		UpdateVisibilityOfPoses();
	}

	public void SetActivePoseIndex(int index)
	{
		bool flag = activePoseIndex != index;
		activePoseIndex = index;
		if (flag)
		{
			UpdateVisibilityOfPoses();
		}
	}

	private void UpdateVisibilityOfPoses()
	{
		for (int i = 0; i < poses.Length; i++)
		{
			((Renderer)poses[i]).enabled = isVisible && activePoseIndex == i;
		}
	}
}
