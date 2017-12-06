using System.Collections.Generic;
using UnityEngine;

public class StreamedAudioClipList : MonoBehaviour
{
	[SerializeField]
	public List<StreamedAudioClipInfo> urls;

	public List<StreamedAudioClipInfo> URLS => urls;
}
