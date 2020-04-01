using UnityEngine;

public class KogamaLogHandlerTest : MonoBehaviour
{
	private ProxyLogHandler kogamaLogHandler;

	[SerializeField]
	private string s;

	[SerializeField]
	private LogType logType;

	private void Start()
	{
		kogamaLogHandler = new ProxyLogHandler();
		kogamaLogHandler.OnLogReceived += KogamaLogHandlerOnOnLogReceived;
		Debug.LogError("test");
	}

	private void KogamaLogHandlerOnOnLogReceived(object sender, ProxyLogHandler.LogFormatData e)
	{
		s = e.Message;
		logType = e.LogType;
	}

	private void Update()
	{
	}
}
