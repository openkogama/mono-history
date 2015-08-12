using UnityEngine;

public abstract class MVGUIDebriefing : MonoBehaviour
{
	[SerializeField]
	protected UXPlane winnerConditionIcon;

	public void SetWinnerConditionIconMaterial(Material winnerConditionIconMaterial)
	{
		winnerConditionIcon.GetComponent<Renderer>().material = winnerConditionIconMaterial;
	}

	public virtual void SetFadeInFadeOut(float alpha)
	{
	}
}
