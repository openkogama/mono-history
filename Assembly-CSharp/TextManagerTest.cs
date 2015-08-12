using UnityEngine;

public class TextManagerTest : MonoBehaviour
{
	private void Start()
	{
		Debug.Log(Resources.Load("Languages/da_DK", typeof(TextAsset)));
		TM.LoadLanguage("da_DK");
	}
}
