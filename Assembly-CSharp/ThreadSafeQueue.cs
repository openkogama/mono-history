using System.Collections.Generic;

public class ThreadSafeQueue<T>
{
	private Queue<T> _queue;

	private object _lock = new object();

	private Queue<T> Queue
	{
		get
		{
			lock (_lock)
			{
				return _queue;
			}
		}
	}

	public int Count => _queue.Count;

	public ThreadSafeQueue(int initialCap)
	{
		_queue = new Queue<T>(initialCap);
	}

	public void Clear()
	{
		lock (_lock)
		{
			_queue.Clear();
		}
	}

	public T Dequeue()
	{
		lock (_lock)
		{
			return _queue.Dequeue();
		}
	}

	public void Enqueue(T a)
	{
		lock (_lock)
		{
			_queue.Enqueue(a);
		}
	}

	public T Peek()
	{
		lock (_lock)
		{
			return _queue.Peek();
		}
	}

	public object ObtainLockForMultiOps()
	{
		return _lock;
	}
}
