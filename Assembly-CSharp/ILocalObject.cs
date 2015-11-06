public interface ILocalObject
{
	int Id { get; }

	InputToInGameAction Update(InputToInGameAction movementMap);

	IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap);
}
