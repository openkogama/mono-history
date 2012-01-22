namespace MV.Common;

public enum DBReasonCode
{
	None,
	NotConnected,
	Exception,
	RedundantName,
	WrongPassword,
	NotFound,
	LargeQueryNoKey,
	ExistingPendingRequest,
	AsyncFailed,
	NoDataFound,
	InsufficientAuthorization,
	FriendPending,
	FriendAlreadyAccepted,
	FriendProfileRejected,
	FriendOtherPending,
	FriendOtherProfileRejected,
	PurchaserIsOwner,
	InsufficientFunds,
	RedundantRow
}
