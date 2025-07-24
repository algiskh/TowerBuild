using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public interface ICubeConfigHolder
{
	IEnumerable<CubeConfig> GetCubeConfigs();
	CubeConfig GetCubeConfigByName(string id);
#if UNITY_EDITOR
	void ApplyParsedData(string data);
#endif
}

[CreateAssetMenu(fileName = "CubeConfigHolder", menuName = "ScriptableObjects/CubeConfigHolder")]
public class CubeConfigHolder : ScriptableObject, ICubeConfigHolder
{
	[SerializeField, TabGroup("Config")] private CubeConfig _defaultConfig;
	[SerializeField] private Sprite[] _cubeSprites;

	/// <summary>
	/// Return default configs for sprites
	/// </summary>
	/// <returns></returns>
	public IEnumerable<CubeConfig> GetCubeConfigs()
	{
		foreach (var sprite in _cubeSprites)
		{
			var cubeConfig = _defaultConfig.Copy(sprite);
			yield return cubeConfig;
		}
	}

	public CubeConfig GetCubeConfigByName(string id)
	{
		foreach (var sprite in _cubeSprites)
		{
			if (sprite.name.Equals(id))
			{
				return _defaultConfig.Copy(sprite);
			}
		}
		return null;
	}

#if UNITY_EDITOR
	public void ApplyParsedData(string data)
	{
		// logic for parsed data 
		UnityEditor.EditorUtility.SetDirty(this);
	}
#endif
}
