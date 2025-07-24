using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class TowerCubeView : MonoBehaviour, IPointerDownHandler
{
	private const float JUMP_DURATION = 0.25f;
	private const float SQUASH_DURATION = 0.07f;
	private const float FALL_ROTATION = 40f;
	private const float FALL_DURATION = 0.5f;

	[Inject] private IInputManager _inputManager;
	[Inject(Id = "dragContainer")] private RectTransform _dragContainer;
	[SerializeField] private RectTransform _rt;
	[SerializeField] private CubeView _cubeView;
	private Tween _tween;

	public RectTransform RT => _rt;
	public CubeConfig Config => _cubeView.Config;
	public float Width => _rt.rect.width;
	private float Height => _rt.rect.height;
	public CubeView View => _cubeView;

	public void Initialize(CubeConfig config)
	{
		_tween?.Kill();
		_cubeView.ResetScale();

		_cubeView.SetView(config)
			.SetState(CubeState.InTower);
		gameObject.SetActive(true);
		PlayJumpAnimation();
	}

	public void Hide()
	{
		_tween?.Kill();
		gameObject.SetActive(false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		var grabOffset = _rt.CalculateGrabOffset(eventData);

		Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
		_cubeView.OnSelect(grabOffset, _dragContainer);
		_inputManager.TryToSelectView(_cubeView);
	}

	#region Animations
	public void PlayJumpAnimation()
	{
		_tween?.Kill();

		var jumpHeight = Height / 2;
		var originalScale = _cubeView.RT.localScale;
		var squashScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);

		var sequence = DOTween.Sequence();
		sequence.Append(_cubeView.RT.DOAnchorPosY(_cubeView.RT.anchoredPosition.y + jumpHeight, JUMP_DURATION / 2).SetEase(Ease.OutQuad))
			.Append(_cubeView.RT.DOAnchorPosY(_cubeView.RT.anchoredPosition.y, JUMP_DURATION / 2).SetEase(Ease.InQuad))
			.Append(_cubeView.RT.DOScale(squashScale, SQUASH_DURATION).SetEase(Ease.OutQuad))
			.Append(_cubeView.RT.DOScale(originalScale, SQUASH_DURATION).SetEase(Ease.InQuad));

		_tween = sequence;
	}

	public void PlayDestroyAnimation(Action onComplete)
	{
		_tween?.Kill();
		_cubeView.ResetScale();

		var rt = _cubeView.RT;
		var fallDistance = Height * 2;
		var randomRotation = UnityEngine.Random.Range(-FALL_ROTATION, FALL_ROTATION);
		var targetPos = rt.anchoredPosition + Vector2.down * fallDistance;

		_tween = DOTween.Sequence()
			.Join(rt.DOAnchorPos(targetPos, FALL_DURATION).SetEase(Ease.InSine))
			.Join(rt.DORotate(new Vector3(0, 0, randomRotation), FALL_DURATION, RotateMode.FastBeyond360).SetEase(Ease.InOutSine))
			.Join(rt.DOScale(Vector3.zero, FALL_DURATION).SetEase(Ease.InSine))
			.OnComplete(() =>
			{
				Hide();
				onComplete?.Invoke();
			});
	}

	public void PlayDumpAnimation(Action onComlete, Vector2 position, Mask mask)
	{
		_tween?.Kill();
		_cubeView.ResetScale();
		_cubeView.SetState(CubeState.Dumping);

		var maskRect = mask.rectTransform;
		var maskCenter = maskRect.rect.center;

		var anchoredTarget = maskRect.GetAnchoredPositionInOtherRect(_cubeView.RT.parent as RectTransform);

		_cubeView.RT.anchoredPosition = position;
		Debug.Log($"anchoredTarget {anchoredTarget}");

		_tween = _cubeView.RT.DOAnchorPos(anchoredTarget, JUMP_DURATION).SetEase(Ease.InOutSine).OnComplete(() =>
		{
			_cubeView.RT.SetParent(maskRect, worldPositionStays: true);
			_cubeView.RT.anchoredPosition = maskCenter;

			var downTarget = maskCenter + Vector2.down * (_cubeView.RT.rect.height * 1.5f);

			var seq = DOTween.Sequence();
			seq.Append(_cubeView.RT.DOAnchorPos(downTarget, JUMP_DURATION).SetEase(Ease.InQuad));
			seq.Join(_cubeView.RT.DOLocalRotate(new Vector3(0, 0, -60), JUMP_DURATION));
			seq.OnComplete(() =>
			{
				_cubeView.RT.SetParent(_rt);
				onComlete?.Invoke();
			});
			_tween = seq;
		});
	}

	#endregion
}

