public class Tuple<T1, T2>
{
	public readonly T1 A;

	public readonly T2 B;

	public Tuple(T1 a, T2 b)
	{
		A = a;
		B = b;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if ((object)GetType() != obj.GetType())
		{
			return false;
		}
		Tuple<T1, T2> tuple = obj as Tuple<T1, T2>;
		return A.Equals(tuple.A) && B.Equals(tuple.B);
	}

	public override int GetHashCode()
	{
		return A.GetHashCode() + B.GetHashCode();
	}
}
