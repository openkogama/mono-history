using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OverrideMaterials : MonoBehaviour
{
	public List<OverrideMaterial> overrideMaterials = new List<OverrideMaterial>();

	public bool dumpToSql;

	public void Register()
	{
		foreach (OverrideMaterial overrideMaterial in overrideMaterials)
		{
			overrideMaterial.Register();
		}
	}

	private void Update()
	{
		if (!dumpToSql)
		{
			return;
		}
		Debug.Log("Dumping override materials to sql...");
		string text = string.Empty;
		foreach (OverrideMaterial overrideMaterial in overrideMaterials)
		{
			text += overrideMaterial;
		}
		using (StreamWriter streamWriter = new StreamWriter("Materials.txt", append: false))
		{
			streamWriter.WriteLine(text);
		}
		dumpToSql = false;
	}
}
