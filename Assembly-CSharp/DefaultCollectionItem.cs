public class DefaultCollectionItem : IUXCollectionItem
{
	private int index;

	private object obj;

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public object Object
	{
		get
		{
			return obj;
		}
		set
		{
			obj = value;
		}
	}

	public DefaultCollectionItem(int index, object obj)
	{
		this.index = index;
		this.obj = obj;
	}
}
