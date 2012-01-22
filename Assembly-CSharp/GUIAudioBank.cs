public static class GUIAudioBank
{
	private static AudioBank instance;

	public static AudioBank Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AudioBank("Audio/GUI/");
			}
			return instance;
		}
	}
}
