using System;
using System.Collections.Generic;

public abstract class SubscribableVariableBase<T>
{
	protected T value;

	public T Value => value;

	public event Action<T> OnChange;

	protected SubscribableVariableBase(T value)
	{
		this.value = value;
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
		return Equals((SubscribableVariableBase<T>)obj);
	}

	public override int GetHashCode()
	{
		return EqualityComparer<T>.Default.GetHashCode(value);
	}

	public static implicit operator T(SubscribableVariableBase<T> s)
	{
		return s.value;
	}

	public bool Equals(SubscribableVariableBase<T> other)
	{
		ref T reference = ref value;
		object obj = other.value;
		return reference.Equals(obj);
	}

	public static bool operator ==(SubscribableVariableBase<T> a, SubscribableVariableBase<T> b)
	{
		ref T reference = ref a.value;
		object obj = b.value;
		return reference.Equals(obj);
	}

	public static bool operator !=(SubscribableVariableBase<T> a, SubscribableVariableBase<T> b)
	{
		return !(a == b);
	}

	public static bool operator !=(SubscribableVariableBase<T> a, T b)
	{
		return !b.Equals(a.value);
	}

	public static bool operator ==(SubscribableVariableBase<T> a, T b)
	{
		return b.Equals(a.value);
	}

	public static bool operator !=(T b, SubscribableVariableBase<T> a)
	{
		return !b.Equals(a.value);
	}

	public static bool operator ==(T b, SubscribableVariableBase<T> a)
	{
		return b.Equals(a.value);
	}

	protected void Notify()
	{
		if (OnChange != null)
		{
			OnChange(value);
		}
	}
}
