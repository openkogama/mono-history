using UnityEngine;

public class MVSoundEmitter : MVLogicObject
{
	public static string GetSoundCueFileName(SoundCue soundCue)
	{
		string text = "Audio/";
		return soundCue switch
		{
			SoundCue.Default => text + "Gui/Kgm_gui_squeak14(loop)", 
			SoundCue.Forest => text + "Ambience/Kgm_amb_Forest", 
			SoundCue.Jungle => text + "Ambience/Kgm_amb_Jungle", 
			SoundCue.OceanSurf => text + "Ambience/Kgm_amb_OceanSurf", 
			_ => "UNDEFINED_AUDIO_CLIP", 
		};
	}

	protected override void CreateMVWOC(bool local)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		interactionFlags = InteractionFlags.Selectable;
		gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/SoundEmitterObject"), Vector3.zero, Quaternion.identity);
		((Object)gameObject).name = GetType().ToString();
		gameObject.layer = LayerMask.NameToLayer("Logic");
		OnDataUpdate();
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void OnDataUpdate()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		GameObject.audio.Stop();
		SoundCue soundCue = (SoundCue)(int)Data["soundCue"];
		GameObject.audio.clip = (AudioClip)Resources.Load(GetSoundCueFileName(soundCue));
		GameObject.audio.loop = !(bool)Data["once"];
		GameObject.audio.volume = (float)Data["volume"];
		GameObject.audio.maxDistance = (float)Data["range"];
		if ((bool)Data["active"])
		{
			GameObject.audio.Play();
		}
	}
}
