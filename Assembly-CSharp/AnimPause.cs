using UnityEngine;

[RequireComponent(typeof(BoneAnimation))]
public class AnimPause : MonoBehaviour
{
	private BoneAnimation _boneAnimation;

	private void Awake()
	{
		_boneAnimation = ((Component)this).GetComponent<BoneAnimation>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)49))
		{
			_boneAnimation.PlayAndPauseAt("Walk", 1f);
		}
		if (Input.GetKeyDown((KeyCode)50))
		{
			_boneAnimation.Play("Walk");
		}
	}
}
