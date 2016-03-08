using System.Collections.Generic;
using UnityEngine;

public class RaycastHitComparer : Comparer<RaycastHit>
{
	public override int Compare(RaycastHit a, RaycastHit b)
	{
		return a.distance.CompareTo(b.distance);
	}
}
