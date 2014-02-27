using UnityEngine;

public class MVGUIRoot : MonoBehaviour
{
	[SerializeField]
	private bool showRegisterMenuForTourist;

	public bool ShowRegisterMenuForTourist => showRegisterMenuForTourist;

	public void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)((Component)this).transform).gameObject);
	}
}
