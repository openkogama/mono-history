using System.Collections.Generic;
using MV.WorldObject;

public class DeltaCubes
{
	private Queue<KeyValuePair<IntVector, CubeAction>> cubeChange = new Queue<KeyValuePair<IntVector, CubeAction>>();

	public Queue<KeyValuePair<IntVector, CubeAction>> CubeChange => cubeChange;

	public int Count => cubeChange.Count;

	public DeltaCubes()
	{
	}

	public DeltaCubes(IEnumerable<KeyValuePair<IntVector, CubeAction>> cubeChangeOriginal)
	{
		foreach (KeyValuePair<IntVector, CubeAction> item in cubeChangeOriginal)
		{
			cubeChange.Enqueue(new KeyValuePair<IntVector, CubeAction>(item.Key, item.Value));
		}
	}

	public void Clear()
	{
		cubeChange.Clear();
	}

	public void Enqueue(IntVector iVector, CubeAction cubeAction)
	{
		cubeChange.Enqueue(new KeyValuePair<IntVector, CubeAction>(iVector, cubeAction));
	}

	public byte[] Dequeue(RuntimePrototypeCubeModel rpcm)
	{
		KeyValuePair<IntVector, CubeAction> keyValuePair = cubeChange.Dequeue();
		BytePacker bytePacker = new BytePacker();
		switch (keyValuePair.Value)
		{
		case CubeAction.Added:
		case CubeAction.FaceChanged:
		case CubeAction.CornersChangedDone:
		{
			bytePacker.Write((byte)keyValuePair.Value);
			byte[] byteCorners = rpcm.GetCube(keyValuePair.Key).ByteCorners;
			byte[] faceMaterials = rpcm.GetCube(keyValuePair.Key).FaceMaterials;
			CubeDataPacker.WriteCompressedCube(bytePacker, keyValuePair.Key.x, keyValuePair.Key.y, keyValuePair.Key.z, byteCorners, faceMaterials);
			return bytePacker.ToArray();
		}
		case CubeAction.Deleted:
			bytePacker.Write((byte)keyValuePair.Value);
			bytePacker.Write(keyValuePair.Key.x);
			bytePacker.Write(keyValuePair.Key.y);
			bytePacker.Write(keyValuePair.Key.z);
			return bytePacker.ToArray();
		default:
			return null;
		}
	}
}
