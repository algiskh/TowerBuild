using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Configs Installer", menuName = "Installers/Configs Installer")]
public class ScriptableInstaller : ScriptableObjectInstaller
{
	[SerializeField] private CubeConfigHolder _cubeConfigHolder;
	[SerializeField] private GameValuesHolder _gameValuesHolder;
	[SerializeField] private Localization.LocalizationHolder _localizationHolder;

	public override void InstallBindings()
	{
		Bind(_cubeConfigHolder);
		Bind(_gameValuesHolder);
		Bind(_localizationHolder);
	}

	private void Bind<T>(T instance) where T : ScriptableObject
	{
		if (typeof(T).GetInterfaces().Any())
		{
			Container.BindInterfacesAndSelfTo<T>().FromInstance(instance);
			return;
		}

		Container.BindInstance(instance);
	}
}
