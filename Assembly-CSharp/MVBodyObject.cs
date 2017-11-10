using UnityEngine;

public class MVBodyObject : ObjectPrefab
{
	[SerializeField]
	private BoneAnimation boneAnimation;

	[SerializeField]
	private AvatarBlobShadowController avatarBlobShadowController;

	[SerializeField]
	private AvatarBlinker avatarBlinker;

	[SerializeField]
	private BodyData bodyData;

	public BoneAnimation BoneAnimation => boneAnimation;

	public AvatarBlobShadowController AvatarBlobShadowController => avatarBlobShadowController;

	public AvatarBlinker AvatarBlinker => avatarBlinker;

	public BodyData BodyData => bodyData;
}
