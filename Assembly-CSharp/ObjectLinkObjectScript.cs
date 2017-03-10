using MV.WorldObject;
using UnityEngine;

public class ObjectLinkObjectScript : LinkObjectBase
{
	[SerializeField]
	private BoxCollider boxCollider;

	[SerializeField]
	private LineRenderer lineRenderer;

	private void Awake()
	{
		enabled = false;
	}

	public void Initialize(ObjectLink link)
	{
		isObjectLink = true;
		linkID = link.id;
		UpdateLinkVisual(link);
		lineRenderer.material.color = Color.yellow;
	}

	private bool UpdatePositions(ObjectLink link)
	{
		Vector3 objectConnectorPos = MVGameControllerBase.WOCM.GetWorldObjectClient(link.objectConnectorWOID).GetObjectConnectorPos();
		Vector3 worldCenterPos = (MVGameControllerBase.WOCM.GetWorldObjectClient(link.objectWOID) as MVCubeModelBase).GetWorldCenterPos();
		return UpdatePositions(objectConnectorPos, worldCenterPos);
	}

	public void UpdateLinkVisual(ObjectLink link)
	{
		if (UpdatePositions(link))
		{
			lineRenderer.SetPosition(0, startPos);
			lineRenderer.SetPosition(1, endPos);
			boxCollider.gameObject.transform.position = startPos + (endPos - startPos) / 2f;
			boxCollider.gameObject.transform.rotation = Quaternion.LookRotation((endPos - startPos).normalized);
			float magnitude = (endPos - startPos).magnitude;
			magnitude -= 0.5f;
			magnitude = Mathf.Max(magnitude, 0.2f);
			boxCollider.transform.localScale = new Vector3(0.2f, 0.2f, magnitude);
			lineRenderer.material.mainTextureScale = new Vector2((endPos - startPos).magnitude / 2f, 1f);
		}
	}
}
