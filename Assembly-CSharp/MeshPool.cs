public sealed class MeshPool
{
	private static readonly MeshPool instance = new MeshPool();

	private MeshPooledObject[] freeMeshes;

	private MeshPooledObject[] usedMeshes;

	private int freeMeshesCount;

	private int usedMeshesCount;

	public static MeshPool Instance => instance;

	public bool GotFreeMesh => FreeMeshes > 0;

	public int MaxAmtMeshes
	{
		get
		{
			return FreeMeshes + UsedMeshes;
		}
		set
		{
			SetPoolAmount(value);
		}
	}

	public int FreeMeshes => freeMeshesCount;

	public int UsedMeshes => usedMeshesCount;

	public MeshPooledObject GetNewMesh => usedMeshes[SendMeshToUsedMeshes()];

	private MeshPool()
	{
	}

	public void ReturnMesh(MeshPooledObject mesh)
	{
		freeMeshes[FreeMeshes] = mesh;
		usedMeshesCount--;
		freeMeshesCount++;
	}

	private int SendMeshToUsedMeshes()
	{
		int num = FreeMeshes - 1;
		ref MeshPooledObject reference = ref usedMeshes[freeMeshes[num].Id];
		reference = freeMeshes[num];
		freeMeshesCount--;
		usedMeshesCount++;
		return freeMeshes[num].Id;
	}

	private void SetPoolAmount(int size)
	{
		int num = size - MaxAmtMeshes;
		if (num > 0)
		{
			freeMeshes = new MeshPooledObject[size];
			usedMeshes = new MeshPooledObject[size];
		}
		for (int i = 0; i < num; i++)
		{
			freeMeshes[i] = default;
			freeMeshes[i].Init(i);
		}
		freeMeshesCount = size;
		usedMeshesCount = 0;
	}
}
