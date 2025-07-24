using Zenject;

namespace Localization
{
	public interface ILocalizationManager
	{
		string GetKey(string key);
		static ILocalizationManager Instance { get; }
	}

	public class LocalizationManager : ILocalizationManager, IInitializable
	{
		[Inject] private ILocalizationHolder _localizationHolder;
		private Language _currentLanguage;
		public static ILocalizationManager Instance { get; private set; }

		public void Initialize()
		{
			Instance ??= this;
			_currentLanguage = _localizationHolder.DefaultLanguage;
		}

		public string GetKey(string key)
		{
			return _localizationHolder.GetKey(key, _currentLanguage);
		}
	}
}