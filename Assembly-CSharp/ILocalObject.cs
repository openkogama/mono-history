public interface ILocalObject
{
	int Id { get; }

	InteractionInput Update(InteractionInput movementMap);

	MovementMap FixedUpdate(MovementMap movementMap);
}
