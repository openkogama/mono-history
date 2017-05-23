using MV.Common;
using UnityEngine;

public abstract class AvatarAccessory : MonoBehaviour
{
	private int _gameObjectID;

	private Transform _transform;

	private Collider[] _colliders;

	private Renderer[] _renderers;

	private bool _visible = true;

	private bool attached;

	private Quaternion worldRotationOnAttach;

	public abstract AccessorySettings AccessorySettings { get; }

	public AvatarAccessoryCategory Category { get; protected set; }

	public string AssetPath { get; private set; }

	public int InventoryID { get; private set; }

	public AvatarAccessorySlot Slot { get; set; }

	public float Offset { get; set; }

	public int GameObjectID => _gameObjectID;

	public Transform Transform
	{
		get
		{
			if (_transform == null)
			{
				_transform = transform;
			}
			return _transform;
		}
	}

	public Collider[] Colliders
	{
		get
		{
			if (_colliders == null)
			{
				_colliders = GetComponentsInChildren<Collider>();
			}
			return _colliders;
		}
	}

	public Renderer[] Renderers
	{
		get
		{
			if (_renderers == null)
			{
				_renderers = GetComponentsInChildren<Renderer>();
			}
			return _renderers;
		}
	}

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible != value)
			{
				Renderer[] renderers = Renderers;
				foreach (Renderer renderer in renderers)
				{
					renderer.enabled = value;
				}
				_visible = value;
			}
		}
	}

	public bool Attached
	{
		get
		{
			return attached;
		}
		set
		{
			if (attached != value)
			{
				attached = value;
				if (attached)
				{
					worldRotationOnAttach = Transform.rotation;
				}
			}
		}
	}

	public virtual Vector3 AttachmentPointWorldPos => Vector3.zero;

	public virtual bool HasAttachmentPoint => false;

	protected virtual void Awake()
	{
		_gameObjectID = gameObject.GetInstanceID();
		Collider[] colliders = Colliders;
		foreach (Collider collider in colliders)
		{
			collider.enabled = false;
		}
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = _visible;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		if (attached && AccessorySettings.ConstantWorldRotation)
		{
			Transform.rotation = worldRotationOnAttach;
		}
	}

	public override int GetHashCode()
	{
		return 17 * InventoryID;
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (o == this)
		{
			return true;
		}
		return false;
	}

	public virtual Bounds GetWorldBounds()
	{
		Bounds result = default;
		bool flag = true;
		Renderer[] renderers = Renderers;
		foreach (Renderer renderer in renderers)
		{
			if (flag)
			{
				result = renderer.bounds;
			}
			else
			{
				result.Encapsulate(renderer.bounds);
			}
		}
		return result;
	}

	public virtual Bounds GetLocalBounds()
	{
		Bounds worldBounds = GetWorldBounds();
		if (transform != null)
		{
			worldBounds.center -= transform.position;
		}
		return worldBounds;
	}

	public virtual void InitAccessory(AvatarAccessoryParams p, string bundleName)
	{
		InventoryID = p.InventoryID;
		AssetPath = p.AssetReqPath;
		name = "Accessory " + InventoryID + " " + bundleName;
	}
}
