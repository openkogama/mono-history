using UnityEngine;

public abstract class ShowUse : MonoBehaviour
{
	public abstract void CalculateUseGraphics(ShowUseOption useOptions, int woID = 0);

	public abstract void Hide();

	public abstract void Show();
}
