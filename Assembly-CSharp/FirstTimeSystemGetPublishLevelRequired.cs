using UnityEngine;
using UnityEngine.UI;

public class FirstTimeSystemGetPublishLevelRequired : MonoBehaviour
{
	[SerializeField]
	private Text levelRequired;

	private void Start()
	{
		levelRequired.text = string.Format(levelRequired.text, MVGameControllerBase.Game.PublishLevel);
	}
}
