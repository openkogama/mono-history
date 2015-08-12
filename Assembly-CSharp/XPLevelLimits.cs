using System;
using UnityEngine;

public class XPLevelLimits
{
	public int PrevXP { get; set; }

	public int NextXP { get; set; }

	public int Level { get; set; }

	public int XPNextRel => NextXP - PrevXP;

	public XPLevelLimits()
	{
	}

	public XPLevelLimits(int prevXp, int nextXp, int level)
	{
		PrevXP = prevXp;
		NextXP = nextXp;
		Level = level;
	}

	public int XpRel(int currentXp)
	{
		return currentXp - PrevXP;
	}

	public bool Validate(int currentXp)
	{
		if (PrevXP >= NextXP)
		{
			throw new Exception("prevXp>= nextXp");
		}
		if (currentXp < PrevXP)
		{
			throw new Exception("currentXp < prevXp");
		}
		if (currentXp >= NextXP)
		{
			Debug.LogWarning("Limits out of date");
			return false;
		}
		if (currentXp < 0)
		{
			throw new Exception("currentXp< 0");
		}
		return true;
	}

	public override string ToString()
	{
		return $"Level {Level}. PrevXP {PrevXP}. NextXP {NextXP}.";
	}
}
