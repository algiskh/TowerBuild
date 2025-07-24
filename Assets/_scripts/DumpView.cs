using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DumpView : MonoBehaviour
{
	[Inject] private ITowerManager _towerManager;
	[Inject] private IInputManager _inputManager;
	[SerializeField] private Mask _mask;
	[SerializeField] private PolygonArea _polygon;

	

	private void Awake()
	{
		_inputManager.CurrentCubeView
			.SkipLatestValueOnSubscribe()
			.Pairwise()
			.Subscribe(OnChangeSelection)
			.AddTo(this);
	}

	private void OnChangeSelection(Pair<CubeView> pair)
	{
		var prev = pair.Previous;
		if (pair.Current == null && prev != null && prev.CubeState == CubeState.InTower)
		{

			if (IsInsideMask(prev))
			{
				prev.SetState(CubeState.Dumping);
				_towerManager.TryToDumpCube(prev, prev.RT.anchoredPosition, _mask);
			}
		}
	}

	private bool IsInsideMask(CubeView view)
	{
		var localPos = view.RT.GetPositionInOtherRectTranform(_polygon.RT);
		return _polygon.IsInsidePolygon(localPos);
	}
}