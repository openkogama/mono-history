using System;

public abstract class RepositoryCollection : IUXCollection
{
	protected ARepository repository;

	protected IUXCollectionItem[] cache = new IUXCollectionItem[0];

	protected int[] allowedItemCategories;

	public OnCollectionChangeDelegate OnCollectionChange { get; set; }

	public int Count => CountItems();

	public RepositoryCollection(ARepository repository, int[] allowedItemCategories)
	{
		this.repository = repository;
		ARepository aRepository = this.repository;
		aRepository.OnRepositoryChange = (ARepository.OnRepositoryChangeDelegate)Delegate.Combine(aRepository.OnRepositoryChange, new ARepository.OnRepositoryChangeDelegate(HandleRepositoryChange));
		this.allowedItemCategories = allowedItemCategories;
		HandleRepositoryChange(repository);
	}

	public IUXCollectionItem GetItem(int index)
	{
		return cache[index];
	}

	public abstract int GetSlotIndexFromItemId(int itemId);

	protected abstract int CountItems();

	protected abstract void HandleRepositoryChange(ARepository repository);

	protected void NotifyOnCollectionChange()
	{
		if (OnCollectionChange != null)
		{
			OnCollectionChange();
		}
	}
}
