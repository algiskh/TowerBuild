using UnityEngine;
using UnityEngine.UI;

public enum CubeState
{
	InScroll,
	InTower,
	Destroying,
	Dumping
}

public class CubeView : MonoBehaviour
{
	[SerializeField] private Image _image;
	[SerializeField] private RectTransform _rectTransform;

	private CubeConfig _config;
	private Vector2 _originalPos = Vector2.zero;
	private RectTransform _originalParent;
	private Vector2 _grabOffset;

	public CubeConfig Config => _config;
	public float Width => _rectTransform.rect.width;
	public RectTransform RT => _rectTransform;
	public CubeState CubeState { get; private set; }

	public CubeView SetView(CubeConfig config)
	{
		_rectTransform.anchoredPosition = Vector2.zero;
		_rectTransform.rotation = Quaternion.identity;
		_config = config;
		_image.sprite = config.Sprite;
		_originalPos = _rectTransform.anchoredPosition;
		_originalParent = _rectTransform.parent as RectTransform;
		SetTransparent(false);
		gameObject.SetActive(true);
		return this;
	}

	public CubeView SetState(CubeState state)
	{
		CubeState = state;
		return this;
	}

	public void SetTransparent(bool transparent)
	{
		_image.color = _image.color.WithAlpha(transparent ? 0.5f : 1f);
	}

	public void SetScreenPosition(Vector2 screenPosition)
	{
		_rectTransform.SetAnchoredPositionByScreenPoint(screenPosition, _grabOffset);
	}

	public void OnSelect(Vector2 grabOffset, RectTransform dragContainer)
	{
		_rectTransform.SetParent(dragContainer, true);
		_grabOffset = grabOffset;
		SetTransparent(true);
	}

	public void OnDeselect()
	{
		if (CubeState == CubeState.Dumping)
		{
			return;
		}

		_rectTransform.SetParent(_originalParent, false);
		SetTransparent(false);
		_rectTransform.anchoredPosition = _originalPos;
	}

	public void ResetScale()
	{
		_rectTransform.localScale = Vector2.one;
	}
}