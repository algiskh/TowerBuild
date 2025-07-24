using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SlotView : MonoBehaviour, IPointerDownHandler
{
	[Inject] private IInputManager _inputManager;
	[Inject(Id = "dragContainer")] private RectTransform _dragContainer;
	[SerializeField] private RectTransform _rt;
	[SerializeField] private CubeView _cubeView;
	public RectTransform RT => _rt;
	public CubeConfig Config => _cubeView.Config;
	public float Width => _rt.rect.width;

	public void Initialize(CubeConfig config)
	{
		_cubeView.SetView(config)
		.SetState(CubeState.InScroll);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		var grabOffset = _rt.CalculateGrabOffset(eventData);

		Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
		_cubeView.OnSelect(grabOffset, _dragContainer);
		_inputManager.TryToSelectView(_cubeView);
	}
}
