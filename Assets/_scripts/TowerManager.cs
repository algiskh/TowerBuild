using ComponentUtils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface ITowerManager : IDisposable
{
	void TryToDumpCube(CubeView view, Vector2 position, Mask mask);
}

public class TowerManager : ITowerManager, IInitializable
{
	[Inject] private IInputManager _inputManager;
	[Inject] private IGameValuesHolder _gameValues;
	[Inject] private IGameLog _gameLog;
	[Inject] private TowerView _view;
	[Inject] private DiContainer _diContainer;

	private CompositeDisposable _disposables = new();
	private readonly List<TowerCubeView> _cubes = new();
	private Pool<TowerCubeView> _cubesPool;
	private TowerCubeView TopCube => _cubes.Count > 0 ? _cubes[_cubes.Count - 1] : null;
	private Vector2 TopCubePosition => TopCube != null ? TopCube.RT.anchoredPosition : Vector2.zero;
	private float CubeHeight => _view.CubePrefab.RT.rect.height;

	public void Initialize()
	{
		_inputManager.CurrentCubeView.SkipLatestValueOnSubscribe().Pairwise().Subscribe(OnChangeSelection).AddTo(_disposables);
		_cubesPool = new Pool<TowerCubeView>(_view.CubePrefab, _view.CubeParent, _diContainer);
	}


	private void OnChangeSelection(Pair<CubeView> pair)
	{
		var prev = pair.Previous;
		if (pair.Current == null && prev != null && prev.CubeState is CubeState.InScroll)
		{
			if (prev.RT.IsRectOverlapping(_view.RT))
			{
				if (TryToGetPlace(prev, out var posToPlace) && FulfillCondition(prev))
				{
					PlaceItem(prev.Config, posToPlace);
				}
				else
				{
					ShowDestroyingItem(prev.Config, posToPlace);
				}
			}
		}
	}

	public void TryToDumpCube(CubeView view, Vector2 position, Mask mask)
	{
		foreach (var cube in _cubes)
		{
			if (cube.View == view)
			{
				Debug.Log($"Dumping cube {view.Config.Id}");
				_cubes.Remove(cube);
				cube.PlayDumpAnimation(() => _cubesPool.Push(cube), position, mask);
				_gameLog.SetMessage("CUBE_DUMPED".Replace("*", view.Config.Id.ToString()));
				break;
			}
		}
		RecalculateTower();
	}

	private void RecalculateTower()
	{
		if (_cubes.Count == 0)
			return;

		var currentY = 0f;

		for (int i = 0; i < _cubes.Count; i++)
		{
			var cube = _cubes[i];

			Debug.Log($"Checking {cube.Config.Id} cube.RT.anchoredPosition.y {cube.RT.anchoredPosition.y} currentY {currentY}");

			if (Mathf.Abs(cube.RT.anchoredPosition.y - currentY) > 0.01f)
			{
				DOTween.Kill(cube.RT);
				// Smoothly move the cube to the target position using DoTween
				cube.RT.DOAnchorPos(new Vector2(cube.RT.anchoredPosition.x, currentY), 0.5f);
			}
			currentY += CubeHeight;
		}
		Debug.Log($"Cubes left : {_cubes.Count}, last Y position: {currentY}");
	}

	private bool FulfillCondition(CubeView view)
	{
		var config = view.Config;
		if (config.PlaceCondition is PlaceCondition.FirstOnly && _cubes.Count > 0) 
		{
			return false;
		}
		if (config.PlaceCondition is PlaceCondition.SameColor && !TopCube.Config.Id.Equals(config.Id))
		{
			return false;
		}

		return true;
	}

	private bool TryToGetPlace(CubeView view, out Vector2 pos)
	{
		if (_cubes.Count == 0)
		{
			var topY = 0;
			pos = view.RT.GetPositionInOtherRectTranform(_view.CubeParent);

			if (pos.y < topY)
			{
				return false;
			}
			pos = pos.SetY(0);
			return true;
		}
		else
		{
			var topY = CubeHeight * _cubes.Count;
			pos = view.RT.GetPositionInOtherRectTranform(_view.CubeParent);

			var topCubeWidth = TopCube.Width;

			var maxAllowedDeviation = topCubeWidth * (_gameValues.PlacementDeviation);

			if (pos.y < topY || Mathf.Abs(pos.x - TopCubePosition.x) > maxAllowedDeviation)
			{
				return false;
			}

			pos = pos.SetY(topY);
			return true;
		}
	}

	private void PlaceItem(CubeConfig config, Vector2 position)
	{
		var cube = _cubesPool.Pop();
		cube.Initialize(config);
		cube.RT.anchoredPosition = position;
		_cubes.Add(cube);
	}

	private void ShowDestroyingItem(CubeConfig config, Vector2 position)
	{
		var cube = _cubesPool.Pop();
		cube.RT.SetAsLastSibling();
		cube.Initialize(config);
		cube.RT.anchoredPosition = position;
		_gameLog.SetMessage("CANNOT_PLACE");
		cube.PlayDestroyAnimation(() => { _cubesPool.Push(cube); cube.SetActive(false); });
	}

	public void Dispose()
	{
		_disposables?.Dispose();
	}

}
