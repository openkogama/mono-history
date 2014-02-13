using UnityEngine;

[ExecuteInEditMode]
public class WaterTile : MonoBehaviour
{
	public PlanarReflection reflection;

	public WaterBase waterBase;

	public void Start()
	{
		AcquireComponents();
	}

	private void AcquireComponents()
	{
		if (!Object.op_Implicit((Object)(object)reflection))
		{
			if (Object.op_Implicit((Object)(object)((Component)this).transform.parent))
			{
				reflection = ((Component)((Component)this).transform.parent).GetComponent<PlanarReflection>();
			}
			else
			{
				reflection = ((Component)((Component)this).transform).GetComponent<PlanarReflection>();
			}
		}
		if (!Object.op_Implicit((Object)(object)waterBase))
		{
			if (Object.op_Implicit((Object)(object)((Component)this).transform.parent))
			{
				waterBase = ((Component)((Component)this).transform.parent).GetComponent<WaterBase>();
			}
			else
			{
				waterBase = ((Component)((Component)this).transform).GetComponent<WaterBase>();
			}
		}
	}

	public void OnWillRenderObject()
	{
		if (Object.op_Implicit((Object)(object)reflection))
		{
			reflection.WaterTileBeingRendered(((Component)this).transform, Camera.current);
		}
		if (Object.op_Implicit((Object)(object)waterBase))
		{
			waterBase.WaterTileBeingRendered(((Component)this).transform, Camera.current);
		}
	}
}
