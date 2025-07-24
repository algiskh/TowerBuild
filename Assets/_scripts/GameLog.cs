using DG.Tweening;
using Localization;
using UnityEngine;
using UnityEngine.UI;

public interface IGameLog
{
	void SetMessage(string key, bool localize = true);
}

public class GameLog : MonoBehaviour, IGameLog
{
	private const float FADE_DURATION = 0.3f;
	private const float WAIT_DURATION = 3f;
	[SerializeField] private Text _text;
	private Tween _tween;

	public void SetMessage(string key, bool localize = true)
	{
		_tween?.Kill();

		_text.text = localize ? key.Localize() : key;

		var color = _text.color;
		color.a = 0f;
		_text.color = color;

		_tween = DOTween.Sequence()
			.Append(_text.DOFade(1f, FADE_DURATION))      
			.AppendInterval(WAIT_DURATION)         
			.Append(_text.DOFade(0f, FADE_DURATION))      
			.OnComplete(() => _tween = null);
	}
}
