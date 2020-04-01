using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EmbeddedSiteConfigData
{
	[SerializeField]
	public List<string> sites;

	[SerializeField]
	public EmbeddedSite siteEnum;

	[SerializeField]
	public bool showTouristPromotion;

	[SerializeField]
	public bool allowsOpenInNewTab;

	[SerializeField]
	public bool allowsRedirectToWebpage;

	[SerializeField]
	public bool allowsModals;

	[SerializeField]
	public bool integratedSdk;

	[SerializeField]
	public bool allowsFallbackAds;

	[SerializeField]
	public bool showPlayButtonAd;
}
