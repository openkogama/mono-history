public interface ILocalObject
{
	int Id { get; }

	MovementMap Update(MovementMap movementMap);

	MovementMap FixedUpdate(MovementMap movementMap);
}
