using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MV.Common.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)(object)SettingsBase.Synchronized((SettingsBase)(object)new Settings());

	public static Settings Default => defaultInstance;

	[DefaultSettingValue("mvuserdbdev.c86uleowzacr.eu-west-1.rds.amazonaws.com")]
	[DebuggerNonUserCode]
	[UserScopedSetting]
	public string DBUrl
	{
		get
		{
			return (string)((SettingsBase)this)["DBUrl"];
		}
		set
		{
			((SettingsBase)this)["DBUrl"] = value;
		}
	}

	[UserScopedSetting]
	[DefaultSettingValue("MVUserDB")]
	[DebuggerNonUserCode]
	public string DBName
	{
		get
		{
			return (string)((SettingsBase)this)["DBName"];
		}
		set
		{
			((SettingsBase)this)["DBName"] = value;
		}
	}

	[DefaultSettingValue("root")]
	[DebuggerNonUserCode]
	[UserScopedSetting]
	public string DBUser
	{
		get
		{
			return (string)((SettingsBase)this)["DBUser"];
		}
		set
		{
			((SettingsBase)this)["DBUser"] = value;
		}
	}

	[DebuggerNonUserCode]
	[UserScopedSetting]
	[DefaultSettingValue("Am00c00ma")]
	public string DBPassword
	{
		get
		{
			return (string)((SettingsBase)this)["DBPassword"];
		}
		set
		{
			((SettingsBase)this)["DBPassword"] = value;
		}
	}
}
