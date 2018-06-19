using UnityEngine;

public class ThemeSpawner : MonoBehaviour
{
	protected void Start()
	{
		Theme theme = Object.Instantiate(ThemeRepository.Instance.GetThemePrefab("Animals"));
		theme.InitializeForPreview();
		theme.Activate();
	}
}
