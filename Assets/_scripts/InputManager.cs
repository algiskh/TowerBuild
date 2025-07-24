using Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using Zenject;

public interface IInputManager : IDisposable
{
	IReadOnlyReactiveProperty<CubeView> CurrentCubeView { get; }
	void TryToSelectView(CubeView view);
	void OnDeselectView(CubeView view);
}

public class InputManager : IInputManager
{
	[Inject] private IGameLog _gameLog;

	private ReactiveProperty<CubeView> _currentCubeView = new();
	public IReadOnlyReactiveProperty<CubeView> CurrentCubeView => _currentCubeView;
	private bool IsTouch => (Application.isEditor && Input.GetMouseButton(0)) || (!Application.isEditor && Input.touchCount == 1);
	private Vector2 TouchPosition => Application.isEditor
	? Input.mousePosition
	: (Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero);

	private CompositeDisposable _disposables = new();

	[Inject]
	public InputManager()
	{
		Observable.EveryLateUpdate()
			.Subscribe(_ =>
				{
					//Debug.Log($"Has touch {IsTouch}");
					if (!IsTouch && _currentCubeView.Value != null)
					{
						OnDeselectView(_currentCubeView.Value);
					}
					else if (_currentCubeView.Value != null)
					{
						_currentCubeView.Value.SetScreenPosition(TouchPosition);
					}
				})
			.AddTo(_disposables);
	}


	public void OnDeselectView(CubeView view)
	{
		_currentCubeView.Value = null;
		view?.OnDeselect();
	}

	public void TryToSelectView(CubeView view)
	{
		if (!IsTouch)
		{
			Debug.Log($"No touch. Failed to select");
			return;
		}
		UnityEngine.Debug.Log($"{view.Config.Id} selected");
		_currentCubeView.Value = view;

		var key = _currentCubeView.Value.CubeState == CubeState.InScroll
			? "CUBE_SELECTED_IN_SCROLL"
			: "CUBE_SELECTED_IN_TOWER";
		_gameLog.SetMessage(key);
	}

	public void Dispose()
	{
		_disposables?.Dispose();
	}
}
