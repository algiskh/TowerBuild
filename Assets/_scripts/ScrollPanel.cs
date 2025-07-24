using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;
using UniRx;


public class ScrollPanel : MonoBehaviour
{
	[Inject] private IInputManager _inputManager;
	[Inject] private ICubeConfigHolder _cubeConfigHolder;
	[Inject] private DiContainer _container;

	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private RectTransform _content;
	[SerializeField] private SlotView _slotPrefab;
	[SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;

	private readonly List<SlotView> _slots = new();
	private bool _isInitialized = false;
	private float _slotWidth;
	private float _viewportWidth;

	private float Spacing => _horizontalLayoutGroup.spacing;

	private void Awake()
	{
		_inputManager.CurrentCubeView.SkipLatestValueOnSubscribe().Subscribe(OnChangeSelection).AddTo(this);
		_scrollRect.movementType = ScrollRect.MovementType.Unrestricted; // <-- ключевое условие!
	}

	private void OnChangeSelection(CubeView view)
	{
		Debug.Log($"Selected: {view != null}");
		_scrollRect.enabled = view == null;
	}

	private void Start()
	{
		if (!_isInitialized)
			FillScroll();

		_slotWidth = _slotPrefab.Width;
		_viewportWidth = _scrollRect.viewport.rect.width;
	}

	private void LateUpdate()
	{
		if (!_isInitialized)
			return;

		var first = _slots[0];
		var last = _slots[_slots.Count - 1];

		var firstEdge = first.RT.GetPositionInOtherRectTranform(_scrollRect.viewport).x;
		var lastEdge = last.RT.GetPositionInOtherRectTranform(_scrollRect.viewport).x;

		if (firstEdge < 0 && lastEdge < _viewportWidth)
		{
			MoveFirstToLast(first);
		}
		else if (lastEdge > _viewportWidth && firstEdge > 0)
		{
			MoveLastToFirst(last);
		}
	}

	private void MoveLastToFirst(SlotView last)
	{

		last.RT.SetAsFirstSibling();
		last.RT.anchoredPosition = new Vector2(
			last.RT.anchoredPosition.x - (_slotWidth * _slots.Count + (Spacing * _slots.Count - 1)),
			last.RT.anchoredPosition.y
		);
		_slots.Remove(last);
		_slots.Insert(0, last);
	}

	private void MoveFirstToLast(SlotView first)
	{
		first.RT.SetAsLastSibling();
		first.RT.anchoredPosition = new Vector2(
			first.RT.anchoredPosition.x + (_slotWidth * _slots.Count + (Spacing * _slots.Count - 1)),
			first.RT.anchoredPosition.y
		);
		_slots.Remove(first);
		_slots.Add(first);
	}


	[Sirenix.OdinInspector.Button]
	private void FillScroll()
	{
		if (_slots.Count > 0)
			return;
		_horizontalLayoutGroup.enabled = true;

		var configs = _cubeConfigHolder.GetCubeConfigs();

		foreach (var config in configs)
		{
			var slot = _container.InstantiatePrefabForComponent<SlotView>(_slotPrefab, _content);
			slot.Initialize(config);
			_slots.Add(slot);
		}
		Canvas.ForceUpdateCanvases();
		_horizontalLayoutGroup.enabled = false;
		_isInitialized = true;
	}
}
