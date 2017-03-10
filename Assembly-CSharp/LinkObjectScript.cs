using MV.WorldObject;
using UnityEngine;

public class LinkObjectScript : LinkObjectBase
{
	[SerializeField]
	private BoxCollider boxCollider;

	[SerializeField]
	private LineRenderer lineRenderer;

	private void Awake()
	{
		enabled = false;
	}

	public void Initialize(Link link)
	{
		isObjectLink = false;
		linkID = link.id;
		UpdateLinkVisual(link);
	}

	private bool UpdatePositions(Link link)
	{
		Vector3 outputConnectorPos = MVGameControllerBase.WOCM.GetWorldObjectClient(link.outputWOID).GetOutputConnectorPos();
		Vector3 inputConnectorPos = MVGameControllerBase.WOCM.GetWorldObjectClient(link.inputWOID).GetInputConnectorPos();
		return UpdatePositions(outputConnectorPos, inputConnectorPos);
	}

	public void UpdateLinkVisual(Link link)
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
		if (lineRenderer.isVisible)
		{
			if (link.isSet)
			{
				lineRenderer.material.color = Color.green;
				Vector2 mainTextureOffset = boxCollider.gameObject.GetComponent<Renderer>().material.mainTextureOffset;
				mainTextureOffset.x -= Time.deltaTime * 1.5f;
				lineRenderer.material.mainTextureOffset = mainTextureOffset;
			}
			else
			{
				lineRenderer.material.color = Color.grey;
			}
		}
	}
}
