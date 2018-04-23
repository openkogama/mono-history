using UnityEngine;

public class PersistentUI : MonoBehaviour
{
	[SerializeField]
	private RectTransform transformToInstantiateWinningConditionIn;

	public RectTransform TransformToInstantiateWinningConditionIn => transformToInstantiateWinningConditionIn;
}
