using UnityEngine;

namespace Localization
{

	public static class LocalizationExtensions
	{
		private static ILocalizationManager Localization => LocalizationManager.Instance;

		public static string Localize(this string key)
		{
			if (Localization == null)
			{
				Debug.LogError("LocalizationManager is not initialized!");
				return string.Empty; // Return empty string if localization manager is not available
			}
			return Localization.GetKey(key);
		}
	}
}