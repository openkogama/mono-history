public class UXSingleRowCollectionView : UXCollectionView
{
	public override void Initialize()
	{
		_rows = 1;
		base.Initialize();
		_allowMoveItems = false;
	}
}
