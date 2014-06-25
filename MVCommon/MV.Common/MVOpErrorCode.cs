namespace MV.Common;

public class MVOpErrorCode
{
	public const int UndefinedFail = -1;

	public const int JoinPlanetNotFound = -2;

	public const int JoinNotAuthorized = -3;

	public const int JoinGameDisposed = -4;

	public const int JoinProfileAlreadyJoined = -5;

	public const int JoinDeserializationFailed = -6;

	public const int JoinOperationValidationFailed = -7;

	public const int JoinProfileNotFound = -8;

	public const int JoinTouristNotAllowedInMode = -9;

	public const int JoinGameInitializationFailed = -10;

	public const int PublishNotAuthorized = -2;

	public const int InventoryPrototypeNotFound = -2;

	public const int InventoryNotAuthorized = -3;

	public const int InventoryCreateItemFailed = -4;

	public const int InventoryAddItemFailed = -5;

	public const int InventoryNotAuthorizedButForSale = -6;

	public const int InventoryAlreadyInInventory = -7;

	public const int InventoryKogamaPackageCreationFailed = -8;

	public const int FriendNotFound = -2;

	public const int FriendPendingRequestExists = -3;

	public const int FriendAlreadyAccepted = -4;

	public const int FriendProfileRejected = -5;

	public const int FriendOtherPendingRequestExists = -6;

	public const int FriendOtherProfileRejected = -7;

	public const int UngroupFailed = -2;

	public const int PurchaseItemInsufficientFunds = -2;

	public const int PurchaseItemPurchaserIsOwner = -3;

	public const int PurchaseItemNotFound = -4;

	public const int PurchaseItemAlreadyInInventory = -5;
}
