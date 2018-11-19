using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVMaterial
{
	public int unlockPriceGold;

	public bool isUnlocked;

	public Mesh Mesh { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public PhysicalProperties PhysicalProperties { get; private set; }

	public AvatarModifierPackageType ModifierPackageType { get; private set; }

	public Texture2D ButtonTexture { get; private set; }

	public bool IsAvailable => MVMaterialRepository.AllowDestructibleMaterialSelection || PhysicalProperties.toughness == 0f;

	public bool IsDestructible => PhysicalProperties.toughness != 0f;

	public MVMaterial()
	{
	}

	public MVMaterial(int materialId, string name, string description, PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType, int priceGold, bool isUnlocked, MaterialButtonTextureGenerator materialButtonTextureGenerator)
		: this(physicalProperties, materialSound, modifierPackageType)
	{
		if (materialId == 60)
		{
			materialId = 24;
		}
		GenerateCube(materialId);
		if (MVGameControllerBase.GameMode != MVGameMode.Play && materialButtonTextureGenerator != null)
		{
			ButtonTexture = materialButtonTextureGenerator.TakePicture(Mesh);
		}
		unlockPriceGold = priceGold;
		this.isUnlocked = isUnlocked;
		Name = name;
		Description = description;
	}

	public MVMaterial(PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType)
	{
		PhysicalProperties = physicalProperties;
		ModifierPackageType = modifierPackageType;
	}

	private void GenerateCube(int materialId)
	{
		Mesh = new Mesh();
		MeshDataPool.Reset();
		Rect rect = TextureAtlas.UV[materialId];
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			num += 4;
			AddVertices(i);
			MeshDataPool.AddIndex(num - 4);
			MeshDataPool.AddIndex(num - 3);
			MeshDataPool.AddIndex(num - 2);
			MeshDataPool.AddIndex(num - 3);
			MeshDataPool.AddIndex(num - 1);
			MeshDataPool.AddIndex(num - 2);
			MeshDataPool.AddUv(new Vector2(0f, 0f));
			MeshDataPool.AddUv(new Vector2(1f, 0f));
			MeshDataPool.AddUv(new Vector2(0f, 1f));
			MeshDataPool.AddUv(new Vector2(1f, 1f));
			for (int j = 0; j < 4; j++)
			{
				MeshDataPool.AddColor(new Color(1f, rect.x, rect.y));
			}
		}
		Mesh.vertices = MeshDataPool.GetVertices();
		Mesh.uv = MeshDataPool.GetUvs();
		Mesh.triangles = MeshDataPool.GetIndices();
		Mesh.colors = MeshDataPool.GetColors();
		Mesh.RecalculateNormals();
	}

	private void AddVertices(int direction)
	{
		switch (direction)
		{
		case 0:
			MeshDataPool.AddVertex(new Vector3(-0.5f, 0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, 0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, -0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, -0.5f, -0.5f));
			break;
		case 1:
			MeshDataPool.AddVertex(new Vector3(0.5f, 0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, 0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, -0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, -0.5f, 0.5f));
			break;
		case 2:
			MeshDataPool.AddVertex(new Vector3(-0.5f, 0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, 0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, -0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, -0.5f, -0.5f));
			break;
		case 3:
			MeshDataPool.AddVertex(new Vector3(0.5f, 0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, 0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, -0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, -0.5f, 0.5f));
			break;
		case 4:
			MeshDataPool.AddVertex(new Vector3(-0.5f, 0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, 0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, 0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, 0.5f, -0.5f));
			break;
		case 5:
			MeshDataPool.AddVertex(new Vector3(0.5f, -0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, -0.5f, 0.5f));
			MeshDataPool.AddVertex(new Vector3(0.5f, -0.5f, -0.5f));
			MeshDataPool.AddVertex(new Vector3(-0.5f, -0.5f, -0.5f));
			break;
		}
	}
}
