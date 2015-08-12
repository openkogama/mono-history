using System.Collections.Generic;
using UnityEngine;

public class MVTeleporter : MVLogicObject
{
	private const string prefabPath = "Prefabs/TelePorterObject";

	private TriggerBoxEvents triggerBoxEvents;

	private List<MVAvatar> avatarIgnoreList = new List<MVAvatar>();

	private TeleportAvatar teleportAvatarPrefab;

	private ParticleSystem teleportParticles;

	private MVTeleporter target;

	private GameCoinLogic gameCoinLogic;

	private Vector3 gameCoinDisplayObjectOffset = new Vector3(0f, 1.5f, 0f);

	private bool isDestroyed;

	public override Vector3 InputConnectorOffset => new Vector3(-1.5f, 0f, 0f);

	public override Vector3 ObjectConnectorOffset => new Vector3(0f, 1.5f, 0f);

	public override Quaternion ObjectConnectorRotation => Quaternion.LookRotation(Vector3.down);

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override bool HasObjectConnector => false;

	public MVTeleporter Target
	{
		set
		{
			target = value;
		}
	}

	public MVTeleporter(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/TelePorterObject", worldObjects)
	{
		teleportAvatarPrefab = Resources.Load("Prefabs/Logic/TeleportAvatar", typeof(TeleportAvatar)) as TeleportAvatar;
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += triggerBoxEvents_TriggerExit;
		teleportParticles = gameObject.GetComponentInChildren<ParticleSystem>();
		interactionFlags &= ~InteractionFlags.CanClone;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		gameCoinLogic = new GameCoinLogic(gameObject, Data, gameCoinDisplayObjectOffset);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		gameCoinLogic.OnDataUpdate(Data);
	}

	public override void Initialize()
	{
		base.Initialize();
		OnInputLinkChanged();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (gameCoinLogic.PurchaseAmount > 0 && triggerBoxEvents.IsInTrigger && !avatarIgnoreList.Contains(MVGameController.WOCM.AvatarLocal) && gameCoinLogic.ShowUseGUI())
		{
			DoTeleport(MVGameController.WOCM.AvatarLocal.Id);
		}
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		return worldObjectClientManager.GetWorldObjectClient(groupId)?.Delete(worldObjectClientManager, ref errorText) ?? false;
	}

	public override bool ValidateObjectLinkTarget(MVWorldObjectClient wo)
	{
		return wo is MVTeleporter;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 vector = new Vector3(Mathf.Round(position.x / gridSize), Mathf.Round(position.y / gridSize), Mathf.Round(position.z / gridSize));
		return vector * gridSize;
	}

	public override void OnInputLinkChanged()
	{
		base.OnInputLinkChanged();
		OnInputStateChanged();
	}

	public override void OnInputStateChanged()
	{
		base.OnInputStateChanged();
		bool flag = InputLinkRefs.Count == 0 || InputState;
		teleportParticles.startColor = ((!flag) ? new Color(1f, 0.5f, 0f) : new Color(0f, 0.5f, 1f));
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if ((InputLinkRefs.Count == 0 || InputState) && gameCoinLogic.PurchaseAmount <= 0)
		{
			DoTeleport(e.instigatorWOID);
		}
	}

	private void DoTeleport(int instigatorWOID)
	{
		MVTeleporter mVTeleporter = target;
		if (!(MVGameController.WOCM.GetWorldObjectClient(instigatorWOID) is MVAvatarLocal))
		{
			return;
		}
		MVAvatarLocal mVAvatarLocal = MVGameController.WOCM.GetWorldObjectClient(instigatorWOID) as MVAvatarLocal;
		if (!mVAvatarLocal.IsEnteringVehicle)
		{
			if (mVAvatarLocal.IsSeated)
			{
				mVAvatarLocal.LeaveVehicle();
			}
			if (!avatarIgnoreList.Contains(mVAvatarLocal))
			{
				TeleportAvatar teleportAvatar = Object.Instantiate(teleportAvatarPrefab, transform.position, Quaternion.identity) as TeleportAvatar;
				teleportAvatar.avatar = mVAvatarLocal;
				teleportAvatar.targetPosition = target.WorldPosition;
				teleportAvatar.originPosition = transform.position;
				mVTeleporter.avatarIgnoreList.Add(mVAvatarLocal);
			}
		}
	}

	private void triggerBoxEvents_TriggerExit(object sender, TriggerEventArgs e)
	{
		if (MVGameController.WOCM.GetWorldObjectClient(e.instigatorWOID) is MVAvatarLocal)
		{
			MVAvatarLocal avatarLocal = MVGameController.WOCM.AvatarLocal;
			if (e.instigatorWOID == avatarLocal.Id)
			{
				avatarIgnoreList.Remove(avatarLocal);
			}
		}
	}

	public override void Destroy()
	{
		if (!isDestroyed)
		{
			if (gameCoinLogic != null)
			{
				gameCoinLogic.OnDestroy(Data);
			}
			base.Destroy();
			isDestroyed = true;
		}
	}
}
