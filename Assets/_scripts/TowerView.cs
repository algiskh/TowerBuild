using UnityEngine;

public class TowerView : MonoBehaviour
{
	[SerializeField] private RectTransform _rt;
	[SerializeField] private RectTransform _base;
	[SerializeField] private TowerCubeView _cubePrefab;
	public TowerCubeView CubePrefab => _cubePrefab;
	public RectTransform CubeParent => _base;
	public RectTransform RT => _rt;
}
