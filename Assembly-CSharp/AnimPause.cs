using UnityEngine;

[RequireComponent(typeof(BoneAnimation))]
public class AnimPause : MonoBehaviour
{
	private BoneAnimation _boneAnimation;

	private void Awake()
	{
		_boneAnimation = GetComponent<BoneAnimation>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha1))
		{
			_boneAnimation.PlayAndPauseAt("Walk", 1f);
		}
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.Alpha2))
		{
			_boneAnimation.Play("Walk");
		}
	}
}
