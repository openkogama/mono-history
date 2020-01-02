namespace Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.SpawnRoleVariableTypes;

public class SpawnRoleVariable<T>
{
	public delegate void SubDelegate(T value);

	protected readonly SubscribableVariable<T> subscribableVariable;

	public T Value => subscribableVariable.Value;

	public event SubDelegate OnChange;

	protected SpawnRoleVariable(T value)
	{
		subscribableVariable = new SubscribableVariable<T>(value);
		subscribableVariable.OnChange += SubscribableVariableOnOnChange;
	}

	public override bool Equals(object obj)
	{
		if (object.ReferenceEquals(null, obj))
		{
			return false;
		}
		if (object.ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((SpawnRoleVariable<T>)obj);
	}

	public override int GetHashCode()
	{
		return (subscribableVariable != null) ? subscribableVariable.GetHashCode() : 0;
	}

	private void SubscribableVariableOnOnChange(T value)
	{
		if (OnChange != null)
		{
			OnChange(value);
		}
	}

	public static implicit operator T(SpawnRoleVariable<T> s)
	{
		return s.subscribableVariable.Value;
	}

	public bool Equals(SpawnRoleVariable<T> other)
	{
		return subscribableVariable.Value.Equals(other.subscribableVariable.Value);
	}

	public static bool operator ==(SpawnRoleVariable<T> a, SpawnRoleVariable<T> b)
	{
		return a.subscribableVariable.Value.Equals(b.subscribableVariable.Value);
	}

	public static bool operator !=(SpawnRoleVariable<T> a, SpawnRoleVariable<T> b)
	{
		return !(a == b);
	}

	public static bool operator !=(SpawnRoleVariable<T> a, T b)
	{
		return !b.Equals(a.subscribableVariable.Value);
	}

	public static bool operator ==(SpawnRoleVariable<T> a, T b)
	{
		return b.Equals(a.subscribableVariable.Value);
	}

	public static bool operator !=(T b, SpawnRoleVariable<T> a)
	{
		return !b.Equals(a.subscribableVariable.Value);
	}

	public static bool operator ==(T b, SpawnRoleVariable<T> a)
	{
		return b.Equals(a.subscribableVariable.Value);
	}
}
