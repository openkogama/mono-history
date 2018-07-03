using UnityEngine;

public abstract class AvatarAccessory : MonoBehaviour
{
	private string previewImageStreamPath;

	private Transform _transform;

	private Collider[] _colliders;

	private Renderer[] _renderers;

	private bool _visible = true;

	public abstract AccessorySettings AccessorySettings { get; }

	public string AssetPath { get; private set; }

	public float Offset { get; set; }

	public float Scale { get; set; }

	public string PreviewImageStreamPath
	{
		get
		{
			return previewImageStreamPath;
		}
		set
		{
			previewImageStreamPath = value;
		}
	}

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

	public virtual Vector3 AttachmentPointWorldPos => Vector3.zero;

	public virtual bool HasAttachmentPoint => false;

	protected virtual void Awake()
	{
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

	public virtual void InitAccessory(string assetReqPath, string bundleName, string previewImagePath)
	{
		AssetPath = assetReqPath;
		name = "Accessory " + AssetPath;
		PreviewImageStreamPath = previewImagePath;
	}
}
