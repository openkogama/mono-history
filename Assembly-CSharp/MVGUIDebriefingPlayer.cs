using UnityEngine;

public class MVGUIDebriefingPlayer : MVGUIDebriefing
{
	[SerializeField]
	private Transform avatarAttachPoint;

	private float maxLocalScale = 6f;

	private MeshRenderer[] meshRenderers;

	[SerializeField]
	protected UXText winnerName;

	[SerializeField]
	protected UXText winnerValue;

	public void SetWinValue(string winValue)
	{
		winnerValue.Text = winValue;
	}

	public void Init(int id)
	{
		if (MVGameControllerBase.Game.Players.TryGetValue(id, out var value))
		{
			GameObject gameObject = value.Avatar.Body.CopyByValue();
			meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
			SetupBody(gameObject);
			SetName(value.Username);
			SetupSecondaryCamera();
		}
		else
		{
			Debug.LogWarning("Winning player can't be found. Probably left game session");
		}
	}

	public override void SetFadeInFadeOut(float scale)
	{
		scale = Mathf.Max(scale, 0.01f);
		avatarAttachPoint.localScale = Vector3.one * maxLocalScale * scale;
	}

	public void SetAlpha(float alpha)
	{
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			Material[] materials = meshRenderer.materials;
			foreach (Material material in materials)
			{
				SetAlphaForMaterial(alpha, material);
			}
		}
	}

	private void SetupSecondaryCamera()
	{
		UXCamera uXCamera = UXUtils.UXCamera;
		uXCamera.SecondaryUXCamera.gameObject.SetActive(value: true);
	}

	private void OnDestroy()
	{
		UXCamera uXCamera = UXUtils.UXCamera;
		if (!(uXCamera == null) && !(uXCamera.SecondaryUXCamera == null))
		{
			uXCamera.SecondaryUXCamera.gameObject.SetActive(value: false);
		}
	}

	private void SetName(string userName)
	{
		winnerName.Text = userName;
	}

	private void SetupBody(GameObject bodyCloneGO)
	{
		LayerUtil.SetLayerRecursively(bodyCloneGO.transform, "Default", "UXElementSecondary");
		LayerUtil.SetLayerRecursively(bodyCloneGO.transform, "Player", "UXElementSecondary");
		LayerUtil.SetLayerRecursively(bodyCloneGO.transform, -1048577, LayerUtil.GetLayerNumber(LayerFlags.Hidden));
		BoneAnimation componentInChildren = bodyCloneGO.GetComponentInChildren<BoneAnimation>();
		componentInChildren.Play("Idle");
		bodyCloneGO.transform.parent = avatarAttachPoint;
		bodyCloneGO.transform.localPosition = Vector3.zero;
		bodyCloneGO.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		SetFadeInFadeOut(0f);
		SetAlpha(1f);
	}

	private void SetAlphaForMaterial(float alpha, Material material)
	{
		if (material.HasProperty("_Color"))
		{
			Color color = material.color;
			color.a = alpha;
			material.color = color;
		}
	}
}
