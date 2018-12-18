using System;
using Borodar.FarlandSkies.CloudyCrownPro.DotParams;
using UnityEngine;

public class DayNightCycleColorPresets : ScriptableObject
{
	[Serializable]
	public class Preset
	{
		[SerializeField]
		private string name;

		[SerializeField]
		private SkyParamsList skyParamList;

		[SerializeField]
		private CelestialParamsList sunParamsList;

		[SerializeField]
		private CelestialParamsList moonParamsList;

		[SerializeField]
		private StarsParamsList starsParamList;

		private bool initialized;

		public string Name => name;

		public SkyParamsList Sky => skyParamList;

		public CelestialParamsList Sun => sunParamsList;

		public CelestialParamsList Moon => moonParamsList;

		public StarsParamsList Stars => starsParamList;

		public bool IsInitialized => initialized;

		public void Initialize()
		{
			skyParamList.Init();
			sunParamsList.Init();
			moonParamsList.Init();
			starsParamList.Init();
			initialized = true;
		}

		public void TranslateName()
		{
			name = TM._(name);
			TM.LanguageChanged(LanguageLoadedCallback);
		}

		public void Cleanup()
		{
			initialized = false;
		}

		private void LanguageLoadedCallback()
		{
			name = TM._(name);
		}
	}

	[SerializeField]
	private Preset[] presets;

	public int Length => presets.Length;

	public Preset this[int i]
	{
		get
		{
			Preset preset = presets[i];
			if (!preset.IsInitialized)
			{
				preset.Initialize();
			}
			return preset;
		}
	}

	protected void OnEnable()
	{
		TranslatePresetNames();
	}

	protected void OnDisable()
	{
		Preset[] array = presets;
		foreach (Preset preset in array)
		{
			preset.Cleanup();
		}
	}

	protected void TranslatePresetNames()
	{
		Preset[] array = presets;
		foreach (Preset preset in array)
		{
			preset.TranslateName();
		}
	}
}
