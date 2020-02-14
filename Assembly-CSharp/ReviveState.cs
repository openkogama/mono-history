using System.Collections.Generic;
using UnityEngine;

public class ReviveState
{
	private List<SafeSpotData> safePositions = new List<SafeSpotData>();

	private float minDistanceBetweenSafePositions = 3f;

	private int maxNumberOfSafePositions = 3;

	private int currentPreviewedSafePosition;

	public bool CanSafelySpawn => safePositions.Count > 0;

	public SafeSpotData SafeGroundedData
	{
		get
		{
			if (safePositions.Count > 0)
			{
				return safePositions[currentPreviewedSafePosition];
			}
			return default;
		}
		set
		{
			if (!MVClientSettings.ReviveEnabled)
			{
				return;
			}
			if (safePositions.Count == 0)
			{
				safePositions.Add(value);
			}
			else if ((safePositions[safePositions.Count - 1].Position - value.Position).magnitude > minDistanceBetweenSafePositions)
			{
				if (safePositions.Count < maxNumberOfSafePositions)
				{
					safePositions.Add(value);
				}
				else if (safePositions.Count >= maxNumberOfSafePositions)
				{
					currentPreviewedSafePosition = Mathf.FloorToInt(maxNumberOfSafePositions / 2);
					safePositions.RemoveAt(0);
					safePositions.Add(value);
				}
			}
		}
	}

	public void ResetSafePostions()
	{
		currentPreviewedSafePosition = 0;
		safePositions.Clear();
	}

	public void SetSafeGroundedDataIndex(int index)
	{
		Debug.Log("Set safe spot grounded index: " + index + " / " + safePositions.Count);
		if (safePositions.Count > index)
		{
			currentPreviewedSafePosition = index;
		}
	}

	public SafeSpotData GetSafeGroundedDataAtSelectedIndex()
	{
		if (safePositions.Count > currentPreviewedSafePosition)
		{
			return safePositions[currentPreviewedSafePosition];
		}
		Debug.Log("GetSafeGroundedPosition: " + currentPreviewedSafePosition);
		Debug.LogError("Safe position non-existent.");
		return default;
	}

	public List<SafeSpotData> GetSafeGroundedPositions()
	{
		return safePositions;
	}
}
