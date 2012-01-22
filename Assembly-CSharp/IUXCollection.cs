public interface IUXCollection
{
	int Count { get; }

	OnCollectionChangeDelegate OnCollectionChange { get; set; }

	IUXCollectionItem GetItem(int index);
}
