using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public class MVMaterial
{
	public Mesh mesh;

	public string name;

	public string description;

	public PhysicalProperties physicalProperties;

	public MaterialSound materialSound;

	public AvatarModifierPackageType modifierPackageType;

	public int unlockPriceGold;

	public int unlockPriceSilver;

	public bool isUnlocked;

	private ObscuredString textureHashCode = string.Empty;

	public bool IsAvailable
	{
		get
		{
			if (MVMaterialRepository.AllowDestructibleMaterialSelection)
			{
				return true;
			}
			if (physicalProperties.toughness == 0f)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsDestructible
	{
		get
		{
			if (physicalProperties.toughness == 0f)
			{
				return false;
			}
			return true;
		}
	}

	public MVMaterial()
	{
	}

	public MVMaterial(int materialId, string name, string description, PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType, int priceGold, int priceSilver, bool isUnlocked)
		: this(physicalProperties, materialSound, modifierPackageType)
	{
		GenerateCube(materialId);
		unlockPriceGold = priceGold;
		unlockPriceSilver = priceSilver;
		this.isUnlocked = isUnlocked;
		this.name = name;
		this.description = description;
	}

	public MVMaterial(PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType)
	{
		this.physicalProperties = physicalProperties;
		this.materialSound = materialSound;
		this.modifierPackageType = modifierPackageType;
	}

	public void Validate()
	{
	}

	private void GenerateCube(int materialId)
	{
		mesh = new Mesh();
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
		mesh.vertices = MeshDataPool.GetVertices();
		mesh.uv = MeshDataPool.GetUvs();
		mesh.triangles = MeshDataPool.GetIndices();
		mesh.colors = MeshDataPool.GetColors();
		mesh.RecalculateNormals();
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
